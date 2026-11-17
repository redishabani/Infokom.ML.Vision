using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Main class for capturing audio from Kinect and get access to sound source localization.
	/// </summary>
	public sealed class KinectAudioSource
	{
		/// <summary>
		/// Beamforming mode for the microphone array.
		/// Values map to the values of DMO property MFPKEY_WMAAECMA_FEATR_MICARR_MODE.
		/// </summary>
		internal enum _MIC_ARRAY_MODE
		{
			/// <summary>
			/// Single channel. Last 8 bits specify which channel to use - NOT USED.
			/// </summary>
			SingleChannel = 0,
			/// <summary>
			/// Simply sum all channels - NOT USED.
			/// </summary>
			SimpleSum = 256,
			/// <summary>
			/// Perform beamformer. Beam selected by internal source localizer.
			/// </summary>
			SingleBeam = 512,
			/// <summary>
			/// Perform beamformer. Always select the center beam - NOT USED.
			/// </summary>
			FixedBeam = 1024,
			/// <summary>
			/// Perform beamformer. Beam selected externally by applications.
			/// </summary>
			ExternalBeam = 2048,
			/// <summary>
			/// Perform adaptive beamformer. Beam selected by internal source localizer.
			/// </summary>
			AdaptiveBeam = 4352
		}

		/// <summary>
		/// System mode for the microphone array echo cancellation pipeline.
		/// Values map to the values of DMO property MFPKEY_WMAAECMA_FEATR_SYSTEM_MODE.
		/// </summary>
		internal enum _AEC_SYSTEM_MODE
		{
			/// <summary>
			/// Audio data from microphone array will be streamed with NO AEC processing applied to it.
			/// </summary>
			ArrayOnly = 2,
			/// <summary>
			/// Audio data from microphone array will be streamed with AEC processing applied to it.
			/// </summary>
			ArrayAndEchoCancellation = 4
		}

		/// <summary>
		/// Value of first valid DMO property ID.
		/// </summary>
		internal const int PID_FIRST_USABLE = 2;

		/// <summary>
		/// ID of DMO property used to turn Acoustic Echo Cancellation mode on and off.
		/// </summary>
		internal const int MFPKEY_WMAAECMA_SYSTEM_MODE = 2;

		/// <summary>
		/// ID of DMO property used to toggle between source mode and filter mode.
		/// </summary>
		internal const int MFPKEY_WMAAECMA_DMO_SOURCE_MODE = 3;

		/// <summary>
		/// ID of DMO property used to specify the indexes of microphone and speaker devices
		/// that are bound to DMO.
		/// </summary>
		internal const int MFPKEY_WMAAECMA_DEVICE_INDEXES = 4;

		/// <summary>
		/// ID of DMO property used to turn Feature mode on and off.
		/// </summary>
		internal const int MFPKEY_WMAAECMA_FEATURE_MODE = 5;

		/// <summary>
		/// ID of DMO property used to turn Noise Suppression on and off.
		/// </summary>
		internal const int MFPKEY_WMAAECMA_FEATR_NS = 8;

		/// <summary>
		/// ID of DMO property used to turn Automatic Gain Control on and off.
		/// </summary>
		internal const int MFPKEY_WMAAECMA_FEATR_AGC = 9;

		/// <summary>
		/// ID of DMO property used to configure how many times acoustic echo suppression algorithm is run.
		/// </summary>
		internal const int MFPKEY_WMAAECMA_FEATR_AES = 10;

		/// <summary>
		/// ID of DMO property used to configure beamforming mode.
		/// </summary>
		internal const int MFPKEY_WMAAECMA_FEATR_MICARR_MODE = 18;

		/// <summary>
		/// Gets the smallest possible value for <see cref="P:Microsoft.Kinect.KinectAudioSource.BeamAngle" />.
		/// </summary>
		private const double s_minBeamAngle = -50.0;

		/// <summary>
		/// Gets the largest possible value for <see cref="P:Microsoft.Kinect.KinectAudioSource.BeamAngle" />.
		/// </summary>
		private const double s_maxBeamAngle = 50.0;

		/// <summary>
		/// Gets the smallest possible value for <see cref="P:Microsoft.Kinect.KinectAudioSource.SoundSourceAngle" />.
		/// </summary>
		private const double s_minSoundSourceAngle = -50.0;

		/// <summary>
		/// Gets the largest possible value for <see cref="P:Microsoft.Kinect.KinectAudioSource.SoundSourceAngle" />.
		/// </summary>
		private const double s_maxSoundSourceAngle = 50.0;

		/// <summary>
		/// Name of audio capture thread.
		/// </summary>
		internal const string AUDIO_CAPTURE_THREAD_NAME = "AudioCaptureThread";

		/// <summary>
		/// Format GUID used to identify DMO property keys.
		/// </summary>
		internal static readonly Guid MFPKEY_WMAAECMA_KEY_BASE = new Guid(1867695463, 864, 19410, new byte[8] { 150, 23, 204, 191, 20, 33, 201, 57 });

		/// <summary>
		/// Default value for stale read threshold overridable in <see cref="M:Microsoft.Kinect.KinectAudioSource.Start" />.
		/// </summary>
		internal static readonly TimeSpan DefaultReadStaleThreshold = TimeSpan.FromMilliseconds(500.0);

		/// <summary>
		/// Dictionary of DMO property keys to property values.
		/// </summary>
		private readonly Dictionary<PROPERTYKEY, object> _dmoProperties = new Dictionary<PROPERTYKEY, object>();

		/// <summary>
		/// <code>true</code> when there are properties set by client still pending to be applied to DMO.
		/// <code>false</code> otherwise.
		/// </summary>
		private bool _applyProperties;

		/// <summary>
		/// Facade object used to call into native layer in an easily mockable way.
		/// </summary>
		private readonly INativeAudioWrapper _nativeAudioWrapper;

		/// <summary>
		/// Lock object used to protect DMO property get/set operations.
		/// </summary>
		private readonly object _propertyLock = new object();

		/// <summary>
		/// Lock object used to protect audio capture initialization, uninitialization and resource disposal.
		/// </summary>
		private readonly object _initLock = new object();

		/// <summary>
		/// <see cref="T:Microsoft.Kinect.KinectAudioStream" /> actively capturing audio.
		/// If <code>null</code>, no stream is currently being captured.
		/// </summary>
		private KinectAudioStream _stream;

		/// <summary>
		/// Sound source angle detected by DMO during audio capture.
		/// </summary>
		private double _soundSourceAngle;

		/// <summary>
		/// Confidence measured by DMO for sound source angle value, during audio capture.
		/// </summary>
		private double _soundSourceAngleConfidence;

		/// <summary>
		/// Beam angle detected by DMO during audio capture.
		/// </summary>
		private double _beamAngle;

		/// <summary>
		/// Manual beam angle specified by client.
		/// </summary>
		private double _manualBeamAngle;

		/// <summary>
		/// Beam angle to set on DMO in capture thread.
		/// </summary>
		private double _beamAngleSetValue = double.MinValue;

		/// <summary>
		/// The <see cref="T:Microsoft.Kinect.KinectSensor" /> object that owns this audio source.
		/// </summary>
		private KinectSensor _sensor;

		/// <summary>
		/// Handler for event that fires when the sound source changes.
		/// </summary>
		private readonly ContextEventHandler<BeamAngleChangedEventArgs> _BeamAngleChangedContextHandler;

		/// <summary>
		/// Handler for event that fires when the sound source changes.
		/// </summary>
		private readonly ContextEventHandler<SoundSourceAngleChangedEventArgs> _SoundSourceAngleChangedContextHandler;

		/// <summary>
		/// The specified <see cref="P:Microsoft.Kinect.KinectAudioSource.EchoCancellationMode" /> value that determines how the DMO
		/// performs echo cancellation and suppression.
		/// </summary>
		private EchoCancellationMode _echoCancellationMode;

		/// <summary>
		/// The specified <see cref="P:Microsoft.Kinect.KinectAudioSource.BeamAngleMode" /> value that determines how the beam angle
		/// is controlled.
		/// </summary>
		private BeamAngleMode _beamAngleMode;

		/// <summary>
		/// Gets a reference to the DMO wrapper, used internally by the audio stream.
		/// </summary>
		internal INativeAudioWrapper NativeWrapper => _nativeAudioWrapper;

		/// <summary>
		/// Gets or sets microphone and speaker device indexes currently bound to DMO.
		/// Maps to the DMO property MFPKEY_WMAAECMA_DEVICE_INDEXES.
		/// </summary>
		private int DeviceIndexes
		{
			get
			{
				object property = GetProperty(4);
				if (property == null)
				{
					return 0;
				}
				return (int)property;
			}
			set
			{
				SetProperty(4, value);
			}
		}

		/// <summary>
		/// Sets the index of microphone device associated with audio processing pipeline.
		/// </summary>
		/// <remarks>
		/// Internally used to match the sensor to its audio device.
		/// </remarks>
		internal ushort MicrophoneIndex
		{
			set
			{
				DeviceIndexes = (int)(DeviceIndexes & 0xFFFF0000u) | value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether feature mode is turned on.
		/// ON by default.
		/// Maps to the DMO property MFPKEY_WMAAECMA_FEATURE_MODE.
		/// </summary>
		/// <remarks>
		/// If feature mode is enabled, this in turn enables turning on DMO features
		/// such as noise suppression and acoustic gain control.
		/// </remarks>
		private bool FeatureMode
		{
			get
			{
				object property = GetProperty(5);
				if (property == null)
				{
					return false;
				}
				return (bool)property;
			}
			set
			{
				SetProperty(5, value);
			}
		}

		/// <summary>
		/// Sets the microphone array beam forming mode.
		/// Requires FeatureMode set.
		/// _MIC_ARRAY_MODE.SingleBeam by default.
		/// Maps to the DMO property MFPKEY_WMAAECMA_FEATR_MICARR_MODE.
		/// </summary>
		private _MIC_ARRAY_MODE MicArrayMode
		{
			set
			{
				VerifyFeatureMode();
				SetProperty(18, (int)value);
			}
		}

		/// <summary>
		/// Gets the smallest possible value for <see cref="P:Microsoft.Kinect.KinectAudioSource.BeamAngle" />.
		/// </summary>
		public static double MinBeamAngle => -50.0;

		/// <summary>
		/// Gets the largest possible value for <see cref="P:Microsoft.Kinect.KinectAudioSource.BeamAngle" />.
		/// </summary>
		public static double MaxBeamAngle => 50.0;

		/// <summary>
		/// Gets the smallest possible value for <see cref="P:Microsoft.Kinect.KinectAudioSource.SoundSourceAngle" />.
		/// </summary>
		public static double MinSoundSourceAngle => -50.0;

		/// <summary>
		/// Gets the largest possible value for <see cref="P:Microsoft.Kinect.KinectAudioSource.SoundSourceAngle" />.
		/// </summary>
		public static double MaxSoundSourceAngle => 50.0;

		/// <summary>
		/// Gets or sets the echo cancellation and suppression mode.
		/// Default is None (i.e.: echo cancellation and echo suppression are both turned off).
		/// </summary>
		public EchoCancellationMode EchoCancellationMode
		{
			get
			{
				return _echoCancellationMode;
			}
			set
			{
				switch (value)
				{
				case EchoCancellationMode.None:
					SetEchoCancellationMode(0, _AEC_SYSTEM_MODE.ArrayOnly);
					break;
				case EchoCancellationMode.CancellationOnly:
					SetEchoCancellationMode(0, _AEC_SYSTEM_MODE.ArrayAndEchoCancellation);
					break;
				case EchoCancellationMode.CancellationAndSuppression:
					SetEchoCancellationMode(1, _AEC_SYSTEM_MODE.ArrayAndEchoCancellation);
					break;
				default:
					throw new InvalidOperationException(Resources.InvalidEchoCancellationMode);
				}
				_echoCancellationMode = value;
			}
		}

		/// <summary>
		/// Gets or sets the beam angle mode that determines how the beam angle is controlled.
		/// Default is Automatic.
		/// </summary>
		public BeamAngleMode BeamAngleMode
		{
			get
			{
				return _beamAngleMode;
			}
			set
			{
				switch (value)
				{
				case BeamAngleMode.Adaptive:
					MicArrayMode = _MIC_ARRAY_MODE.AdaptiveBeam;
					break;
				case BeamAngleMode.Automatic:
					MicArrayMode = _MIC_ARRAY_MODE.SingleBeam;
					break;
				case BeamAngleMode.Manual:
					MicArrayMode = _MIC_ARRAY_MODE.ExternalBeam;
					break;
				default:
					throw new InvalidOperationException(Resources.InvalidBeamAngleMode);
				}
				_beamAngleMode = value;
			}
		}

		/// <summary>
		/// Gets the most recent sound source position observed, in degrees.
		/// </summary>
		/// <remarks>
		/// This property gets updated continuously after calling Start().
		/// </remarks>
		public double SoundSourceAngle => _soundSourceAngle * 180.0 / Math.PI;

		/// <summary>
		/// Gets the most recent measurement of our confidence in the sound source position.
		/// This value is in the range [0,1], 1 being the highest possible confidence.
		/// </summary>
		/// <remarks>
		/// This property gets updated continuously after calling Start().
		/// </remarks>
		public double SoundSourceAngleConfidence => _soundSourceAngleConfidence;

		/// <summary>
		/// Gets or sets index of speaker device used to identify the sound being
		/// produced by computer in order to perform echo cancellation.
		/// </summary>
		public int EchoCancellationSpeakerIndex
		{
			get
			{
				return (int)((DeviceIndexes & 0xFFFF0000u) >> 16);
			}
			set
			{
				DeviceIndexes = (DeviceIndexes & 0xFFFF) | (value << 16);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether Noise Suppression is enabled.
		/// ON by default.
		/// Maps to the DMO property MFPKEY_WMAAECMA_FEATR_NS.
		/// </summary>
		public bool NoiseSuppression
		{
			get
			{
				object property = GetProperty(8);
				if (property == null)
				{
					return false;
				}
				return 0 != (int)property;
			}
			set
			{
				VerifyFeatureMode();
				SetProperty(8, value ? 1 : 0);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether Automatic Gain Control is enabled.
		/// OFF by default. 
		/// Recommended for non-speech scenarios.
		/// Maps to the DMO property MFPKEY_WMAAECMA_FEATR_AGC.
		/// </summary>
		public bool AutomaticGainControlEnabled
		{
			get
			{
				object property = GetProperty(9);
				if (property == null)
				{
					return false;
				}
				return (bool)property;
			}
			set
			{
				VerifyFeatureMode();
				SetProperty(9, value);
			}
		}

		/// <summary>
		/// Gets the sound source angle that the audio array is currently focusing on, in degrees.
		/// </summary>
		public double BeamAngle => _beamAngle * 180.0 / Math.PI;

		/// <summary>
		/// Gets or sets manual Beam angle, in degrees.
		/// </summary>
		/// <remarks>
		/// Only valid when <see cref="P:Microsoft.Kinect.KinectAudioSource.BeamAngleMode" /> is set to Manual.
		/// </remarks>
		public double ManualBeamAngle
		{
			get
			{
				return _manualBeamAngle;
			}
			set
			{
				_manualBeamAngle = value;
				_beamAngleSetValue = _manualBeamAngle * Math.PI / 180.0;
			}
		}

		/// <summary>
		/// Event that fires when the beam angle changes.
		/// </summary>
		public event EventHandler<BeamAngleChangedEventArgs> BeamAngleChanged
		{
			add
			{
				_BeamAngleChangedContextHandler.AddHandler(value);
			}
			remove
			{
				_BeamAngleChangedContextHandler.RemoveHandler(value);
			}
		}

		/// <summary>
		/// Event that fires when the beam angle changes.
		/// </summary>
		public event EventHandler<SoundSourceAngleChangedEventArgs> SoundSourceAngleChanged
		{
			add
			{
				_SoundSourceAngleChangedContextHandler.AddHandler(value);
			}
			remove
			{
				_SoundSourceAngleChangedContextHandler.RemoveHandler(value);
			}
		}

		/// <summary>
		/// Returns a <see cref="T:Microsoft.Kinect.PROPERTYKEY" /> that corresponds to the specified property id.
		/// </summary>
		/// <param name="pid">
		/// ID of property for which corresponding <see cref="T:Microsoft.Kinect.PROPERTYKEY" /> will be returned.
		/// </param>
		/// <returns>
		/// <see cref="T:Microsoft.Kinect.PROPERTYKEY" /> corresponding to <paramref name="pid" /> property ID.
		/// </returns>
		internal static PROPERTYKEY PropertyKeyFromId(int pid)
		{
			return new PROPERTYKEY
			{
				pid = pid,
				fmtid = MFPKEY_WMAAECMA_KEY_BASE
			};
		}

		/// <summary>
		/// Sets DMO property value in DMO property dictionary and flags properties
		/// as pending to be applied to DMO object.
		/// </summary>
		/// <param name="pid">ID of property to set.</param>
		/// <param name="value">Value of property to set.</param>
		private void SetProperty(int pid, object value)
		{
			PROPERTYKEY key = PropertyKeyFromId(pid);
			lock (_propertyLock)
			{
				_dmoProperties[key] = value;
				_applyProperties = true;
			}
		}

		/// <summary>
		/// Gets DMO property value from DMO property dictionary.
		/// </summary>
		/// <param name="pid">ID of property to get.</param>
		/// <returns>
		/// Value of requested property. Will be null before first time property is set, so internal code should
		/// handle this and also set appropriate defaults for all properties accessible to clients.
		/// </returns>
		private object GetProperty(int pid)
		{
			object result = null;
			PROPERTYKEY key = new PROPERTYKEY
			{
				pid = pid,
				fmtid = MFPKEY_WMAAECMA_KEY_BASE
			};
			lock (_propertyLock)
			{
				if (_dmoProperties.ContainsKey(key))
				{
					result = _dmoProperties[key];
				}
			}
			return result;
		}

		/// <summary>
		/// Method to be called in audio capture thread to apply DMO properties specified by client
		/// in main thread.
		/// </summary>
		/// <param name="propertyStore">
		/// Property store to which queued-up properties will be applied.
		/// </param>
		/// <remarks>
		/// Whenever a property is set, we flag that we have properties pending to be applied to DMO.
		/// Also, whenever a capture thread is started we also flag that we have properties pending
		/// to be applied to DMO.
		/// Then, whenever capture thread notices that this flag is set, it applies all property
		/// values in property bag to DMO, not merely the ones that have changed since last time.
		/// This won't result in a noticeable performance overhead, because there are only a handful
		/// of DMO properties that we care about, and doing things this way is simpler than having a
		/// mechanism to keep track of property diffs.
		/// </remarks>
		internal void ApplyPendingProperties(IPropertyStore propertyStore)
		{
			lock (_propertyLock)
			{
				if (!_applyProperties)
				{
					return;
				}
				foreach (PROPERTYKEY key2 in _dmoProperties.Keys)
				{
					PROPERTYKEY key = key2;
					object pv = _dmoProperties[key2];
					propertyStore.SetValue(ref key, ref pv);
				}
				_applyProperties = false;
			}
		}

		/// <summary>
		/// Throws an exception if feature mode is not turned on.
		/// Called by setters for properties that require feature mode
		/// to be turned on for proper functioning.
		/// </summary>
		private void VerifyFeatureMode()
		{
			if (!FeatureMode)
			{
				throw new InvalidOperationException(Resources.NeedFeatureMode);
			}
		}

		/// <summary>
		/// Initializes a new instance of the KinectAudioSource class.
		/// </summary>
		/// <param name="sensor">
		/// <see cref="T:Microsoft.Kinect.KinectSensor" /> that owns this audio source.
		/// </param>
		/// <param name="audioWrapper">
		/// Facade to use for performing native call operations.
		/// </param>
		/// <remarks>
		/// This constructor is internal and is injectable with different <see cref="T:Microsoft.Kinect.INativeAudioWrapper" />
		/// objects for testing purposes.
		/// </remarks>
		internal KinectAudioSource(KinectSensor sensor, INativeAudioWrapper audioWrapper)
		{
			_BeamAngleChangedContextHandler = new ContextEventHandler<BeamAngleChangedEventArgs>();
			_SoundSourceAngleChangedContextHandler = new ContextEventHandler<SoundSourceAngleChangedEventArgs>();
			_sensor = sensor;
			_nativeAudioWrapper = audioWrapper;
			_beamAngle = 0.0;
			_manualBeamAngle = 0.0;
			_soundSourceAngle = 0.0;
			_soundSourceAngleConfidence = 0.0;
			SetProperty(3, true);
			FeatureMode = true;
			EchoCancellationMode = EchoCancellationMode.None;
			EchoCancellationSpeakerIndex = GetDefaultSpeakerIndex();
			BeamAngleMode = BeamAngleMode.Automatic;
			NoiseSuppression = true;
			AutomaticGainControlEnabled = false;
		}

		/// <summary>
		/// Initializes a new instance of the KinectAudioSource class, with default properties set.
		/// </summary>
		/// <param name="sensor">
		/// <see cref="T:Microsoft.Kinect.KinectSensor" /> that owns this audio source.
		/// </param>
		internal KinectAudioSource(KinectSensor sensor)
			: this(sensor, new DmoAudioWrapper(sensor))
		{
		}

		/// <summary>
		/// Starts capturing audio from the device. The data can be read using the returned stream.
		/// </summary>     
		/// <param name="readStaleThreshold">
		/// <para>
		/// If there are no reads to the stream for longer than this threshold the DMO discards
		/// any buffered audio. This prevents stale data from being returned in scenarios such as speech recognition and dialog
		/// systems, when the consumption of audio samples may stop for a while.
		/// </para>
		/// <para>
		/// Higher values for this threshold (e.g.: multiple seconds) are advisable if clients notice dropped audio packets,
		/// such as is more likely in high CPU-load scenarios.
		/// </para>
		/// <para>
		/// Pass TimeSpan.MaxValue to avoid hitting this threshold.
		/// </para>
		/// </param>   
		/// <returns>
		/// A stream of audio samples, in 16khz, 16-bit PCM format.
		/// </returns>
		/// <remarks>
		/// If the stream data is not read and the buffered audio exceeds the size specified in <paramref name="readStaleThreshold" />,
		/// the oldest samples are discarded.
		/// </remarks>
		public Stream Start(TimeSpan readStaleThreshold)
		{
			lock (_initLock)
			{
				if (_sensor == null || !_sensor.IsRunning)
				{
					throw new InvalidOperationException(Resources.SensorMustBeRunningForAudio);
				}
				if (_stream != null)
				{
					Stop();
				}
				if (_stream != null)
				{
					throw new InvalidOperationException(Resources.CaptureAlreadyStarted);
				}
				_applyProperties = true;
				_stream = new KinectAudioStream(this, readStaleThreshold);
				_stream.Start();
			}
			return _stream;
		}

		/// <summary>
		/// Starts capturing audio from the device. The data can be read using the returned stream.
		/// </summary>
		/// <returns>
		/// A stream of audio samples, in 16khz, 16-bit PCM format.
		/// </returns>
		/// <remarks>
		/// If the data is not read for more than 500ms, buffered data is discarded to prevent returning old audio.
		/// </remarks>
		public Stream Start()
		{
			return Start(DefaultReadStaleThreshold);
		}

		/// <summary>
		/// Stops the capturing and closes the stream.
		/// </summary>
		public void Stop()
		{
			lock (_initLock)
			{
				if (_stream != null)
				{
					_stream.Stop();
					_stream.Dispose();
					_stream = null;
				}
			}
		}

		/// <summary>
		/// Change the echo cancellation and echo suppression configuration according to the
		/// specified values.
		/// </summary>
		/// <param name="echoSuppression">
		/// Specifies how many times the DMO will perform acoustic echo suppression (AES) on
		/// the residual signal after echo cancellation has been applied.
		/// 0 implies echo suppression is turned off, and 1 and 2 are acceptable values used
		/// to turn this on.
		/// </param>
		/// <param name="sysMode">
		/// Specifies which acoustic echo cancellation mode we should use.
		/// _AEC_SYSTEM_MODE.ArrayAndEchoCancellation turns AEC on and
		/// _AEC_SYSTEM_MODE.ArrayOnly turns AEC off.
		/// </param>
		private void SetEchoCancellationMode(int echoSuppression, _AEC_SYSTEM_MODE sysMode)
		{
			VerifyFeatureMode();
			SetProperty(10, echoSuppression);
			SetProperty(2, (int)sysMode);
		}

		/// <summary>
		/// Updates the current beam angle and fires the event.
		/// </summary>
		/// <param name="angle">Beam angle, in radians.</param>
		internal void SetBeamAngle(double angle)
		{
			_beamAngle = angle;
			BeamAngleChangedEventArgs e = new BeamAngleChangedEventArgs();
			e.Angle = BeamAngle;
			BeamAngleChangedEventArgs eventArgs = e;
			_BeamAngleChangedContextHandler.Invoke(this, eventArgs);
		}

		/// <summary>
		/// Updates the current sound sounce angle and confidence values, 
		/// and fires the event if beyond a threshold.
		/// </summary>
		/// <param name="angle">Sound source angle, in radians.</param>
		/// <param name="confidence">ConfidenceLevel, in 0-1 range.</param>
		internal void SetSoundSourceAngleAndConfidence(double angle, double confidence)
		{
			_soundSourceAngle = angle;
			_soundSourceAngleConfidence = confidence;
			SoundSourceAngleChangedEventArgs e = new SoundSourceAngleChangedEventArgs();
			e.Angle = SoundSourceAngle;
			e.ConfidenceLevel = SoundSourceAngleConfidence;
			SoundSourceAngleChangedEventArgs eventArgs = e;
			_SoundSourceAngleChangedContextHandler.Invoke(this, eventArgs);
		}

		/// <summary>
		/// Returns the angle last set by the client and then clears it (sets it to double.MinValue).
		/// </summary>
		/// <returns>
		/// The manual beam angle (in degrees) to be applied to DMO.
		/// </returns>
		internal double FetchNextBeamAngle()
		{
			double num = 0.0;
			num = _beamAngleSetValue;
			_beamAngleSetValue = double.MinValue;
			return num;
		}

		/// <summary>
		/// Enumerates all sound capture (e.g.: microphone) devices in system.
		/// </summary>
		/// <returns>
		/// An enumeration of sound capture device metadata, where the metadata
		/// returned for each enumerated device is enough to uniquely identify
		/// capture device within system.
		/// </returns>
		private IEnumerable<MicrophoneArrayDevice> FindCaptureDevices()
		{
			int count = 0;
			_nativeAudioWrapper.GetMicrophoneArrayDevices(null, ref count);
			MicrophoneArrayDevice[] array = new MicrophoneArrayDevice[count];
			_nativeAudioWrapper.GetMicrophoneArrayDevices(array, ref count);
			return array;
		}

		/// <summary>
		/// Returns the device index corresponding to the default speaker for the system.
		/// </summary>
		/// <returns>
		/// An integer in range [0,{number of speaker devices}-1] representing the device
		/// index for the default speaker device.
		/// </returns>
		private int GetDefaultSpeakerIndex()
		{
			int count = 0;
			_nativeAudioWrapper.GetSpeakerDevices(null, ref count);
			SpeakerDevice[] array = new SpeakerDevice[count];
			_nativeAudioWrapper.GetSpeakerDevices(array, ref count);
			int result = 0;
			SpeakerDevice[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				SpeakerDevice speakerDevice = array2[i];
				if (speakerDevice.IsDefault)
				{
					result = speakerDevice.DeviceIndex;
					break;
				}
			}
			return result;
		}

		/// <summary>
		/// Finds the microphone index that matches the sensor associated with this audio source.
		/// </summary>
		/// <returns>
		/// The index of the matching microphone array device, if a matching device was found.
		/// <code>null</code> otherwise.
		/// </returns>
		internal ushort? FindMicrophoneIndex()
		{
			IEnumerable<MicrophoneArrayDevice> enumerable = FindCaptureDevices();
			foreach (MicrophoneArrayDevice item in enumerable)
			{
				if (_nativeAudioWrapper.MatchAudioDeviceToSensor(item))
				{
					return (ushort)item.DeviceIndex;
				}
			}
			return null;
		}

		/// <summary>
		/// Stops audio capture, if necessary, and disposes all resources used by the
		/// <see cref="T:Microsoft.Kinect.KinectAudioSource" />.
		/// </summary>
		internal void Dispose()
		{
			lock (_initLock)
			{
				if (_sensor != null)
				{
					if (_stream != null)
					{
						Stop();
					}
					_BeamAngleChangedContextHandler.Dispose();
					_SoundSourceAngleChangedContextHandler.Dispose();
					_sensor = null;
				}
			}
		}
	}
}
