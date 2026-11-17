using System;
using System.Threading;
using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This class maintains the stream of data related to the skeleton.
	/// </summary>
	public sealed class SkeletonStream
	{
		/// <summary>
		/// This flag is returned in dwFlags of a skeleton frame that was captured while in seated mode.
		/// </summary>
		private const uint NUI_SKELETON_FRAME_FLAG_SEATED_SUPPORT_ENABLED = 8u;

		/// <summary>
		/// This value prevents the INuiSensor::NuiSkeletonGetNextFrame method from returning E_NUI_FRAME_NO_DATA errors. 
		/// Instead, calls to NuiSkeletonGetNextFrame block execution until data is available or the timeout period passes.
		/// </summary>
		private const uint NUI_SKELETON_TRACKING_FLAG_SUPPRESS_NO_FRAME_DATA = 1u;

		/// <summary>
		/// This value disables the default player selection mode and enables the title to choose which detected skeletons are tracked.
		/// </summary>
		private const uint NUI_SKELETON_TRACKING_FLAG_TITLE_SETS_TRACKED_SKELETONS = 2u;

		/// <summary>
		/// This value is used to enable seated skeleton tracking support.
		/// </summary>
		private const uint NUI_SKELETON_TRACKING_FLAG_ENABLE_SEATED_SUPPORT = 4u;

		/// <summary>
		/// This value is used to enable skeleton tracking while the depth camera is in near mode.
		/// </summary>
		private const uint NUI_SKELETON_TRACKING_FLAG_ENABLE_IN_NEAR_RANGE = 8u;

		/// <summary>
		/// This constant is the maximum number of trackable skeletons.
		/// </summary>
		internal const int s_skeletonCount = 6;

		/// <summary>
		/// This is the associated KinectSensor.
		/// </summary>
		private readonly KinectSensor _nuiSensor;

		/// <summary>
		/// This is the stream's pool for SkeletonData.
		/// </summary>
		private readonly DataPool<int, Tuple<float, float, float, float>, object, Skeleton[]> _skeletonDataPool;

		/// <summary>
		/// This event is used to indicate the availability of a new skeleton frame.
		/// </summary>
		internal ManualResetEvent NextSkeletonEvent = new ManualResetEvent(initialState: false);

		/// <summary>
		/// This field indicates of the stream is enabled or not.
		/// </summary>
		private bool _isEnabled;

		/// <summary>
		/// This field indicates if the app is choosing skeletons or not.
		/// </summary>
		private bool _appChoosesSkeletons;

		/// <summary>
		/// Indicates whether the pipeline is running in seated or default tracking mode.
		/// </summary>
		private SkeletonTrackingMode _trackingMode;

		/// <summary>
		/// Indicates whether skeleton tracking is enabled when the depth
		/// camera is in near mode.
		/// </summary>
		private bool _enableTrackingInNearRange;

		/// <summary>
		/// This field indicates of the stream is tracking skeletons or not.
		/// </summary>
		private bool _isTracking;

		/// <summary>
		/// Gets the associated KinectSensor.
		/// </summary>
		internal KinectSensor Sensor => _nuiSensor;

		/// <summary>
		/// Gets a value indicating whether the skeleton stream is enabled.
		/// </summary>
		public bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			private set
			{
				if (_isEnabled != value)
				{
					if (_nuiSensor.IsRunning && !value)
					{
						StopTracking();
					}
					_isEnabled = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the application will be responsible for choosing skeletons or not.
		/// </summary>
		public bool AppChoosesSkeletons
		{
			get
			{
				return _appChoosesSkeletons;
			}
			set
			{
				if (_appChoosesSkeletons != value)
				{
					_appChoosesSkeletons = value;
					if (_isTracking)
					{
						StartTracking();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the tracking mode.
		/// </summary>
		/// <value>
		/// The current tracking mode of the skeleton stream.
		/// The default value is SkeletonTrackingMode.Default.
		/// </value>
		/// <remarks>
		/// This property may be set at any time, regardless of the current
		/// state of SkeletonStream.IsEnabled or KinectSensor.IsRunning.
		/// If the tracking mode is changed while both SkeletonStream.IsEnabled
		/// and KinectSensor.IsRunning are true, any skeletons currently being
		/// tracked will be lost.
		/// </remarks>
		/// <exception cref="T:System.InvalidOperationException">
		/// Indicates that the Kinect sensor is in a state that prevents
		/// skeleton tracking from running.
		/// </exception>
		public SkeletonTrackingMode TrackingMode
		{
			get
			{
				return _trackingMode;
			}
			set
			{
				if (_trackingMode != value)
				{
					_trackingMode = value;
					if (_isTracking)
					{
						StartTracking();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether skeleton tracking is enabled
		/// when the KinectSensor's DepthImageStream.Range is DepthRange.Near.
		/// </summary>
		/// <value>
		/// The current tracking mode of the flag.
		/// The default value is false.
		/// </value>
		/// <exception cref="T:System.InvalidOperationException">
		/// Indicates that the Kinect sensor is in a state that prevents
		/// skeleton tracking from running.
		/// </exception>
		public bool EnableTrackingInNearRange
		{
			get
			{
				return _enableTrackingInNearRange;
			}
			set
			{
				if (_enableTrackingInNearRange != value)
				{
					_enableTrackingInNearRange = value;
					if (_isTracking)
					{
						StartTracking();
					}
				}
			}
		}

		/// <summary>
		/// Gets the associated smoothing parameters.
		/// </summary>
		public TransformSmoothParameters SmoothParameters { get; private set; }

		/// <summary>
		/// Gets a value indicating whether smoothing is enabled.
		/// </summary>
		public bool IsSmoothingEnabled { get; private set; }

		/// <summary>
		/// Gets the total length of the skeleton data buffer of each SkeletonFrame from this SkeletonStream.
		/// </summary>
		public int FrameSkeletonArrayLength => 6;

		/// <summary>
		/// Initializes a new instance of the SkeletonStream class.
		/// </summary>
		/// <param name="mainNui">The associated KinectSensor.</param>
		internal SkeletonStream(KinectSensor mainNui)
		{
			_nuiSensor = mainNui;
			_isEnabled = false;
			_skeletonDataPool = new DataPool<int, Tuple<float, float, float, float>, object, Skeleton[]>(2);
		}

		/// <summary>
		/// This method is called to enable the skeleton stream with the default smoothing values.
		/// </summary>
		public void Enable()
		{
			SetSmoothingState(enable: false, default(TransformSmoothParameters));
			SetEnabledState(enable: true);
		}

		/// <summary>
		/// This method is called to enable the skeleton stream.
		/// </summary>
		/// <param name="smoothParameters">The desired smoothing parameters.</param>
		public void Enable(TransformSmoothParameters smoothParameters)
		{
			if (smoothParameters.Smoothing < 0f || smoothParameters.Smoothing >= 1f || smoothParameters.Correction < 0f || smoothParameters.Correction > 1f || smoothParameters.Prediction < 0f || smoothParameters.JitterRadius < 0f || smoothParameters.MaxDeviationRadius < 0f)
			{
				throw new ArgumentOutOfRangeException("smoothParameters");
			}
			if (IsEnabled && smoothParameters != SmoothParameters)
			{
				SetEnabledState(enable: false);
			}
			SetSmoothingState(enable: true, smoothParameters);
			SetEnabledState(enable: true);
		}

		/// <summary>
		/// This method is called to disable the skeleton stream.
		/// </summary>
		public void Disable()
		{
			SetSmoothingState(enable: false, default(TransformSmoothParameters));
			SetEnabledState(enable: false);
		}

		/// <summary>
		/// This private method sets the enable state.
		/// </summary>
		/// <param name="enable">The value to set it to.</param>
		private void SetEnabledState(bool enable)
		{
			IsEnabled = enable;
			_nuiSensor.RequestSkeletonEngine(enable);
		}

		/// <summary>
		/// This method sets the smoothing state.
		/// </summary>
		/// <param name="enable">The value to set it to.</param>
		/// <param name="smoothParameters">The related smoothing parameters to use.</param>
		private void SetSmoothingState(bool enable, TransformSmoothParameters smoothParameters)
		{
			IsSmoothingEnabled = enable;
			SmoothParameters = smoothParameters;
		}

		/// <summary>
		/// This method chooses the default skeletons.
		/// </summary>
		public void ChooseSkeletons()
		{
			ChooseSkeletons(0, 0);
		}

		/// <summary>
		/// This method chooses a skeleton to track.
		/// </summary>
		/// <param name="trackingId1">The skeleton ID the application wishes to track.</param>
		public void ChooseSkeletons(int trackingId1)
		{
			ChooseSkeletons(trackingId1, 0);
		}

		/// <summary>
		/// This method chooses a skeleton to track.
		/// </summary>
		/// <param name="trackingId1">The first skeleton ID the application wishes to track.</param>
		/// <param name="trackingId2">The second skeleton ID the application wishes to track.</param>
		public void ChooseSkeletons(int trackingId1, int trackingId2)
		{
			_nuiSensor.NuiSensor.NuiSkeletonSetTrackedSkeletons((uint)trackingId1, (uint)trackingId2);
		}

		/// <summary>
		/// This private method begins tracking skeletons.
		/// </summary>
		private void StartTracking()
		{
			uint num = 1u;
			if (_trackingMode == SkeletonTrackingMode.Seated)
			{
				num |= 4;
			}
			if (_enableTrackingInNearRange)
			{
				num |= 8;
			}
			if (_appChoosesSkeletons)
			{
				num |= 2;
			}
			IntPtr hNextFrameEvent = NextSkeletonEvent.SafeWaitHandle.DangerousGetHandle();
			_nuiSensor.NuiSensor.NuiSkeletonTrackingEnable(hNextFrameEvent, num);
			_isTracking = true;
		}

		/// <summary>
		/// This private method stops tracking skeletons.
		/// </summary>
		private void StopTracking()
		{
			_nuiSensor.NuiSensor.NuiSkeletonTrackingDisableNoThrow();
			_isTracking = false;
		}

		/// <summary>
		/// This internal method starts tracking.
		/// </summary>
		internal void Start()
		{
			if (_isEnabled)
			{
				StartTracking();
			}
		}

		/// <summary>
		/// This internal method stops tracking.
		/// </summary>
		internal void Stop()
		{
			if (_isTracking)
			{
				StopTracking();
			}
		}

		/// <summary>
		/// Retrieves the next SkeletonFrame from the sensor.  The SkeletonFrame must be disposed.
		/// </summary>
		/// <param name="millisecondsWait">The time to wait for a new frame from the sensor, in milliseconds.</param>
		/// <returns>The next ColorImageFrame, or null if the timeout has expired.</returns>
		public SkeletonFrame OpenNextFrame(int millisecondsWait)
		{
			SkeletonFrame result = null;
			if (!_nuiSensor.IsRunning)
			{
				throw new InvalidOperationException(Resources.SensorMustBeRunning);
			}
			if (!_isEnabled)
			{
				throw new InvalidOperationException(Resources.SkeletonEngineMustBeEnabled);
			}
			if (_nuiSensor.HasSkeletonInvocations)
			{
				throw new InvalidOperationException(Resources.CannotPollAndUseEvents);
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedOpenNextFrameInfo(2, millisecondsWait);
			if (TryGetNextFrameInternal(millisecondsWait, out var frameNumber, out var timestamp, out var trackingMode))
			{
				result = SkeletonFrame.Create(this, frameNumber, timestamp, trackingMode);
			}
			return result;
		}

		/// <summary>
		/// Used to store the skeleton data during the window when it's available from native code.  
		/// </summary>
		/// <param name="frameNumber">The frameNumber of the skeleton data.</param>
		/// <param name="skeletonData">The native description of the skeleton data.</param>
		/// <param name="floorClipPlane">The floorClipPlane of the new frame.</param>
		internal void StoreSkeletonData(int frameNumber, _NUI_SKELETON_FRAME skeletonData, Tuple<float, float, float, float> floorClipPlane)
		{
			DataPool<int, Tuple<float, float, float, float>, object, Skeleton[], bool?>.Entry entry = _skeletonDataPool.CheckOutFreeEntryForUpdate();
			entry.Key = frameNumber;
			entry.Value1 = floorClipPlane;
			if (entry.Value3 == null || entry.Value3.Length != skeletonData.SkeletonData.Length)
			{
				entry.Value3 = new Skeleton[skeletonData.SkeletonData.Length];
			}
			for (int i = 0; i < entry.Value3.Length; i++)
			{
				if (entry.Value3[i] == null)
				{
					entry.Value3[i] = new Skeleton(skeletonData.SkeletonData[i]);
				}
				else
				{
					entry.Value3[i].CopyFrom(skeletonData.SkeletonData[i]);
				}
			}
			_skeletonDataPool.CheckInEntryForUpdate(entry);
		}

		/// <summary>
		/// Retrieves the next frame from the sensor, and calls "StoreSkeletonData" with the data, if present.
		/// </summary>
		/// <param name="millisecondsWait">The time to wait for a new frame from the sensor, in milliseconds.</param>
		/// <param name="frameNumber">The frameNumber of the new frame.</param>
		/// <param name="timestamp">The timestamp of the new frame.</param>
		/// <param name="trackingMode">The tracking mode in which the frame was captured.</param>
		/// <returns>This method returns true if the method succeeded and false otherwise.</returns>
		internal bool TryGetNextFrameInternal(int millisecondsWait, out int frameNumber, out long timestamp, out SkeletonTrackingMode trackingMode)
		{
			frameNumber = 0;
			timestamp = 0L;
			trackingMode = SkeletonTrackingMode.Default;
			_NUI_SKELETON_FRAME pSkeletonFrame = default(_NUI_SKELETON_FRAME);
			int num = _nuiSensor.NuiSensor.NuiSkeletonGetNextFrameNoThrow((uint)millisecondsWait, ref pSkeletonFrame);
			if (num < 0)
			{
				NextSkeletonEvent.Reset();
				return false;
			}
			if (num == 0)
			{
				if (IsSmoothingEnabled)
				{
					_NUI_TRANSFORM_SMOOTH_PARAMETERS pSmoothingParams = new _NUI_TRANSFORM_SMOOTH_PARAMETERS
					{
						fSmoothing = SmoothParameters.Smoothing,
						fCorrection = SmoothParameters.Correction,
						fPrediction = SmoothParameters.Prediction,
						fJitterRadius = SmoothParameters.JitterRadius,
						fMaxDeviationRadius = SmoothParameters.MaxDeviationRadius
					};
					_nuiSensor.NuiSensor.NuiTransformSmooth(ref pSkeletonFrame, ref pSmoothingParams);
				}
				frameNumber = (int)pSkeletonFrame.dwFrameNumber;
				timestamp = pSkeletonFrame.liTimeStamp.QuadPart;
				if ((pSkeletonFrame.dwFlags & 8) != 0)
				{
					trackingMode = SkeletonTrackingMode.Seated;
				}
				Tuple<float, float, float, float> floorClipPlane = new Tuple<float, float, float, float>(pSkeletonFrame.vFloorClipPlane.x, pSkeletonFrame.vFloorClipPlane.y, pSkeletonFrame.vFloorClipPlane.z, pSkeletonFrame.vFloorClipPlane.w);
				StoreSkeletonData(frameNumber, pSkeletonFrame, floorClipPlane);
				return true;
			}
			return false;
		}

		/// <summary>
		/// This method locks the associated skeleton frame.
		/// </summary>
		/// <param name="frameNumber">The frame number to lock.</param>
		/// <returns>The available frame.</returns>
		internal DataPool<int, Tuple<float, float, float, float>, object, Skeleton[], bool?>.Entry LockSkeletonData(int frameNumber)
		{
			_skeletonDataPool.TryLockEntry(frameNumber, out var entry);
			return entry;
		}

		/// <summary>
		/// This method unlocks the frame data.
		/// </summary>
		/// <param name="entry">The skeleton frame to unlock.</param>
		internal void UnlockSkeletonData(DataPool<int, Tuple<float, float, float, float>, object, Skeleton[], bool?>.Entry entry)
		{
			_skeletonDataPool.UnlockEntry(entry);
		}

		/// <summary>
		/// This method disposes the SkeletonStream.
		/// </summary>
		internal void Dispose()
		{
			if (NextSkeletonEvent != null)
			{
				NextSkeletonEvent.Close();
			}
		}
	}
}
