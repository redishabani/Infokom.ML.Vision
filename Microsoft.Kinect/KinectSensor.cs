using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// A Microsoft Kinect™ sensor.
	/// </summary>
	public sealed class KinectSensor : IDisposable
	{
		/// <summary>
		/// This class and its more strongly typed subclass is a very specific utility for handling the multiple
		/// frame types that KinectSensor managed, sorts and dispatches.
		/// </summary>
		private abstract class FrameStateHelper
		{
			/// <summary>
			/// Gets or sets the most recent timestamp for this frame type.
			/// </summary>
			public long MostRecentTimestamp { get; protected set; }

			/// <summary>
			/// Gets or sets the most recent framenumber for this frame type.
			/// </summary>
			public int MostRecentFramenumber { get; protected set; }

			/// <summary>
			/// Gets a value indicating whether this instance is ready for updated args.
			/// This can happen if the args are null or have already been dispatched, or if there are no type
			/// specific listeners.
			/// If this is false, we should not call SetArgs because we have data that needs to be dispatched.
			/// </summary>
			public bool IsReadyForSetArgs
			{
				get
				{
					if (this.HasArgs)
					{
						return this.HaveDispatchedEvent;
					}
					return true;
				}
			}

			/// <summary>
			/// Gets a value indicating whether there are currently args stored in this instance.
			/// </summary>
			public abstract bool HasArgs { get; }

			/// <summary>
			/// Gets or sets a value indicating whether the args currently held by this class have been dispatched.
			/// </summary>
			protected bool HaveDispatchedEvent { get; set; }

			/// <summary>
			/// Gets a value indicating whether the wrapped even handler has listeners.
			/// </summary>
			protected abstract bool HasListeners { get; }

			/// <summary>
			/// Set new args at the specified timestamp.  HaveDispatchedEvent will become false.
			/// </summary>
			/// <param name="args">The new args.  Must match the correct type for this instance.</param>
			/// <param name="timestamp">The timestamp of this args.</param>
			/// <param name="framenumber">The framenumber of this args.</param>
			public abstract void SetArgs(EventArgs args, long timestamp, int framenumber);

			/// <summary>
			/// Invoke the current args with the handler supplied at construction time, if there are
			/// listeners.
			/// HaveDispatchedEvent will become true after this call even if there weren't any listeners.
			/// </summary>
			/// <param name="sender">The sender of the event.</param>
			public abstract void Invoke(KinectSensor sender);

			/// <summary>
			/// Reset the instance to no args and no timestamp, returning the args (if present)
			/// for the caller to use if desired.
			/// </summary>
			/// <returns>The Args (if any) that were set in this instance.</returns>
			public abstract EventArgs ResetAndReturnArgs();

			/// <summary>
			/// Determine whether this FrameStateHelper is a candidate for invocation.
			/// This happens if:
			/// There are Args to dispatch and
			/// We have not already dispatched the event and
			/// The timestamp is earlier than the timestamp the caller provides.
			/// Note that this does *not* imply that HasListeners is true - just that this instance
			/// is the next candidate (based on timestamp and frame number).
			/// </summary>
			/// <param name="earliestTimestampSoFar">
			/// The earliest timestamp being considered by the caller.  
			/// The timestamp must be &lt; earliest, or == and framenumber &lt; earliestframenumber.
			/// </param>
			/// <param name="earliestFramenumberSoFar">
			/// The earliest framenumber being considered by the caller.  
			/// The timestamp must be &lt; earliest, or == and framenumber &lt; earliestframenumber.
			/// </param>
			/// <returns>Whether or not this FrameStateHelper is a candiate for invocation.</returns>
			public bool IsCandidateForInvoke(long earliestTimestampSoFar, int earliestFramenumberSoFar)
			{
				if (this.HasArgs && !this.HaveDispatchedEvent)
				{
					if (this.MostRecentTimestamp >= earliestTimestampSoFar)
					{
						if (this.MostRecentTimestamp == earliestTimestampSoFar)
						{
							return this.MostRecentFramenumber < earliestFramenumberSoFar;
						}
						return false;
					}
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// This is used to help the FrameStateHelper determine which stream it belongs to.
		/// </summary>
		internal enum FrameStateType
		{
			/// <summary>
			/// The color stream.
			/// </summary>
			Color,
			/// <summary>
			/// The depth stream.
			/// </summary>
			Depth,
			/// <summary>
			/// The skeleton stream.
			/// </summary>
			Skeleton
		}

		/// <summary>
		/// The concrete, strongly typed version of FrameStateHelper, typed to the Args type.
		/// </summary>
		/// <typeparam name="TArgs">The type of EventArgs handled by this instance.</typeparam>
		private class FrameStateHelper<TArgs> : FrameStateHelper where TArgs : EventArgs
		{
			/// <summary>
			/// Wrapped handler object.
			/// </summary>
			private readonly ContextEventHandler<TArgs> _handler;

			/// <summary>
			/// Used for logging to tell which stream this frame belongs to.
			/// </summary>
			private readonly FrameStateType _type;

			/// <summary>
			/// Arguments for the wrapped handler.
			/// </summary>
			private TArgs _args;

			/// <summary>
			/// Gets a value indicating whether there are currently args stored in this instance.
			/// </summary>
			public override bool HasArgs => this._args != null;

			/// <summary>
			/// Gets a value indicating whether the wrapped even handler has listeners.
			/// </summary>
			protected override bool HasListeners => this._handler.HasHandlers;

			/// <summary>
			/// Initializes a new instance of the FrameStateHelper class with a wrapped handler.
			/// </summary>
			/// <param name="type">The frame type this helper is associated with.</param>
			/// <param name="contextHandler">The wrapped handler.</param>
			public FrameStateHelper(FrameStateType type, ContextEventHandler<TArgs> contextHandler)
			{
				this._type = type;
				this._handler = contextHandler;
			}

			/// <summary>
			/// Set new args at the specified timestamp.  HaveDispatchedEvent will become false.
			/// </summary>
			/// <param name="args">The new args.  Must match the correct type for this instance.</param>
			/// <param name="timestamp">The timestamp of this args.</param>
			/// <param name="framenumber">The framenumber of this args.</param>
			public override void SetArgs(EventArgs args, long timestamp, int framenumber)
			{
				this._args = (TArgs)args;
				base.MostRecentTimestamp = timestamp;
				base.MostRecentFramenumber = framenumber;
				base.HaveDispatchedEvent = false;
			}

			/// <summary>
			/// Invoke the current args with the handler supplied at construction time, if there are
			/// listeners.
			/// HaveDispatchedEvent will become true after this call even if there weren't any listeners.
			/// </summary>
			/// <param name="sender">The sender of the event.</param>
			public override void Invoke(KinectSensor sender)
			{
				switch (this._type)
				{
				case FrameStateType.Color:
						//TO DO: 
						//KinectEtwProvider.EventWriteColorFrameReady(base.MostRecentTimestamp);
						break;
				case FrameStateType.Depth:
						//TO DO: 
						//KinectEtwProvider.EventWriteDepthFrameReady(base.MostRecentTimestamp);
						break;
				case FrameStateType.Skeleton:
						//TO DO: 
						//KinectEtwProvider.EventWriteSkeletonFrameReady(base.MostRecentTimestamp);
						break;
				}
				if (this.HasListeners)
				{
					this._handler.Invoke(sender, this._args);
				}
				base.HaveDispatchedEvent = true;
			}

			/// <summary>
			/// Reset the instance to no args and no timestamp, returning the args (if present)
			/// for the caller to use if desired.
			/// </summary>
			/// <returns>The Args (if any) that were set in this instance.</returns>
			public override EventArgs ResetAndReturnArgs()
			{
				TArgs args = this._args;
				this._args = null;
				base.HaveDispatchedEvent = false;
				base.MostRecentTimestamp = 0L;
				return args;
			}
		}

		/// <summary>
		/// Options for initializing an INuiSensor.  Must be in sync with NuiApi.h.
		/// </summary>
		[Flags]
		private enum SensorOptions
		{
			/// <summary>
			/// No options chosen.
			/// </summary>
			None = 0,
			/// <summary>
			/// Both depth and player index desired.  Note that this uses the skeletal engine.
			/// </summary>
			UseDepthAndPlayerIndex = 1,
			/// <summary>
			/// Color frames desired.
			/// </summary>
			UseColor = 2,
			/// <summary>
			/// Skeletal frames desired.
			/// </summary>
			UseSkeletalTracking = 8,
			/// <summary>
			/// Depth frames desired.
			/// </summary>
			UseDepth = 0x20,
			/// <summary>
			/// Audio capture (Microphone array) desired.
			/// </summary>
			UseAudio = 0x10000000
		}

		/// <summary>
		/// Index of color FrameStateHelper in the _frameHelpers array.
		/// </summary>
		private const int ColorHelperIndex = 0;

		/// <summary>
		/// Index of depth FrameStateHelper in the _frameHelpers array.
		/// </summary>
		private const int DepthHelperIndex = 1;

		/// <summary>
		/// Index of skeletal FrameStateHelper in the _frameHelpers array.
		/// </summary>
		private const int SkeletonHelperIndex = 2;

		/// <summary>
		/// This is an pointer to the internal debug stack that is used for diagnosing
		/// failures within the Kinect runtime.
		/// </summary>
		private static ulong failureStackPointer = NativeMethods.NuiDebugGetFailureStack();

		/// <summary>
		/// We would like to use the _initLock for multi-reader/single writer access to this class.
		/// Unfortunately, ReaderWriterLockSlim must itself be Disposed, meaning that we need a thread-safe
		/// means to detect Disposal.  So, we use a normal lock to check the IsDisposed state and acquire
		/// the _initLock, which allows us to spend most of the time under the R/W lock rather than the full lock.
		/// To ensure safe use of these locks, _initLock must always be acquired under a held _disposeLock, and 
		/// _disposeLock must never be acquired under _initLock.
		/// </summary>
		private readonly ReaderWriterLockSlim _initLock = new ReaderWriterLockSlim();

		/// <summary>
		/// Lock for disposing the sensor.
		/// </summary>
		private readonly object _disposeLock = new object();

		/// <summary>
		/// The KinectAudioSource object.
		/// </summary>
		private KinectAudioSource _audioSource;

		/// <summary>
		/// True if the unsupported message has been written to the debug stream.
		/// </summary>
		private static bool _unsupportedMessageShown = false;

		/// <summary>
		/// The last status found for this sensor.
		/// </summary>
		private KinectStatus _lastStatus = KinectStatus.NotReady;

		/// <summary>
		/// Event that fires when a new depth frame is available from this Kinect sensor.
		/// </summary>
		private readonly ContextEventHandler<DepthImageFrameReadyEventArgs> _depthFrameReadyContextHandler;

		/// <summary>
		/// Event that fires when a new color frame is available from this Kinect sensor.
		/// </summary>
		private readonly ContextEventHandler<ColorImageFrameReadyEventArgs> _colorFrameReadyContextHandler;

		/// <summary>
		/// Event that fires when a new skeleton frame is available from this Kinect sensor.
		/// </summary>
		private readonly ContextEventHandler<SkeletonFrameReadyEventArgs> _skeletonFrameReadyContextHandler;

		/// <summary>
		/// Event that fires when new frames are available from each of this Kinect sensor's active streams.
		/// </summary>
		private readonly ContextEventHandler<AllFramesReadyEventArgs> _allFramesReadyContextHandler;

		/// <summary>
		/// Privately cached reference to the associated CoordinateMapper object for the sensor.
		/// </summary>
		private CoordinateMapper _coordinateMapper;

		/// <summary>
		/// Event indiating that the thread should exit.
		/// NOTE: moved from constructor to inside Initialize so that we can reuse this object after unInitialize.
		/// </summary>
		private ManualResetEvent _hKillThread;

		/// <summary>
		/// Array of FrameStateHelpers for all frames.
		/// </summary>
		private readonly FrameStateHelper[] _frameHelpers;

		/// <summary>
		/// Options desired for the sensor.
		/// </summary>
		private SensorOptions _requestedOptions;

		/// <summary>
		/// Options requested from the sensor.
		/// </summary>
		private SensorOptions _currentOptions;

		/// <summary>
		/// The frame processing background thread.
		/// </summary>
		private Thread _thread;

		/// <summary>
		/// Gets the index of the nui device.  Do not expose this to the end user.
		/// NOTE: Clients of this method expect it to be non-locking.
		/// </summary>
		internal int InstanceIndex
		{
			get
			{
				if (this.NuiSensor != null)
				{
					return this.NuiSensor.NuiInstanceIndex();
				}
				return int.MinValue;
			}
		}

		/// <summary>
		/// Gets the collection of all Kinect™ sensors.
		/// </summary>
		public static KinectSensorCollection KinectSensors => KinectSensorCollection.Instance;

		/// <summary>
		/// Gets the internal wrapper for INuiSensor.
		/// </summary>
		internal NuiSensor NuiSensor { get; private set; }

		/// <summary>
		/// Gets the unique device connection name for the sensor.  This is the camera device instance id.
		/// </summary>
		public string UniqueKinectId
		{
			get
			{
				using (this.EnterReadLock())
				{
					return this.GetUniqueKinectId();
				}
			}
		}

		/// <summary>
		/// Gets the USB HUB device instance id.
		/// </summary>
		public string DeviceConnectionId
		{
			get
			{
				using (this.EnterReadLock())
				{
					return this.GetDeviceConnectionId();
				}
			}
		}

		/// <summary>
		/// Gets the reported status of the sensor.
		/// </summary>
		public KinectStatus Status
		{
			get
			{
				using (this.EnterReadLock())
				{
					return this.LastStatus = this.GetStatus();
				}
			}
		}

		/// <summary>
		/// Gets or sets the last status for this sensor.
		/// </summary>
		internal KinectStatus LastStatus
		{
			get => this._lastStatus;
			set
			{
				if (value == KinectStatus.Connected && this._requestedOptions == SensorOptions.None)
				{
					this.CreateStreamObjects();
				}
				this._lastStatus = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the sensor was running.
		/// </summary>
		internal bool WasRunning { get; set; }

		/// <summary>
		/// Gets a value indicating whether the depth frame handler has subscriptions.
		/// </summary>
		internal bool HasDepthInvocations => this._depthFrameReadyContextHandler.HasHandlers;

		/// <summary>
		/// Gets a value indicating whether the color frame handler has subscriptions.
		/// </summary>
		internal bool HasColorInvocations => this._colorFrameReadyContextHandler.HasHandlers;

		/// <summary>
		/// Gets a value indicating whether the skeleton frame handler has subscriptions.
		/// </summary>
		internal bool HasSkeletonInvocations => this._skeletonFrameReadyContextHandler.HasHandlers;

		/// <summary>
		/// Gets a value indicating whether the AllFrames handler has subscriptions.
		/// </summary>
		internal bool HasAllFramesInvocations => this._allFramesReadyContextHandler.HasHandlers;

		/// <summary>
		/// Gets the CoordinateMapper object for the sensor.
		/// </summary>
		public CoordinateMapper CoordinateMapper => this._coordinateMapper ?? (this._coordinateMapper = new CoordinateMapper(this));

		/// <summary>
		/// Gets the depth stream for the sensor.
		/// </summary>
		public DepthImageStream DepthStream { get; private set; }

		/// <summary>
		/// Gets the color stream for the sensor.
		/// </summary>
		public ColorImageStream ColorStream { get; private set; }

		/// <summary>
		/// Gets the skeleton stream for the sensor.
		/// </summary>
		public SkeletonStream SkeletonStream { get; private set; }

		/// <summary>
		/// Gets the audio source for the sensor.
		/// </summary>
		public KinectAudioSource AudioSource
		{
			get
			{
				using (this.EnterReadLock())
				{
					if (this._audioSource == null)
					{
						if (this.LastStatus == KinectStatus.Connected)
						{
							this._audioSource = new KinectAudioSource(this);
						}
						else
						{
							this._audioSource = null;
						}
					}
					return this._audioSource;
				}
			}
		}

		/// <summary>
		/// Gets the maximum camera elevation angle in degrees.
		/// </summary>
		public int MaxElevationAngle => 27;

		/// <summary>
		/// Gets the minimum camera elevation angle in degrees.
		/// </summary>
		public int MinElevationAngle => -27;

		/// <summary>
		/// Gets or sets the desired camera elevation angle. See the documentation about 
		/// the limits of this property.
		/// </summary>
		public int ElevationAngle
		{
			get
			{
				using (this.EnterReadLock())
				{
					this.CheckNotRunning();
					return this.NuiSensor.NuiCameraElevationGetAngle();
				}
			}
			set
			{
				using (this.EnterReadLock())
				{
					this.CheckNotRunning();
					if (value < this.MinElevationAngle || value > this.MaxElevationAngle)
					{
						throw new ArgumentOutOfRangeException(Resources.ElevationIncorrect);
					}
					this.NuiSensor.NuiCameraElevationSetAngle(value);
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the infrared emitter is disabled.
		/// Default value of false.
		/// </summary>
		public bool ForceInfraredEmitterOff
		{
			get => this.NuiSensor.ForceInfraredEmitterOff; set => this.NuiSensor.ForceInfraredEmitterOff = value;
		}

		/// <summary>
		/// Gets or sets a filter to be applied to each depth frame.
		/// </summary>
		public IDepthFilter DepthFilter
		{
			get => this.NuiSensor.DepthFilter; set => this.NuiSensor.DepthFilter = value;
		}

		/// <summary>
		/// Gets a value indicating whether the Kinect is currently streaming data. To set it, call Start() or Stop().
		/// </summary>
		public bool IsRunning
		{
			get
			{
				using (this.EnterReadLock())
				{
					return this.IsRunningInternal;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the Kinect is currently streaming data. 
		/// Non-locking version of IsRunning.
		/// </summary>
		private bool IsRunningInternal => this._currentOptions != SensorOptions.None;

		/// <summary>
		/// Gets a value indicating whether this KinectSensor has been disposed.
		/// </summary>
		internal bool IsDisposed { get; private set; }

		/// <summary>
		/// Event that fires when a new depth frame is available from this Kinect sensor.
		/// </summary>
		public event EventHandler<DepthImageFrameReadyEventArgs> DepthFrameReady
		{
			add =>
			  //TO DO: 
			  //KinectEtwProvider.EventWriteManagedFrameReadyEventAddedInfo(1);
			  this._depthFrameReadyContextHandler.AddHandler(value); remove =>
															  //TO DO: 
															  //KinectEtwProvider.EventWriteManagedFrameReadyEventRemovedInfo(1);
															  this._depthFrameReadyContextHandler.RemoveHandler(value);
		}

		/// <summary>
		/// Event that fires when a new color frame is available from this Kinect sensor.
		/// </summary>
		public event EventHandler<ColorImageFrameReadyEventArgs> ColorFrameReady
		{
			add =>
			  //KinectEtwProvider.EventWriteManagedFrameReadyEventAddedInfo(0);
			  this._colorFrameReadyContextHandler.AddHandler(value); remove =>
															  //KinectEtwProvider.EventWriteManagedFrameReadyEventRemovedInfo(0);
															  this._colorFrameReadyContextHandler.RemoveHandler(value);
		}

		/// <summary>
		/// Event that fires when a new skeleton frame is available from this Kinect sensor.
		/// </summary>
		public event EventHandler<SkeletonFrameReadyEventArgs> SkeletonFrameReady
		{
			add =>
			  //TO DO: 
			  //KinectEtwProvider.EventWriteManagedFrameReadyEventAddedInfo(2);
			  this._skeletonFrameReadyContextHandler.AddHandler(value); remove =>
																//TO DO: 
																//KinectEtwProvider.EventWriteManagedFrameReadyEventRemovedInfo(2);
																this._skeletonFrameReadyContextHandler.RemoveHandler(value);
		}

		/// <summary>
		/// Event that fires when new frames are available from each of this Kinect sensor's active streams.
		/// </summary>
		public event EventHandler<AllFramesReadyEventArgs> AllFramesReady
		{
			add =>
			  //TO DO: 
			  //KinectEtwProvider.EventWriteManagedAllFramesReadyEventAddedInfo();
			  this._allFramesReadyContextHandler.AddHandler(value); remove =>
															 //TO DO: 
															 //KinectEtwProvider.EventWriteManagedAllFramesReadyEventRemovedInfo();
															 this._allFramesReadyContextHandler.RemoveHandler(value);
		}

		/// <summary>
		/// Initializes a new instance of the KinectSensor class with the given INuiSensor. 
		/// Note that there should be ONLY one KinectSensor object per INuiSensor.
		/// </summary>
		/// <param name="nuiSensor">INuiSensor to wrap.</param>
		internal KinectSensor(INuiSensor nuiSensor)
		{
			this._allFramesReadyContextHandler = new ContextEventHandler<AllFramesReadyEventArgs>();
			this._depthFrameReadyContextHandler = new ContextEventHandler<DepthImageFrameReadyEventArgs>();
			this._colorFrameReadyContextHandler = new ContextEventHandler<ColorImageFrameReadyEventArgs>();
			this._skeletonFrameReadyContextHandler = new ContextEventHandler<SkeletonFrameReadyEventArgs>();
			this._frameHelpers = new FrameStateHelper[3]
			{
				new FrameStateHelper<ColorImageFrameReadyEventArgs>(FrameStateType.Color, this._colorFrameReadyContextHandler),
				new FrameStateHelper<DepthImageFrameReadyEventArgs>(FrameStateType.Depth, this._depthFrameReadyContextHandler),
				new FrameStateHelper<SkeletonFrameReadyEventArgs>(FrameStateType.Skeleton, this._skeletonFrameReadyContextHandler)
			};
			this.NuiSensor = new NuiSensor(nuiSensor);
			this._requestedOptions = SensorOptions.None;
			this.LastStatus = this.GetStatus();
			if (!_unsupportedMessageShown && !this.NuiSensor.IsSupported)
			{
				if (this.LastStatus == KinectStatus.DeviceNotSupported)
				{
					Debugger.Log(0, "Warning", Resources.KinectNotSupportedNonDeveloper);
				}
				else
				{
					Debugger.Log(0, "Warning", Resources.KinectNotSupportedDeveloper);
				}
				_unsupportedMessageShown = true;
			}
		}

		/// <summary>
		/// Replace the NuiSensor with another one.  Used when a user plugs a sensor into a
		/// different USB port.
		/// </summary>
		/// <param name="nuiSensor">New nuiSensor to wrap.</param>
		internal void UpdateNuiSensor(INuiSensor nuiSensor)
		{
			this.NuiSensor = new NuiSensor(nuiSensor);
		}

		/// <summary>
		///  Creates the stream objects for this sensor.
		/// </summary>
		private void CreateStreamObjects()
		{
			this.DepthStream = new DepthImageStream(this);
			this.ColorStream = new ColorImageStream(this);
			this.SkeletonStream = new SkeletonStream(this);
			this._requestedOptions = SensorOptions.UseColor | SensorOptions.UseDepth;
		}

		/// <summary>
		/// Non-locking version of UniqueKinectId.
		/// </summary>
		/// <returns>The camera device instance id.</returns>
		internal string GetUniqueKinectId()
		{
			if (this.LastStatus != KinectStatus.Connected)
			{
				return null;
			}
			string text = this.NuiSensor.NuiUniqueId();
			if (string.IsNullOrEmpty(text))
			{
				throw new InvalidOperationException(Resources.FailedToGetDeviceName);
			}
			return text;
		}

		/// <summary>
		/// Provides the USB hub device instance id.  Non-locking.
		/// </summary>
		/// <returns>USB hub device instance id.</returns>
		internal string GetDeviceConnectionId()
		{
			return this.NuiSensor.NuiDeviceConnectionId();
		}

		/// <summary>
		/// Non-locking version of Status property.
		/// </summary>
		/// <returns>The status of the sensor.</returns>
		internal KinectStatus GetStatus()
		{
			return MapStatus(this.NuiSensor.NuiStatus());
		}

		/// <summary>
		/// Maps the NuiStatus HRESULT to a KinectStatus.
		/// </summary>
		/// <param name="code">The HRESULT to map.</param>
		/// <returns>Mapped KinectStatus.</returns>
		internal static KinectStatus MapStatus(uint code)
		{
			return code switch
			{
				0u => KinectStatus.Connected, 
				50397185u => KinectStatus.Initializing, 
				2147483648u => KinectStatus.Error, 
				2197880852u => KinectStatus.Disconnected, 
				2197881471u => KinectStatus.NotPowered, 
				2197880853u => KinectStatus.NotReady, 
				2197880838u => KinectStatus.DeviceNotGenuine, 
				2197880839u => KinectStatus.InsufficientBandwidth, 
				2197880840u => KinectStatus.DeviceNotSupported, 
				_ => KinectStatus.Error, 
			};
		}

		/// <summary>
		/// Tests whether the DepthImagePixel has a known value.
		/// </summary>
		/// <param name="depthImagePixel">The DepthImagePixel to test.</param>
		/// <returns>Returns true if the DepthImagePixel has a known value, false otherwise.</returns>
		public static bool IsKnownPoint(DepthImagePixel depthImagePixel)
		{
			return depthImagePixel.Depth != 0;
		}

		/// <summary>
		/// Tests whether the ColorImagePoint has a known value.
		/// </summary>
		/// <param name="colorImagePoint">The ColorImagePoint to test.</param>
		/// <returns>Returns true if the ColorImagePoint has a known value, false otherwise.</returns>
		public static bool IsKnownPoint(ColorImagePoint colorImagePoint)
		{
			if (colorImagePoint.X == int.MinValue)
			{
				return colorImagePoint.Y != int.MinValue;
			}
			return true;
		}

		/// <summary>
		/// Tests whether the DepthImagePoint has a known value.
		/// </summary>
		/// <param name="depthImagePoint">The DepthImagePoint to test.</param>
		/// <returns>Rreturns false if depth value == 0.</returns>
		public static bool IsKnownPoint(DepthImagePoint depthImagePoint)
		{
			return depthImagePoint.Depth != 0;
		}

		/// <summary>
		/// Tests whether the SkeletonPoint has a known value.
		/// </summary>
		/// <param name="skeletonPoint">The SkeletonPoint to test.</param>
		/// <returns>Returns true if the Skeleton point has a known value, false otherwise.</returns>
		public static bool IsKnownPoint(SkeletonPoint skeletonPoint)
		{
			return skeletonPoint.Z > 0f;
		}

		/// <summary>
		/// Gets the current accelerometer reading of the Kinect sensor.
		/// </summary>
		/// <returns>
		/// A Vector4 pointing in the directon of gravity.
		/// </returns>
		/// <remarks>
		/// <para>The accelerometer reading is returned as a 3d vector pointing in the direction of 
		/// gravity (i.e. floor (gravity) on a non-accelerating sensor).  The unit of the vector
		/// is in gravity units (g), 9.81m/s^2.  The coordinate system is centered on the sensor, 
		/// right-handed coordinate system with positive-Z in the direction the sensor is pointing at.  
		/// In the default sensor rotation (horizontal, level placement), this will return the vector 
		/// (0, -1.0, 0, 0).  The w value of the Vector4 is always set to 0.0.</para>
		/// </remarks>        
		public Vector4 AccelerometerGetCurrentReading()
		{
			using (this.EnterReadLock())
			{
				return this.NuiSensor.NuiAccelerometerGetCurrentReading();
			}
		}

		/// <summary>
		/// Maps from a depth coordinate with depth value to color coordinates.
		/// </summary>
		/// <param name="depthImageFormat">The depth format of the source.</param>
		/// <param name="depthX">The X coordinate of the depth frame.</param>
		/// <param name="depthY">The Y coordinate of the depth frame.</param>
		/// <param name="depthPixelValue">The value from the depth frame's pixel data at the given coordinates (depthX, depthY).</param>
		/// <param name="colorImageFormat">The desired target image format.</param>
		/// <returns>The ColorImagePoint corresponding to the point in DepthImage space.</returns>
		[Obsolete("This method is replaced by Microsoft.Kinect.CoordinateMapper.MapDepthPointToColorPoint", false)]
		public ColorImagePoint MapDepthToColorImagePoint(DepthImageFormat depthImageFormat, int depthX, int depthY, short depthPixelValue, ColorImageFormat colorImageFormat)
		{
			if (depthImageFormat == DepthImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "depthImageFormat");
			}
			if (colorImageFormat == ColorImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "colorImageFormat");
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteMapDepthToColorImagePointStart();
			_NUI_IMAGE_VIEW_AREA pcViewArea = new _NUI_IMAGE_VIEW_AREA
			{
				eDigitalZoom = 0,
				lCenterX = 0,
				lCenterY = 0
			};
			this.NuiSensor.NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolution((_NUI_IMAGE_RESOLUTION)ColorImageStream.LookUpImageResolution(colorImageFormat), (_NUI_IMAGE_RESOLUTION)DepthImageStream.LookUpImageResolution(depthImageFormat), ref pcViewArea, depthX, depthY, (ushort)depthPixelValue, out var plColorX, out var plColorY);
			//TO DO: 
			//KinectEtwProvider.EventWriteMapDepthToColorImagePointEnd();
			return new ColorImagePoint
			{
				X = plColorX,
				Y = plColorY
			};
		}

		/// <summary>
		/// Maps every point in a depth frame to the corresponding location in a ColorImageFormat coordinate space.
		/// </summary>
		/// <param name="depthImageFormat">The depth format of the source.</param>
		/// <param name="depthPixelData">
		/// The depth frame pixel data, as retrieved from DepthImageFrame.CopyPixelDataTo.
		/// Must be equal in length to Width*Height of the depth format specified by depthImageFormat.
		/// </param>
		/// <param name="colorImageFormat">The desired target image format.</param>
		/// <param name="colorCoordinates">
		/// The ColorImagePoint array to receive the data.  Each element will be be the result of mapping the 
		/// corresponding depthPixelDatum to the specified ColorImageFormat coordinate space.
		/// Must be equal in length to depthPixelData.
		/// </param>
		[Obsolete("This method is replaced by Microsoft.Kinect.CoordinateMapper.MapDepthFrameToColorFrame", false)]
		public void MapDepthFrameToColorFrame(DepthImageFormat depthImageFormat, short[] depthPixelData, ColorImageFormat colorImageFormat, ColorImagePoint[] colorCoordinates)
		{
			if (depthImageFormat == DepthImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "depthImageFormat");
			}
			if (depthPixelData == null)
			{
				throw new ArgumentNullException("depthPixelData");
			}
			if (depthPixelData.Length != DepthImageStream.LookUpPixelDataLength(depthImageFormat))
			{
				throw new ArgumentException(Resources.PixelBufferIncorrectLength, "depthPixelData");
			}
			if (colorImageFormat == ColorImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "colorImageFormat");
			}
			if (colorCoordinates == null)
			{
				throw new ArgumentNullException("colorCoordinates");
			}
			if (colorCoordinates.Length != depthPixelData.Length)
			{
				throw new ArgumentException(Resources.PixelBufferIncorrectLength, "colorCoordinates");
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteMapDepthFrameToColorFrameStart();
			this.NuiSensor.NuiImageGetColorPixelCoordinateFrameFromDepthPixelFrameAtResolution((_NUI_IMAGE_RESOLUTION)ColorImageStream.LookUpImageResolution(colorImageFormat), (_NUI_IMAGE_RESOLUTION)DepthImageStream.LookUpImageResolution(depthImageFormat), depthPixelData, colorCoordinates);
			//TO DO: 
			//KinectEtwProvider.EventWriteMapDepthFrameToColorFrameEnd();
		}

		/// <summary>
		/// Looks up the color frame coordinates for a given skeleton point.
		/// </summary>
		/// <param name="skeletonPoint">The supplied skeleton point.</param>
		/// <param name="colorImageFormat">The color format to convert to.</param>
		/// <returns>The ColorImagePoint corresponding to the given skeleton point.</returns>
		[Obsolete("This method is replaced by Microsoft.Kinect.CoordinateMapper.MapSkeletonPointToColorPoint", false)]
		public ColorImagePoint MapSkeletonPointToColor(SkeletonPoint skeletonPoint, ColorImageFormat colorImageFormat)
		{
			if (colorImageFormat == ColorImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "colorImageFormat");
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteMapSkeletonPointToColorStart();
			_Vector4 vPoint = default(_Vector4);
			vPoint.x = skeletonPoint.X;
			vPoint.y = skeletonPoint.Y;
			vPoint.z = skeletonPoint.Z;
			vPoint.w = 1f;
			ManagedNuiTransformSkeletonToDepthImage(vPoint, out var pfDepthX, out var pfDepthY, out var pfDepthValue);
			ColorImagePoint result = this.MapDepthToColorImagePoint(DepthImageFormat.Resolution640x480Fps30, (int)(pfDepthX * 640f), (int)(pfDepthY * 480f), (short)((int)pfDepthValue << 3), colorImageFormat);
			//TO DO: 
			//KinectEtwProvider.EventWriteMapSkeletonPointToColorEnd();
			return result;
		}

		/// <summary>
		/// Looks up the depth frame coordinates for a given skeleton point.
		/// </summary>
		/// <param name="skeletonPoint">The supplied skeleton point.</param>
		/// <param name="depthImageFormat">The depth format to convert to.</param>
		/// <returns>The DepthImagePoint that contains the X, Y and depth value of the given skeleton point.</returns>
		[Obsolete("This method is replaced by Microsoft.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint", false)]
		public DepthImagePoint MapSkeletonPointToDepth(SkeletonPoint skeletonPoint, DepthImageFormat depthImageFormat)
		{
			if (depthImageFormat == DepthImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "depthImageFormat");
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteMapSkeletonPointToDepthStart();
			_Vector4 vPoint = default(_Vector4);
			vPoint.x = skeletonPoint.X;
			vPoint.y = skeletonPoint.Y;
			vPoint.z = skeletonPoint.Z;
			vPoint.w = 1f;
			ManagedNuiTransformSkeletonToDepthImage(vPoint, out var pfDepthX, out var pfDepthY, out var pfDepthValue);
			ImageResolution resolution = DepthImageStream.LookUpImageResolution(depthImageFormat);
			ImageStream.ResolutionToHeightWidth(resolution, out var width, out var height);
			DepthImagePoint result = new DepthImagePoint
			{
				X = (int)(pfDepthX * (float)width + 0.5f),
				Y = (int)(pfDepthY * (float)height + 0.5f),
				Depth = (int)(pfDepthValue + 0.5f)
			};
			//TO DO: 
			//KinectEtwProvider.EventWriteMapSkeletonPointToDepthEnd();
			return result;
		}

		/// <summary>
		/// Looks up the skeleton point location of the given depth X, Y.
		/// </summary>
		/// <param name="depthImageFormat">The format to use in the conversion.</param>
		/// <param name="depthX">The X coordinate of the depth frame.</param>
		/// <param name="depthY">The Y coordinate of the depth frame.</param>
		/// <param name="depthPixelValue">The value from the depth frame's pixel data at the given coordinates (depthX, depthY).</param>
		/// <returns>The skeleton point for the given X, Y, depth and format.</returns>
		[Obsolete("This method is replaced by Microsoft.Kinect.CoordinateMapper.MapDepthPointToSkeletonPoint", false)]
		public SkeletonPoint MapDepthToSkeletonPoint(DepthImageFormat depthImageFormat, int depthX, int depthY, short depthPixelValue)
		{
			if (depthImageFormat == DepthImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "depthImageFormat");
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteMapDepthToSkeletonPointStart();
			ImageResolution resolution = DepthImageStream.LookUpImageResolution(depthImageFormat);
			ImageStream.ResolutionToHeightWidth(resolution, out var width, out var height);
			_Vector4 vector = ManagedNuiTransformDepthImageToSkeleton((float)depthX / (float)width, (float)depthY / (float)height, depthPixelValue);
			SkeletonPoint result = new SkeletonPoint
			{
				X = vector.x,
				Y = vector.y,
				Z = vector.z
			};
			//TO DO: 
			//KinectEtwProvider.EventWriteMapDepthToSkeletonPointEnd();
			return result;
		}

		/// <summary>
		/// Based off the function NuiTransformDepthImageToSkeleton from NuiSkeleton.h, which is inline
		/// so it is neither directly possible nor is it performant to p-invoke it from managed.
		/// </summary>
		/// <param name="fDepthX">X coordinate from the depth image.</param>
		/// <param name="fDepthY">Y coordinate from the depth image.</param>
		/// <param name="depthPixelDatum">Depth value from the depth image.</param>
		/// <returns>A Vector indicating the point on the skeleton corresponding to the depth pixel.</returns>
		private static _Vector4 ManagedNuiTransformDepthImageToSkeleton(float fDepthX, float fDepthY, short depthPixelDatum)
		{
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedNuiTransformDepthImageToSkeletonStart();
			float num = (float)(depthPixelDatum >> 3) / 1000f;
			float x = (fDepthX - 0.5f) * (0.003501f * num) * 320f;
			float y = (0.5f - fDepthY) * (0.003501f * num) * 240f;
			_Vector4 result = default(_Vector4);
			result.x = x;
			result.y = y;
			result.z = num;
			result.w = 1f;
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedNuiTransformDepthImageToSkeletonEnd();
			return result;
		}

		/// <summary>
		/// Based off the function NuiTransformSkeletonToDepthImage from NuiSkeleton.h, which is inline
		/// so it is neither directly possible nor is it performant to p-invoke it from managed.
		/// </summary>
		/// <param name="vPoint">Vector from skeleton to transform.</param>
		/// <param name="pfDepthX">Resulting X coordinate for skeleton point.</param>
		/// <param name="pfDepthY">Resulting Y coordinate for skeleton point.</param>
		/// <param name="pfDepthValue">Resulting depth value for skeleton point.</param>
		internal static void ManagedNuiTransformSkeletonToDepthImage(_Vector4 vPoint, out float pfDepthX, out float pfDepthY, out float pfDepthValue)
		{
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedNuiTransformSkeletonToDepthImageStart();
			if (vPoint.z > float.Epsilon)
			{
				pfDepthX = 0.5f + vPoint.x * 285.63f / (vPoint.z * 320f);
				pfDepthY = 0.5f - vPoint.y * 285.63f / (vPoint.z * 240f);
				pfDepthValue = vPoint.z * 1000f;
			}
			else
			{
				pfDepthX = 0f;
				pfDepthY = 0f;
				pfDepthValue = 0f;
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedNuiTransformSkeletonToDepthImageEnd();
		}

		/// <summary>
		/// Start streaming data from this Kinect. This needs to be called whether polling or registering for the events. It also needs to be called to get control of the Camera.
		/// This API may throw System.IO.IOException if the KinectSensor is already in use by another process.
		/// </summary>
		public void Start()
		{
			using AutoLock autoLock = this.EnterUpgradeableReadLock();
			if (this.LastStatus != KinectStatus.Connected)
			{
				throw new InvalidOperationException(Resources.KinectNotReady);
			}
			if (!this.IsRunningInternal)
			{
				using (autoLock.UpgradeToWrite())
				{
					this.Initialize(this._requestedOptions);
				}
			}
			//KinectEtwProvider.EventWriteManagedKinectSensorStartInfo(GetDeviceConnectionId());
		}

		/// <summary>
		/// Stop streaming data from this Kinect.
		/// </summary>
		public void Stop()
		{
			using AutoLock autoLock = this.EnterUpgradeableReadLock();
			if (!this.IsRunningInternal)
			{
				return;
			}
			using (autoLock.UpgradeToWrite())
			{
				this.Uninitialize();
			}
		}

		/// <summary>
		/// This method is used by the SkeletonStream to inform the sensor that it wants the skeleton engine running.
		/// This will re-call Initialize to fire up or shut down the skeleton engine if needed.
		/// </summary>
		/// <param name="enable">Indicates if the skeletal engine is should be turned on or off.</param>
		internal void RequestSkeletonEngine(bool enable)
		{
			using AutoLock autoLock = this.EnterUpgradeableReadLock();
			if (enable)
			{
				this._requestedOptions = SensorOptions.UseDepthAndPlayerIndex | SensorOptions.UseColor | SensorOptions.UseSkeletalTracking | SensorOptions.UseDepth;
			}
			else
			{
				this._requestedOptions = SensorOptions.UseColor | SensorOptions.UseDepth;
			}
			if (this.LastStatus == KinectStatus.Connected && this.IsRunningInternal && this._currentOptions != (this._requestedOptions | SensorOptions.UseAudio))
			{
				using (autoLock.UpgradeToWrite())
				{
					this.Uninitialize();
					this.Initialize(this._requestedOptions);
					return;
				}
			}
		}

		/// <summary>
		/// Calls NuiInitialize with the desired options.
		/// <remarks>
		/// Must be called under a _initLock in Write mode.
		/// Must not be called if IsRunningInternal is true.
		/// </remarks>
		/// </summary>
		/// <param name="options">The desired optoins.</param>
		private void Initialize(SensorOptions options)
		{
			//KinectEtwProvider.EventWriteKinectSensorInitializeStart();
			options |= SensorOptions.UseAudio;
			this.NuiSensor.NuiInitialize((uint)options);
			this._currentOptions = options;
			try
			{
				this.ColorStream.Start();
				this.DepthStream.Start();
				this.SkeletonStream.Start();
			}
			catch (InvalidOperationException)
			{
				this.Uninitialize();
				throw;
			}
			this._hKillThread = new ManualResetEvent(initialState: false);
			this._hKillThread.Reset();
			this._thread = new Thread(this.MainBackgroundThreadProc);
			this._thread.Name = "Background M-NUI Thread";
			this._thread.IsBackground = true;
			this._thread.Start(this._hKillThread);
			//KinectEtwProvider.EventWriteKinectSensorInitializeEnd();
		}

		/// <summary>
		/// Must be called under a _initLock in Write mode.
		/// Must not be called if IsRunningInternal is false.
		/// </summary>
		private void Uninitialize()
		{
			this.ColorStream.Stop();
			this.DepthStream.Stop();
			this.SkeletonStream.Stop();
			if (this._audioSource != null)
			{
				this._audioSource.Stop();
			}
			if (this._hKillThread != null)
			{
				this._hKillThread.Set();
				this._hKillThread = null;
			}
			if (this._thread != null)
			{
				this._thread.Join();
				this._thread = null;
			}
			this.NuiSensor.NuiShutdown();
			this._currentOptions = SensorOptions.None;
		}

		/// <summary>
		/// Frame processing thread procedure.
		/// </summary>
		/// <param name="data">ManualResetEvent tripped when the thread should exit.</param>
		private void MainBackgroundThreadProc(object data)
		{
			using ManualResetEvent item = (ManualResetEvent)data;
			List<WaitHandle> list = new List<WaitHandle>(4);
			list.Add(item);
			List<WaitHandle> list2 = list;
			while (true)
			{
				if (list2.Count > 1)
				{
					list2.RemoveRange(1, list2.Count - 1);
				}
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				if (this.ColorStream != null && this.ColorStream.IsEnabled && (this.HasColorInvocations || this.HasAllFramesInvocations))
				{
					flag = true;
					list2.Add(this.ColorStream.NextFrameEvent);
				}
				if (this.DepthStream != null && this.DepthStream.IsEnabled && (this.HasDepthInvocations || this.HasAllFramesInvocations))
				{
					flag2 = true;
					list2.Add(this.DepthStream.NextFrameEvent);
				}
				if (this.SkeletonStream != null && this.SkeletonStream.IsEnabled && (this.HasSkeletonInvocations || this.HasAllFramesInvocations))
				{
					flag3 = true;
					list2.Add(this.SkeletonStream.NextSkeletonEvent);
				}
				switch (WaitHandle.WaitAny(list2.ToArray(), 250))
				{
				case 258:
					continue;
				case 0:
					return;
				}
				bool flag4 = true;
				while (flag4)
				{
					if (flag && this._frameHelpers[0].IsReadyForSetArgs && this.ColorStream.NextFrameEvent.WaitOne(0))
					{
						this.GetColorFrame();
					}
					if (flag2 && this._frameHelpers[1].IsReadyForSetArgs && this.DepthStream.NextFrameEvent.WaitOne(0))
					{
						this.GetDepthFrame();
					}
					if (flag3 && this._frameHelpers[2].IsReadyForSetArgs && this.SkeletonStream.NextSkeletonEvent.WaitOne(0))
					{
						this.GetSkeletonFrame();
					}
					long earliestTimestampSoFar = long.MaxValue;
					int earliestFramenumberSoFar = int.MaxValue;
					FrameStateHelper frameStateHelper = null;
					FrameStateHelper[] frameHelpers = this._frameHelpers;
					foreach (FrameStateHelper frameStateHelper2 in frameHelpers)
					{
						if (frameStateHelper2.IsCandidateForInvoke(earliestTimestampSoFar, earliestFramenumberSoFar))
						{
							earliestTimestampSoFar = frameStateHelper2.MostRecentTimestamp;
							earliestFramenumberSoFar = frameStateHelper2.MostRecentFramenumber;
							frameStateHelper = frameStateHelper2;
						}
					}
					if (frameStateHelper != null)
					{
						frameStateHelper.Invoke(this);
					}
					else
					{
						flag4 = false;
					}
				}
				bool flag5 = (!flag || this._frameHelpers[0].HasArgs) && (!flag2 || this._frameHelpers[1].HasArgs) && (!flag3 || this._frameHelpers[2].HasArgs);
				if (flag5 && flag2 && flag3)
				{
					if (this._frameHelpers[1].MostRecentTimestamp < this._frameHelpers[2].MostRecentTimestamp)
					{
						this._frameHelpers[1].ResetAndReturnArgs();
						flag5 = false;
					}
					else if (this._frameHelpers[1].MostRecentTimestamp > this._frameHelpers[2].MostRecentTimestamp)
					{
						this._frameHelpers[2].ResetAndReturnArgs();
						flag5 = false;
					}
				}
				if (flag5 && flag && (flag2 || flag3))
				{
					long num = (flag2 ? this._frameHelpers[1].MostRecentTimestamp : this._frameHelpers[2].MostRecentTimestamp);
					long num2 = this._frameHelpers[0].MostRecentTimestamp - num;
					if (num2 > 16 && num2 <= 33)
					{
						if (flag2)
						{
							this._frameHelpers[1].ResetAndReturnArgs();
						}
						if (flag3)
						{
							this._frameHelpers[2].ResetAndReturnArgs();
						}
						flag5 = false;
					}
				}
				if (flag5)
				{
					//TO DO: 
					//KinectEtwProvider.EventWriteAllFramesReady(_frameHelpers[0].MostRecentTimestamp, _frameHelpers[1].MostRecentTimestamp, _frameHelpers[2].MostRecentTimestamp);
					AllFramesReadyEventArgs eventArgs = new AllFramesReadyEventArgs((ColorImageFrameReadyEventArgs)this._frameHelpers[0].ResetAndReturnArgs(), (DepthImageFrameReadyEventArgs)this._frameHelpers[1].ResetAndReturnArgs(), (SkeletonFrameReadyEventArgs)this._frameHelpers[2].ResetAndReturnArgs());
					this._allFramesReadyContextHandler.Invoke(this, eventArgs);
				}
			}
		}

		/// <summary>
		/// Pulls a depth frame out of the NUI pipeline, if available.
		/// </summary>
		private void GetDepthFrame()
		{
			if (this.DepthStream != null && this.DepthStream.TryGetNextFrameInternal(0, out var frameNumber, out var timestamp, out var frameFlags))
			{
				this._frameHelpers[1].SetArgs(DepthImageFrameReadyEventArgs.Create(this.DepthStream, frameNumber, timestamp, frameFlags), timestamp, frameNumber);
			}
		}

		/// <summary>
		/// Pulls a color frame out of the NUI pipeline, if available.
		/// </summary>
		private void GetColorFrame()
		{
			if (this.ColorStream != null && this.ColorStream.TryGetNextFrameInternal(0, out var frameNumber, out var timestamp, out var frameFlags))
			{
				this._frameHelpers[0].SetArgs(ColorImageFrameReadyEventArgs.Create(this.ColorStream, frameNumber, timestamp, frameFlags), timestamp, frameNumber);
			}
		}

		/// <summary>
		/// Pulls a skeletal frame out of the NUI pipeline, if available.
		/// </summary>
		private void GetSkeletonFrame()
		{
			if (this.SkeletonStream != null && this.SkeletonStream.TryGetNextFrameInternal(0, out var frameNumber, out var timestamp, out var trackingMode))
			{
				this._frameHelpers[2].SetArgs(SkeletonFrameReadyEventArgs.Create(this.SkeletonStream, frameNumber, timestamp, trackingMode), timestamp, frameNumber);
			}
		}

		/// <summary>
		/// Frees all memory associated with the sensor.  Terminates all streaming.
		/// </summary>
		public void Dispose()
		{
			lock (this._disposeLock)
			{
				if (this.IsDisposed)
				{
					return;
				}
				using (this.EnterWriteLock())
				{
					this._allFramesReadyContextHandler.Dispose();
					this._depthFrameReadyContextHandler.Dispose();
					this._colorFrameReadyContextHandler.Dispose();
					this._skeletonFrameReadyContextHandler.Dispose();
					if (this.IsRunningInternal)
					{
						this.Uninitialize();
					}
					string name = null;
					if (this.LastStatus == KinectStatus.Connected)
					{
						name = this.GetDeviceConnectionId();
					}
					if (this.DepthStream != null)
					{
						this.DepthStream.Dispose();
						this.DepthStream = null;
					}
					if (this.ColorStream != null)
					{
						this.ColorStream.Dispose();
						this.ColorStream = null;
					}
					if (this.SkeletonStream != null)
					{
						this.SkeletonStream.Dispose();
						this.SkeletonStream = null;
					}
					if (this._audioSource != null)
					{
						this._audioSource.Dispose();
						this._audioSource = null;
					}
					this.NuiSensor = null;
					KinectSensorCollection.Instance.RemakeSensor(this, name);
					this.IsDisposed = true;
				}
				this._initLock.Dispose();
			}
		}

		/// <summary>
		/// Throws an ObjectDisposedException if this instance is diposed.
		/// If this instance is not disposed, will return an AutoLock entered for Read.
		/// </summary>
		/// <returns>The read lock.</returns>
		private AutoLock EnterReadLock()
		{
			lock (this._disposeLock)
			{
				if (this.IsDisposed)
				{
					throw new ObjectDisposedException("KinectSensor");
				}
				return this._initLock.EnterReadLockAuto();
			}
		}

		/// <summary>
		/// Throws an ObjectDisposedException if this instance is diposed.
		/// If this instance is not disposed, will return an AutoLock entered for UpgradeableRead.
		/// </summary>
		/// <returns>The upgradable lock.</returns>
		/// <exception cref="T:System.ObjectDisposedException">Thrown if the object is disposed.</exception>
		private AutoLock EnterUpgradeableReadLock()
		{
			lock (this._disposeLock)
			{
				if (this.IsDisposed)
				{
					throw new ObjectDisposedException("KinectSensor");
				}
				return this._initLock.EnterUpgradeableReadLockAuto();
			}
		}

		/// <summary>
		/// Throws an ObjectDisposedException if this instance is diposed.
		/// If this instance is not disposed, will return an AutoLock entered for Write.
		/// </summary>
		/// <returns>The write lock.</returns>
		private AutoLock EnterWriteLock()
		{
			lock (this._disposeLock)
			{
				if (this.IsDisposed)
				{
					throw new ObjectDisposedException("KinectSensor");
				}
				return this._initLock.EnterWriteLockAuto();
			}
		}

		/// <summary>
		/// Throws an InvalidOperationEception is this instance is not disposed and is not running.
		/// Must be called under _initLock for thread safety.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">Thrown if the sensor is not running.</exception>
		private void CheckNotRunning()
		{
			if (!this.IsRunningInternal)
			{
				throw new InvalidOperationException(Resources.KinectMustBeRunning);
			}
		}
	}
}
