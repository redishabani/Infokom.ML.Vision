using System;
using System.Threading;
using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// The abstract ImageStream class.
	/// </summary>
	public abstract class ImageStream
	{
		/// <summary>
		/// Used to indicate of a new frame is ready.
		/// </summary>
		internal ManualResetEvent NextFrameEvent = new ManualResetEvent(initialState: false);

		/// <summary>
		/// Indicates of the stream is capturing.
		/// </summary>
		private bool _capturing;

		/// <summary>
		/// The native stream handle.
		/// </summary>
		private IntPtr _nativeStreamHandle;

		/// <summary>
		/// The current ImageStreamFlags.
		/// </summary>
		private ImageStreamFlags _imageStreamFlags;

		/// <summary>
		/// The lock object used for synchronizing access to the stream.
		/// </summary>
		private readonly object _nativeStreamInitLock = new object();

		/// <summary>
		/// Gets the width in pixels of each ImageFrame from this ImageStream.
		/// </summary>
		public int FrameWidth { get; private set; }

		/// <summary>
		/// Gets the height in pixels of each ImageFrame from this ImageStream.
		/// </summary>
		public int FrameHeight { get; private set; }

		/// <summary>
		/// Gets or sets the bytes per pixel of each ImageFrame from this ImageStream.
		/// </summary>
		public int FrameBytesPerPixel { get; protected set; }

		/// <summary>
		/// Gets or sets the total length of the pixel data buffer of each ImageFrame from this ImageStream.
		/// </summary>
		public int FramePixelDataLength { get; protected set; }

		/// <summary>
		/// Gets the associated sensor.
		/// </summary>
		internal KinectSensor Sensor { get; private set; }

		/// <summary>
		/// Gets the buffer count.
		/// </summary>
		internal int BufferCount { get; private set; }

		/// <summary>
		/// Gets or sets the image resolution.
		/// </summary>
		internal ImageResolution Resolution { get; set; }

		/// <summary>
		/// Gets the image type.
		/// </summary>
		internal abstract ImageType ImageType { get; }

		/// <summary>
		/// Gets a value indicating whether the ImageStream is enabled.
		/// </summary>
		public bool IsEnabled { get; private set; }

		/// <summary>
		/// Gets or sets the ImageStreamFlags.
		/// </summary>
		internal ImageStreamFlags ImageStreamFlags
		{
			get
			{
				if (_capturing && Sensor.IsRunning)
				{
					_imageStreamFlags = (ImageStreamFlags)Sensor.NuiSensor.NuiImageStreamGetImageFrameFlags(_nativeStreamHandle);
				}
				return _imageStreamFlags;
			}
			set
			{
				if (_capturing && Sensor.IsRunning)
				{
					Sensor.NuiSensor.NuiImageStreamSetImageFrameFlags(_nativeStreamHandle, (uint)value);
				}
				_imageStreamFlags = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the ImageStream class.
		/// </summary>
		/// <param name="sensor">The related sensor.</param>
		/// <param name="imageStreamFlags">The ImageStreamFlags.</param>
		internal ImageStream(KinectSensor sensor, ImageStreamFlags imageStreamFlags = ImageStreamFlags.NUI_IMAGE_STREAM_FLAG_SUPPRESS_NO_FRAME_DATA)
		{
			Sensor = sensor;
			BufferCount = 2;
			IsEnabled = false;
			_imageStreamFlags = imageStreamFlags;
		}

		/// <summary>
		/// Enable the stream.
		/// </summary>
		internal void InternalEnable()
		{
			if (Sensor.IsRunning)
			{
				Open();
			}
			IsEnabled = true;
		}

		/// <summary>
		/// Disable the stream.
		/// </summary>
		public void Disable()
		{
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedImageStreamDisabledInfo((!(this is ColorImageStream)) ? 1 : 0);
			if (IsEnabled)
			{
				if (Sensor.IsRunning)
				{
					Close();
				}
				IsEnabled = false;
			}
		}

		/// <summary>
		/// Starts the stream.
		/// </summary>
		internal void Start()
		{
			if (IsEnabled)
			{
				Open();
			}
		}

		/// <summary>
		/// Stops the stream.
		/// </summary>
		internal void Stop()
		{
			if (_capturing)
			{
				Close();
			}
		}

		/// <summary>
		/// Populates the FrameWidth and FrameHeight values.
		/// </summary>
		internal void FillHeightWidth()
		{
			ResolutionToHeightWidth(Resolution, out var width, out var height);
			FrameWidth = width;
			FrameHeight = height;
		}

		/// <summary>
		/// Sets the width and height based on the given resolution.
		/// </summary>
		/// <param name="resolution">The resolution.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		internal static void ResolutionToHeightWidth(ImageResolution resolution, out int width, out int height)
		{
			switch (resolution)
			{
			case ImageResolution.Resolution1280x960:
				width = 1280;
				height = 960;
				break;
			case ImageResolution.Resolution320x240:
				width = 320;
				height = 240;
				break;
			case ImageResolution.Resolution640x480:
				width = 640;
				height = 480;
				break;
			case ImageResolution.Resolution80x60:
				width = 80;
				height = 60;
				break;
			default:
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Looks up the number of bytes per pixel.
		/// </summary>
		/// <param name="imageType">The image type.</param>
		/// <returns>The number of bytes per pixel.</returns>
		internal static int ImageTypeToBytesPerPixel(ImageType imageType)
		{
			switch (imageType)
			{
			case ImageType.Color:
			case ImageType.ColorYuv:
				return 4;
			case ImageType.DepthAndPlayerIndex:
			case ImageType.ColorYuvRaw:
			case ImageType.Depth:
			case ImageType.ColorInfrared:
				return 2;
			case ImageType.ColorBayer:
				return 1;
			default:
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Opens the stream.
		/// </summary>
		private void Open()
		{
			lock (_nativeStreamInitLock)
			{
				NextFrameEvent.Reset();
				IntPtr hNextFrameEvent = NextFrameEvent.SafeWaitHandle.DangerousGetHandle();
				Sensor.NuiSensor.NuiImageStreamOpen((_NUI_IMAGE_TYPE)ImageType, (_NUI_IMAGE_RESOLUTION)Resolution, (uint)ImageStreamFlags, (uint)BufferCount, hNextFrameEvent, out _nativeStreamHandle);
			}
			_capturing = true;
		}

		/// <summary>
		/// Closes the stream.
		/// </summary>
		private void Close()
		{
			_capturing = false;
		}

		/// <summary>
		/// Override in base classes to store the pixel data during the window when it's available from 
		/// native code.  
		/// </summary>
		/// <param name="frameNumber">The frameNumber of the pixel data.</param>
		/// <param name="lockedRect">The native description of the pixel data.</param>
		/// <param name="imageType">The ImageType of the pixels.</param>
		/// <param name="resolution">The Resolution of the pixels.</param>
		/// <param name="nativeStreamHandle">The native stream handle.</param>
		/// <param name="pNativeFrame">The native image frame.</param>
		internal abstract void StorePixels(int frameNumber, _NUI_LOCKED_RECT lockedRect, ImageType imageType, ImageResolution resolution, IntPtr nativeStreamHandle, ref _NUI_IMAGE_FRAME pNativeFrame);

		/// <summary>
		/// Retrieves the next frame from the sensor, and calls the abstract method "StorePixels" with the data, if present.
		/// This allows the derived ImageStream classes to store the data.
		/// </summary>
		/// <param name="millisecondsWait">Time to wait for a new frame from the sensor, in milliseconds.</param>
		/// <param name="frameNumber">The frameNumber of the new frame.</param>
		/// <param name="timestamp">The timestamp of the new frame.</param>
		/// <param name="frameFlags">The frame flags of the new frame.</param>
		/// <returns>Returns true if the new frame was available, false otherwise.</returns>
		internal bool TryGetNextFrameInternal(int millisecondsWait, out int frameNumber, out long timestamp, out ImageFrameFlags frameFlags)
		{
			frameNumber = 0;
			timestamp = 0L;
			frameFlags = ImageFrameFlags.NUI_IMAGE_FRAME_FLAG_NONE;
			lock (_nativeStreamInitLock)
			{
				if (_nativeStreamHandle == IntPtr.Zero)
				{
					throw new InvalidOperationException(Resources.NativeStreamCantBeZero);
				}
				_NUI_IMAGE_FRAME pImageFrame;
				int num = Sensor.NuiSensor.NuiImageStreamGetNextFrameNoThrow(_nativeStreamHandle, (uint)millisecondsWait, out pImageFrame);
				if (num < 0)
				{
					NextFrameEvent.Reset();
					return false;
				}
				if (num != 0 || pImageFrame.pFrameTexture == null)
				{
					return false;
				}
				try
				{
					INuiFrameTexture pFrameTexture = pImageFrame.pFrameTexture;
					_NUI_SURFACE_DESC pDesc = default(_NUI_SURFACE_DESC);
					KinectExceptionHelper.CheckHr(pFrameTexture.GetLevelDesc(0u, ref pDesc));
					ImageType imageType = ImageType;
					ImageResolution resolution = Resolution;
					if (pImageFrame.eImageType != (_NUI_IMAGE_TYPE)imageType || pImageFrame.eResolution != (_NUI_IMAGE_RESOLUTION)resolution)
					{
						return false;
					}
					_NUI_LOCKED_RECT pLockedRect = default(_NUI_LOCKED_RECT);
					tagRECT pRect = default(tagRECT);
					KinectExceptionHelper.CheckHr(pFrameTexture.LockRect(0u, ref pLockedRect, ref pRect, 0u));
					frameNumber = (int)pImageFrame.dwFrameNumber;
					timestamp = pImageFrame.liTimeStamp.QuadPart;
					StorePixels(frameNumber, pLockedRect, imageType, resolution, _nativeStreamHandle, ref pImageFrame);
					KinectExceptionHelper.CheckHr(pFrameTexture.UnlockRect(0u));
					frameFlags = (ImageFrameFlags)pImageFrame.dwFrameFlags;
					return true;
				}
				finally
				{
					Sensor.NuiSensor.NuiImageStreamReleaseFrame(_nativeStreamHandle, ref pImageFrame);
				}
			}
		}

		/// <summary>
		/// Disposes the stream.
		/// </summary>
		internal void Dispose()
		{
			Close();
			lock (_nativeStreamInitLock)
			{
				if (NextFrameEvent != null)
				{
					NextFrameEvent.Close();
				}
			}
		}
	}
}
