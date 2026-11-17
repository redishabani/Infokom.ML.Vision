using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Wraps the native INuiSensor interface, which provides native access to a sensor.
	/// </summary>
	internal class NuiSensor
	{
		/// <summary>
		/// The wrapped sensor.
		/// </summary>
		private readonly INuiSensor _nuiSensor;

		/// <summary>
		/// The current depth filter.
		/// </summary>
		private IDepthFilter _depthFilter;

		/// <summary>
		/// All depth filters ever used by this sensor.
		/// </summary>
		private List<IDepthFilter> _depthFilters = new List<IDepthFilter>();

		/// <summary>
		/// Caches the INuiAudioBeam object for the sensor.
		/// </summary>
		private INuiAudioBeam _nuiAudioSource;

		/// <summary>
		/// Caches the INuiCoordinateMapper object for the sensor.
		/// </summary>
		private NuiCoordinateMapper _nuiCoordinateMapper;

		/// <summary>
		/// Gets a value indicating whether the device is a supported Kinect.
		/// </summary>
		internal bool IsSupported
		{
			get
			{
				object obj = this[NuiSensorProperty.IsSupported];
				if (obj != null && obj.GetType() == typeof(bool))
				{
					return (bool)obj;
				}
				return false;
			}
		}

		/// <summary>
		/// Returns a private property from the NuiSensors IPropertyStore.
		/// </summary>
		/// <param name="index">Index of the property to retrieve.</param>
		/// <returns>The property value.</returns>
		public object this[NuiSensorProperty index]
		{
			get
			{
				try
				{
					IPropertyStore propertyStore = (IPropertyStore)_nuiSensor;
					PROPERTYKEY pROPERTYKEY = default(PROPERTYKEY);
					pROPERTYKEY.fmtid = new Guid("1f5e088c-a8c7-41d3-9957-209677a13e85");
					pROPERTYKEY.pid = (int)index;
					PROPERTYKEY key = pROPERTYKEY;
					propertyStore.GetValue(ref key, out var pv);
					return pv;
				}
				catch (COMException)
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the infrared emitter is disabled.
		/// Default value of false.
		/// </summary>
		public bool ForceInfraredEmitterOff
		{
			get
			{
				return _nuiSensor.NuiGetForceInfraredEmitterOff() != 0;
			}
			set
			{
				KinectExceptionHelper.CheckHr(_nuiSensor.NuiSetForceInfraredEmitterOff(value ? 1 : 0));
			}
		}

		/// <summary>
		/// Gets or sets a filter to be applied to each depth frame.
		/// </summary>
		public IDepthFilter DepthFilter
		{
			get
			{
				return _depthFilter;
			}
			set
			{
				INuiDepthFilter nuiDepthFilter = null;
				if (value != null)
				{
					nuiDepthFilter = value.NativeObject as INuiDepthFilter;
					if (nuiDepthFilter == null)
					{
						throw new ArgumentException(Resources.DepthFilterMustImplementNativeInterface, "value");
					}
				}
				KinectExceptionHelper.CheckHr(_nuiSensor.NuiSetDepthFilter(nuiDepthFilter));
				_depthFilter = value;
				if (value != null && !_depthFilters.Contains(value))
				{
					_depthFilters.Add(value);
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the NuiSensor class.
		/// Wrapper for INuiSensor.  Slightly more managed friendly than a raw INuiSensor.
		/// </summary>
		/// <param name="wrapped">INuiSensor object to wrap.</param>
		public NuiSensor(INuiSensor wrapped)
		{
			_nuiSensor = wrapped;
		}

		/// <summary>
		/// Gets the zero-based sensor index.
		/// </summary>
		/// <returns>Returns the sensor index.</returns>
		public int NuiInstanceIndex()
		{
			return _nuiSensor.NuiInstanceIndex();
		}

		/// <summary>
		/// Initializes the NUI engine.
		/// </summary>
		/// <param name="dwFlags">The NUI subsystems to initialize, as a bitwise-OR combination of 
		/// the NUI_INITIALIZE constants.</param>
		public void NuiInitialize(uint dwFlags)
		{
			KinectExceptionHelper.CheckHr(_nuiSensor.NuiInitialize(dwFlags));
		}

		/// <summary>
		/// Gets the audio source from a specific sensor.
		/// </summary>
		/// <returns>The INuiAudioBeam interface for the audio source.</returns>
		public INuiAudioBeam NuiGetAudioSource()
		{
			if (_nuiAudioSource == null)
			{
				KinectExceptionHelper.CheckHr(_nuiSensor.NuiGetAudioSource(out var ppDmo));
				_nuiAudioSource = new NuiAudioBeam(ppDmo);
			}
			return _nuiAudioSource;
		}

		/// <summary>
		/// Gets the coordinate mapping object from a specific sensor.
		/// </summary>
		/// <returns>The INuiCoordinateMapper interface for the sensor.</returns>
		public NuiCoordinateMapper NuiGetCoordinateMapper()
		{
			if (_nuiCoordinateMapper == null)
			{
				KinectExceptionHelper.CheckHr(_nuiSensor.NuiGetCoordinateMapper(out var pMapping));
				_nuiCoordinateMapper = new NuiCoordinateMapper(pMapping);
			}
			return _nuiCoordinateMapper;
		}

		/// <summary>
		/// Shuts down the NUI engine.
		/// </summary>
		public void NuiShutdown()
		{
			_nuiAudioSource = null;
			_nuiSensor.NuiShutdown();
		}

		/// <summary>
		/// Sets the event that signals the last frame.
		/// </summary>
		/// <param name="hEvent">A handle to the event.</param>
		/// <param name="dwFrameEventFlag">The frame event options, as a bitwise-OR combination of the NUI_IMAGE_STREAM_FLAG constants.</param>
		public void NuiSetFrameEndEvent(IntPtr hEvent, uint dwFrameEventFlag)
		{
			KinectExceptionHelper.CheckHr(_nuiSensor.NuiSetFrameEndEvent(hEvent, dwFrameEventFlag));
		}

		/// <summary>
		/// Opens an image stream.
		/// </summary>
		/// <param name="eImageType">A NUI_IMAGE_TYPE value that specifies which image stream to open. 
		/// The valid values for this parameter depend on the flags that you passed to the dwFlags 
		/// parameter of the INuiSensor::NuiInitialize method.</param>
		/// <param name="eResolution">A NUI_IMAGE_RESOLUTION value that specifies which resolution to 
		/// use for the image stream. The valid values for this parameter depend on the flags that you 
		/// passed to the dwFlags parameter of the INuiSensor::NuiInitialize method.</param>
		/// <param name="dwImageFrameFlags">Not used; specify NULL.</param>
		/// <param name="dwFrameLimit">The number of frames that the NUI runtime should buffer. 
		/// The maximum value is NUI_IMAGE_STREAM_FRAME_LIMIT_MAXIMUM. Most applications 
		/// should use a frame limit of two.</param>
		/// <param name="hNextFrameEvent">A handle to a manual reset event that will be fired when 
		/// the next frame in the stream is available.</param>
		/// <param name="phStreamHandle">A pointer that receives a handle to the opened stream.</param>
		public void NuiImageStreamOpen(_NUI_IMAGE_TYPE eImageType, _NUI_IMAGE_RESOLUTION eResolution, uint dwImageFrameFlags, uint dwFrameLimit, IntPtr hNextFrameEvent, out IntPtr phStreamHandle)
		{
			KinectExceptionHelper.CheckHr(_nuiSensor.NuiImageStreamOpen(eImageType, eResolution, dwImageFrameFlags, dwFrameLimit, hNextFrameEvent, out phStreamHandle));
		}

		/// <summary>
		/// Sets the image frame flags for the specified stream.
		/// </summary>
		/// <param name="hStream">A handle to the stream.</param>
		/// <param name="dwImageFrameFlags">The image frame flags.</param>
		public void NuiImageStreamSetImageFrameFlags(IntPtr hStream, uint dwImageFrameFlags)
		{
			KinectExceptionHelper.CheckHr(_nuiSensor.NuiImageStreamSetImageFrameFlags(hStream, dwImageFrameFlags));
		}

		/// <summary>
		/// Gets the image frame flags for the specified stream.
		/// </summary>
		/// <param name="hStream">A handle to the stream.</param>
		/// <returns>A DWORD that receives the image frame flags.</returns>
		public uint NuiImageStreamGetImageFrameFlags(IntPtr hStream)
		{
			KinectExceptionHelper.CheckHr(_nuiSensor.NuiImageStreamGetImageFrameFlags(hStream, out var pdwImageFrameFlags));
			return pdwImageFrameFlags;
		}

		/// <summary>
		/// Gets the next frame of data from the specified image stream.
		/// </summary>
		/// <param name="hStream">A handle to the image stream. This stream must have been opened by a call to the NuiImageStreamOpen method.</param>
		/// <param name="dwMillisecondsToWait">The timeout (in milliseconds) before returning without a new frame.</param>
		/// <param name="pImageFrame">A pointer to a NUI_IMAGE_FRAME structure that receives the next image 
		/// frame in the specified stream. The pFrameTexture member of the structure points to an 
		/// INuiFrameTexture instance that contains the frame data.</param>
		/// <returns>Returns S_OK if successful; otherwise, returns a failure code.</returns>
		public int NuiImageStreamGetNextFrameNoThrow(IntPtr hStream, uint dwMillisecondsToWait, out _NUI_IMAGE_FRAME pImageFrame)
		{
			return _nuiSensor.NuiImageStreamGetNextFrame(hStream, dwMillisecondsToWait, out pImageFrame);
		}

		/// <summary>
		/// Releases the specified frame of data from the specified stream.
		/// </summary>
		/// <param name="hStream">A handle to the stream. This stream must have been opened by a call to the INuiSensor::NuiImageStreamOpen method.</param>
		/// <param name="pImageFrame">A pointer to the frame to release.</param>
		public void NuiImageStreamReleaseFrame(IntPtr hStream, ref _NUI_IMAGE_FRAME pImageFrame)
		{
			try
			{
				INuiSensor nuiSensor = _nuiSensor;
				_NUI_IMAGE_FRAME pImageFrame2 = pImageFrame;
				nuiSensor.NuiImageStreamReleaseFrame(hStream, ref pImageFrame2);
				Marshal.FinalReleaseComObject(pImageFrame.pFrameTexture);
			}
			catch (COMException)
			{
			}
		}

		/// <summary>
		/// Returns a pointer to an INuiFrameTexture that contains the data of a depth frame
		/// in NUI_DEPTH_IMAGE_PIXEL format (instead of packed USHORTs).
		/// </summary>
		/// <param name="hStream">A handle to the open depth stream.</param>
		/// <param name="pImageFrame">
		/// The NUI_IMAGE_FRAME for which the texture is being requested. This must be a valid
		/// frame returned by a call to NuiImageStreamGetNextFrame on hStream, and not
		/// yet released by a call to NuiImageStreamReleaseFrame.
		/// </param>
		/// <param name="nearMode">
		/// Returns true if the frame was captured in near mode; false if it was captured in
		/// default mode.
		/// </param>
		/// <param name="ppTexture">
		/// Returns a pointer to an INuiFrameTexture containing the depth pixels.
		/// The lifetime of this texture is tied to the lifetime of pImageFrame;
		/// application code must not access the texture after NuiImageStreamRelease has been
		/// called on pImageFrame.
		/// </param>
		public void NuiImageFrameGetDepthImagePixelFrameTexture(IntPtr hStream, ref _NUI_IMAGE_FRAME pImageFrame, out bool nearMode, out INuiFrameTexture ppTexture)
		{
			KinectExceptionHelper.CheckHr(_nuiSensor.NuiImageFrameGetDepthImagePixelFrameTexture(hStream, ref pImageFrame, out var pNearMode, out ppTexture));
			nearMode = pNearMode != 0;
		}

		/// <summary>
		/// Gets the pixel coordinates in color space that correspond to the specified pixel coordinates in depth space, using the specified depth resolution. 
		/// </summary>
		/// <param name="eColorResolution">The resolution of the color image, as a NUI_IMAGE_RESOLUTION enumeration constant.</param>
		/// <param name="eDepthResolution">The resolution of the depth image, as a NUI_IMAGE_RESOLUTION enumeration constant.</param>
		/// <param name="pcViewArea">The optional zoom and pan settings of the color image, as a pointer 
		/// to a NUI_IMAGE_VIEW_AREA structure. To ensure that the settings are valid, use the ViewArea 
		/// member of the NUI_IMAGE_FRAME that you are registering pixels against. Do not instantiate 
		/// and populate this structure manually.</param>
		/// <param name="lDepthX">The X coordinate in depth image space.</param>
		/// <param name="lDepthY">The Y coordinate in depth image space.</param>
		/// <param name="usDepthValue">The depth value in depth image space. This value is constrained 
		/// between NUI_IMAGE_DEPTH_MINIMUM and NUI_IMAGE_DEPTH_MINIMUM.</param>
		/// <param name="plColorX">Pointer to a LONG value that receives the X coordinate of the pixel in 
		/// color image space. This pointer must be non-NULL when you call this function. If this method 
		/// does not return S_OK, this data is invalid. This value can be outside of the bounds of the 
		/// color image.</param>
		/// <param name="plColorY">Pointer to a LONG value that receives the Y coordinate of the pixel 
		/// in color image space. This pointer must be non-NULL when you call this function. If this 
		/// method does not return S_OK, this data is invalid. This value can be outside of the bounds 
		/// of the color image.</param>
		public void NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolution(_NUI_IMAGE_RESOLUTION eColorResolution, _NUI_IMAGE_RESOLUTION eDepthResolution, ref _NUI_IMAGE_VIEW_AREA pcViewArea, int lDepthX, int lDepthY, ushort usDepthValue, out int plColorX, out int plColorY)
		{
			KinectExceptionHelper.CheckHr(_nuiSensor.NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolution(eColorResolution, eDepthResolution, ref pcViewArea, lDepthX, lDepthY, usDepthValue, out plColorX, out plColorY));
		}

		/// <summary>
		/// Converts a frame's worth of data from depth space to color space. 
		/// </summary>
		/// <param name="eColorResolution">The resolution of the color image, as a NUI_IMAGE_RESOLUTION enumeration constant.</param>
		/// <param name="eDepthResolution">The resolution of the depth image, as a NUI_IMAGE_RESOLUTION enumeration constant.</param>
		/// <param name="depthValues">The data from an entire depth frame .</param>
		/// <param name="colorCoordinates">Array to hold the color frame X,Y coordinates for each pixel in the depth frame.</param>
		public unsafe void NuiImageGetColorPixelCoordinateFrameFromDepthPixelFrameAtResolution(_NUI_IMAGE_RESOLUTION eColorResolution, _NUI_IMAGE_RESOLUTION eDepthResolution, short[] depthValues, ColorImagePoint[] colorCoordinates)
		{
			fixed (ColorImagePoint* ptr = colorCoordinates)
			{
				KinectExceptionHelper.CheckHr(_nuiSensor.NuiImageGetColorPixelCoordinateFrameFromDepthPixelFrameAtResolution(eColorResolution, eDepthResolution, (uint)depthValues.Length, depthValues, (uint)(colorCoordinates.Length * 2), (IntPtr)ptr));
			}
		}

		/// <summary>
		/// Sets the elevation angle of the Kinect sensor.
		/// </summary>
		/// <param name="lAngleDegrees">The elevation angle relative to gravity, in degrees. 
		/// A value of zero indicates that the sensor array should point exactly horizontally. 
		/// Positive values indicate that the sensor array should point above the horizon, and 
		/// negative values indicate that the sensor array should point below the horizon. This 
		/// value is constrained between NUI_CAMERA_ELEVATION_MINIMUM and NUI_CAMERA_ELEVATION_MAXIMUM.</param>
		public void NuiCameraElevationSetAngle(int lAngleDegrees)
		{
			KinectExceptionHelper.CheckHr(_nuiSensor.NuiCameraElevationSetAngle(lAngleDegrees));
		}

		/// <summary>
		/// Gets the elevation angle of the Kinect sensor. 
		/// </summary>
		/// <returns>
		/// The current elevation angle relative to gravity, in degrees. The pointer must be non-NULL 
		/// when you call the function. After the function call returns, the LONG value contains the 
		/// elevation angle. Valid angles range from NUI_CAMERA_ELEVATION_MINIMUM to 
		/// NUI_CAMERA_ELEVATION_MAXIMUM.
		/// </returns>
		public int NuiCameraElevationGetAngle()
		{
			KinectExceptionHelper.CheckHr(_nuiSensor.NuiCameraElevationGetAngle(out var plAngleDegrees));
			return plAngleDegrees;
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
		public Vector4 NuiAccelerometerGetCurrentReading()
		{
			KinectExceptionHelper.CheckHr(_nuiSensor.NuiAccelerometerGetCurrentReading(out var pReading));
			return Vector4.CopyFrom(ref pReading);
		}

		/// <summary>
		/// Enables skeletal tracking.
		/// </summary>
		/// <param name="hNextFrameEvent">A handle to an application-allocated, manual reset event that 
		/// will be set whenever a new frame of skeleton data is available, and will be reset whenever 
		/// the latest frame data is returned. This can be NULL.</param>
		/// <param name="dwFlags">
		/// Flags that control skeleton tracking, as a bitwise-OR combination of the values 
		/// in the following table. Flag Description 
		/// NUI_SKELETON_TRACKING_FLAG_SUPPRESS_NO_FRAME_DATA  - Prevents the 
		/// INuiSensor::NuiSkeletonGetNextFrame method from returning E_NUI_FRAME_NO_DATA errors. 
		/// Instead, calls to NuiSkeletonGetNextFrame block execution until data 
		/// is available or the timeout period passes.  
		/// NUI_SKELETON_TRACKING_FLAG_TITLE_SETS_TRACKED_SKELETONS - Disables the default player 
		/// selection mode and enables the title to choose which detected skeletons are tracked. 
		/// </param>
		public void NuiSkeletonTrackingEnable(IntPtr hNextFrameEvent, uint dwFlags)
		{
			KinectExceptionHelper.CheckHr(_nuiSensor.NuiSkeletonTrackingEnable(hNextFrameEvent, dwFlags));
		}

		/// <summary>
		/// Disables skeleton tracking.
		/// </summary>
		/// <returns>S_OK if successful; otherwise, returns a failure code.</returns>
		public int NuiSkeletonTrackingDisableNoThrow()
		{
			return _nuiSensor.NuiSkeletonTrackingDisable();
		}

		/// <summary>
		/// Gets the next frame of data from the skeleton stream.
		/// </summary>
		/// <param name="dwMillisecondsToWait">The timeout (in milliseconds) before returning without a new frame.</param>
		/// <param name="pSkeletonFrame">A pointer to a NUI_SKELETON_FRAME structure that receives the next image frame in the skeleton stream.</param>
		/// <returns>Returns S_OK if successful; otherwise, returns one of the following failure codes: E_POINTER, E_NUI_FRAME_NO_DATA.</returns>
		public int NuiSkeletonGetNextFrameNoThrow(uint dwMillisecondsToWait, ref _NUI_SKELETON_FRAME pSkeletonFrame)
		{
			return _nuiSensor.NuiSkeletonGetNextFrame(dwMillisecondsToWait, ref pSkeletonFrame);
		}

		/// <summary>
		/// Filters skeleton positions to reduce jitter between frames.
		/// </summary>
		/// <param name="pSkeletonFrame">Points to a NUI_SKELETON_FRAME Structure that contains the skeleton data to be smoothed. On exit, the skeleton data in the structure has been replaced by smoothed data. If this function does not return S_OK, this data is unchanged.</param>
		/// <param name="pSmoothingParams">The parameters for the smoothing function. See the NUI_TRANSFORM_SMOOTH_PARAMETERS structure for a description of the parameters.</param>
		public void NuiTransformSmooth(ref _NUI_SKELETON_FRAME pSkeletonFrame, ref _NUI_TRANSFORM_SMOOTH_PARAMETERS pSmoothingParams)
		{
			KinectExceptionHelper.CheckHr(_nuiSensor.NuiTransformSmooth(ref pSkeletonFrame, ref pSmoothingParams));
		}

		/// <summary>
		/// Gets the sensor connection ID.
		/// </summary>
		/// <returns>The hub device instance id.</returns>
		public string NuiDeviceConnectionId()
		{
			return _nuiSensor.NuiDeviceConnectionId();
		}

		/// <summary>
		/// Gets the unique ID for the sensor.
		/// </summary>
		/// <returns>The camera device instance id.</returns>
		public string NuiUniqueId()
		{
			return _nuiSensor.NuiUniqueId();
		}

		/// <summary>
		/// Gets the audio array ID. 
		/// </summary>
		/// <returns>Returns the audio array ID.</returns>
		public string NuiAudioArrayId()
		{
			return _nuiSensor.NuiAudioArrayId();
		}

		/// <summary>
		/// Gets the connection status of the Kinect sensor.
		/// </summary>
		/// <returns>Returns S_OK if successful; otherwise, returns one of the following failure codes: E_NUI_NOTCONNECTED.</returns>
		public uint NuiStatus()
		{
			return (uint)_nuiSensor.NuiStatus();
		}

		/// <summary>
		/// Gets the flags that were passed to the INuiSensor::NuiInitialize method.
		/// </summary>
		/// <returns>Returns the initialization flags.</returns>
		public uint NuiInitializationFlags()
		{
			return _nuiSensor.NuiInitializationFlags();
		}

		/// <summary>
		/// Sets an array of IDs for skeletal tracking.
		/// </summary>
		/// <param name="trackingId1">The tracking ID for the first skeleton.</param>
		/// <param name="trackingId2">The tracking ID for the second skeleton.</param>
		public void NuiSkeletonSetTrackedSkeletons(uint trackingId1, uint trackingId2)
		{
			KinectExceptionHelper.CheckHr(_nuiSensor.NuiSkeletonSetTrackedSkeletons(new uint[2] { trackingId1, trackingId2 }));
		}

		/// <summary>
		/// Retrieves the color camera settings native object.
		/// This method will throw if the settings are not supported.
		/// </summary>
		/// <returns>The color camera settings.</returns>
		public NuiColorCameraSettings NuiGetColorCameraSettings()
		{
			KinectExceptionHelper.CheckHr(_nuiSensor.NuiGetColorCameraSettings(out var pCameraSettings));
			return new NuiColorCameraSettings(pCameraSettings);
		}

		/// <summary>
		/// Returns the depth filter corresponding to a depth or skeleton frame captured at a
		/// particular timestamp.
		/// </summary>
		/// <param name="timestamp">The timestamp at which the frame was captured.</param>
		/// <returns>The depth filter that was applied to the frame.</returns>
		public IDepthFilter GetDepthFilterForTimestamp(long timestamp)
		{
			_LARGE_INTEGER liTimeStamp = new _LARGE_INTEGER
			{
				QuadPart = timestamp
			};
			if (_nuiSensor.NuiGetDepthFilterForTimeStamp(liTimeStamp, out var ppDepthFilter) == 0 && ppDepthFilter != null)
			{
				foreach (IDepthFilter depthFilter in _depthFilters)
				{
					if (depthFilter.NativeObject as INuiDepthFilter == ppDepthFilter)
					{
						return depthFilter;
					}
				}
			}
			return null;
		}
	}
}
