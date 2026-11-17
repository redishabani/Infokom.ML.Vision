using System;
using System.Runtime.InteropServices;
using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Represents the stream of Depth Image data from the Kinect.
	/// It should only be used as a property of a KinectSensor.
	/// </summary>
	public sealed class DepthImageStream : ImageStream
	{
		/// <summary>
		/// The default minimum depth.
		/// </summary>
		internal const int _DefaultMinDepth = 800;

		/// <summary>
		/// The default maximum depth.
		/// </summary>
		internal const int _DefaultMaxDepth = 4000;

		/// <summary>
		/// The near mode minimum depth.
		/// </summary>
		internal const int _NearMinDepth = 400;

		/// <summary>
		/// The near mode maximum depth.
		/// </summary>
		internal const int _NearMaxDepth = 3000;

		/// <summary>
		/// The value indicating too near.
		/// </summary>
		private const int _TooNearDepth = 0;

		/// <summary>
		/// The value indicating too far.
		/// </summary>
		private const int _TooFarDepth = 4095;

		/// <summary>
		/// The value for an unknown depth.
		/// </summary>
		private const int _UnknownDepth = -1;

		/// <summary>
		/// The depth bytes per pixel constant.
		/// </summary>
		private const int DepthBytesPerPixelConstant = 2;

		/// <summary>
		/// The current depth range mode.
		/// </summary>
		private DepthRange _range;

		/// <summary>
		/// The current minimum depth value.
		/// </summary>
		private int _minDepth;

		/// <summary>
		/// The current maximum depth value.
		/// </summary>
		private int _maxDepth;

		/// <summary>
		/// The current depth image format.
		/// </summary>
		private DepthImageFormat _depthImageFormat;

		/// <summary>
		/// The depth data pool.
		/// </summary>
		private readonly DataPool<int, ImageType, ImageResolution, short[], DepthImagePixel[]> _depthDataPool;

		/// <summary>
		/// Gets the nominal focal length of depth image in pixels, adjusted for resolution.
		/// </summary>
		public float NominalFocalLengthInPixels => ScaleValue(285.63f);

		/// <summary>
		/// Gets the inverse of the NominalFocalLengthInPixels, or 1/NominalFocalLengthInPixels.
		/// </summary>
		public float NominalInverseFocalLengthInPixels => 1f / NominalFocalLengthInPixels;

		/// <summary>
		/// Gets the nominal diagonal field of view of depth image.
		/// </summary>
		public float NominalDiagonalFieldOfView => ScaleValue(70f);

		/// <summary>
		/// Gets the nominal horizontal field of view of depth image.
		/// </summary>
		public float NominalHorizontalFieldOfView => ScaleValue(58.5f);

		/// <summary>
		/// Gets the nominal vertical field of view of depth image.
		/// </summary>
		public float NominalVerticalFieldOfView => ScaleValue(45.6f);

		/// <summary>
		/// Gets or sets the current sensitive range of the Depth Sensor.
		/// </summary>
		public DepthRange Range
		{
			get
			{
				return _range;
			}
			set
			{
				switch (value)
				{
				case DepthRange.Default:
					base.ImageStreamFlags &= ~ImageStreamFlags.NUI_IMAGE_STREAM_FLAG_ENABLE_NEAR_MODE;
					_range = value;
					_minDepth = 800;
					_maxDepth = 4000;
					break;
				case DepthRange.Near:
					base.ImageStreamFlags |= ImageStreamFlags.NUI_IMAGE_STREAM_FLAG_ENABLE_NEAR_MODE;
					_range = value;
					_minDepth = 400;
					_maxDepth = 3000;
					break;
				default:
					throw new ArgumentOutOfRangeException(Resources.UnexpectedDepthRange);
				}
			}
		}

		/// <summary>
		/// Gets the current Framerate, and Resolution.
		/// </summary>
		public DepthImageFormat Format
		{
			get
			{
				return _depthImageFormat;
			}
			private set
			{
				_depthImageFormat = value;
				base.Resolution = LookUpImageResolution(_depthImageFormat);
				FillHeightWidth();
				base.FramePixelDataLength = base.FrameWidth * base.FrameHeight;
			}
		}

		/// <summary>
		/// Gets the constant Depth value for data that is too near for the sensor to accurately track.
		/// This value is guaranteed to be smaller than MinDepth. Note that objects very near
		/// the sensor may actually return UnknownDepth.
		/// </summary>
		public int TooNearDepth => 0;

		/// <summary>
		/// Gets the constant Depth value for data that is too far beyond the sensor range.
		/// This value is guaranteed to be larger than MaxDepth.
		/// </summary>
		public int TooFarDepth => 4095;

		/// <summary>
		/// Gets the constant Depth value for data that is at an unknown distance.
		/// </summary>
		public int UnknownDepth => -1;

		/// <summary>
		/// Gets the minimum Depth value in mm for the current sensor range setting.
		/// </summary>
		public int MinDepth => _minDepth;

		/// <summary>
		/// Gets the maximum Depth value in mm for the current sensor range setting.
		/// </summary>
		public int MaxDepth => _maxDepth;

		/// <summary>
		/// This property can change based on external changes to Sensor.SkeletonStream,
		/// and therefore must be queried every time the value is required.
		/// </summary>
		internal override ImageType ImageType
		{
			get
			{
				if (base.Sensor.SkeletonStream.IsEnabled)
				{
					return ImageType.DepthAndPlayerIndex;
				}
				return ImageType.Depth;
			}
		}

		/// <summary>
		/// This method returns the resolution scale of the value provided.
		/// </summary>
		/// <param name="value">The source value.</param>
		/// <returns>The scaled value based on a 320x240 resolution.</returns>
		private float ScaleValue(float value)
		{
			return Format switch
			{
				DepthImageFormat.Resolution320x240Fps30 => value, 
				DepthImageFormat.Resolution640x480Fps30 => 2f * value, 
				DepthImageFormat.Resolution80x60Fps30 => 0.25f * value, 
				_ => value, 
			};
		}

		/// <summary>
		/// Initializes a new instance of the DepthImageStream class.
		/// </summary>
		/// <param name="mainNui">The related sensor object.</param>
		internal DepthImageStream(KinectSensor mainNui)
			: base(mainNui, ImageStreamFlags.NUI_IMAGE_STREAM_FLAG_SUPPRESS_NO_FRAME_DATA | ImageStreamFlags.NUI_IMAGE_STREAM_FLAG_DISTINCT_OVERFLOW_DEPTH_VALUES)
		{
			Range = DepthRange.Default;
			_depthDataPool = new DataPool<int, ImageType, ImageResolution, short[], DepthImagePixel[]>(base.BufferCount);
			Format = DepthImageFormat.Resolution640x480Fps30;
			base.FrameBytesPerPixel = 2;
		}

		/// <summary>
		/// Enables the Depth Image Stream for this Kinect Sensor using the default value of
		/// DepthImageFormat.Resolution640x480Fps30.
		/// </summary>
		public void Enable()
		{
			Enable(DepthImageFormat.Resolution640x480Fps30);
		}

		/// <summary>
		/// Enables the Depth Image Stream for this Kinect Sensor, with the specified format.
		/// </summary>
		/// <param name="format">The Framerate, and Resolution.</param>
		public void Enable(DepthImageFormat format)
		{
			if (format == DepthImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "format");
			}
			if (!base.Sensor.IsRunning || !base.IsEnabled || Format != format)
			{
				Format = format;
				InternalEnable();
			}
		}

		/// <summary>
		/// Retrieves the next DepthImageFrame from the sensor.  The DepthImageFrame must be disposed.
		/// </summary>
		/// <param name="millisecondsWait">Time to wait for a new frame from the sensor, in milliseconds.</param>
		/// <returns>The next DepthImageFrame, or null if the timeout has expired.</returns>
		public DepthImageFrame OpenNextFrame(int millisecondsWait)
		{
			DepthImageFrame result = null;
			if (!base.Sensor.IsRunning)
			{
				throw new InvalidOperationException(Resources.SensorMustBeRunning);
			}
			if (!base.IsEnabled)
			{
				throw new InvalidOperationException(Resources.DepthStreamMustBeEnabled);
			}
			if (base.Sensor.HasDepthInvocations)
			{
				throw new InvalidOperationException(Resources.CannotPollAndUseEvents);
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedOpenNextFrameInfo(1, millisecondsWait);
			if (TryGetNextFrameInternal(millisecondsWait, out var frameNumber, out var timestamp, out var frameFlags))
			{
				result = DepthImageFrame.Create(this, frameNumber, timestamp, frameFlags);
			}
			return result;
		}

		/// <summary>
		/// Store the pixel data during the window when it's available from native code.  
		/// </summary>
		/// <param name="frameNumber">The frameNumber of the pixel data.</param>
		/// <param name="lockedRect">The native description of the pixel data.</param>
		/// <param name="imageType">The ImageType of the pixels.</param>
		/// <param name="resolution">The Resolution of the pixels.</param>
		/// <param name="nativeStreamHandle">The native stream handle.</param>
		/// <param name="pNativeFrame">The native image frame.</param>
		internal unsafe override void StorePixels(int frameNumber, _NUI_LOCKED_RECT lockedRect, ImageType imageType, ImageResolution resolution, IntPtr nativeStreamHandle, ref _NUI_IMAGE_FRAME pNativeFrame)
		{
			DataPool<int, ImageType, ImageResolution, short[], DepthImagePixel[]>.Entry entry = _depthDataPool.CheckOutFreeEntryForUpdate();
			entry.Key = frameNumber;
			entry.Value1 = imageType;
			entry.Value2 = resolution;
			ImageStream.ResolutionToHeightWidth(resolution, out var width, out var height);
			int num = width * height;
			if (entry.Value3 == null || entry.Value3.Length != num)
			{
				entry.Value3 = new short[num];
			}
			if (entry.Value4 == null || entry.Value4.Length != num)
			{
				entry.Value4 = new DepthImagePixel[num];
			}
			if (num * 2 == lockedRect.size)
			{
				Marshal.Copy(lockedRect.pBits, entry.Value3, 0, entry.Value3.Length);
			}
			base.Sensor.NuiSensor.NuiImageFrameGetDepthImagePixelFrameTexture(nativeStreamHandle, ref pNativeFrame, out var nearMode, out var ppTexture);
			pNativeFrame.dwFrameFlags |= (uint)(nearMode ? 131072 : 0);
			if (ppTexture != null)
			{
				_NUI_LOCKED_RECT pLockedRect = default(_NUI_LOCKED_RECT);
				tagRECT pRect = default(tagRECT);
				KinectExceptionHelper.CheckHr(ppTexture.LockRect(0u, ref pLockedRect, ref pRect, 0u));
				fixed (DepthImagePixel* value = entry.Value4)
				{
					NativeMethods.CopyMemory((IntPtr)value, pLockedRect.pBits, num * sizeof(DepthImagePixel));
				}
			}
			_depthDataPool.CheckInEntryForUpdate(entry);
		}

		/// <summary>
		/// This method locks the pixel data for a given frame number.
		/// </summary>
		/// <param name="frameNumber">The frame number.</param>
		/// <returns>A locked pixel data if available.</returns>
		internal DataPool<int, ImageType, ImageResolution, short[], DepthImagePixel[]>.Entry LockPixelData(int frameNumber)
		{
			_depthDataPool.TryLockEntry(frameNumber, out var entry);
			return entry;
		}

		/// <summary>
		/// This method unlocks the pixel data.
		/// </summary>
		/// <param name="entry">The data to unlock.</param>
		internal void UnlockPixelData(DataPool<int, ImageType, ImageResolution, short[], DepthImagePixel[]>.Entry entry)
		{
			_depthDataPool.UnlockEntry(entry);
		}

		/// <summary>
		/// This method looks up the image resolution based on a given depth format.
		/// </summary>
		/// <param name="format">The depth format.</param>
		/// <returns>The ImageResolution for the DepthImageFormat.</returns>
		internal static ImageResolution LookUpImageResolution(DepthImageFormat format)
		{
			return format switch
			{
				DepthImageFormat.Resolution640x480Fps30 => ImageResolution.Resolution640x480, 
				DepthImageFormat.Resolution320x240Fps30 => ImageResolution.Resolution320x240, 
				DepthImageFormat.Resolution80x60Fps30 => ImageResolution.Resolution80x60, 
				_ => ImageResolution.Invalid, 
			};
		}

		/// <summary>
		/// This method looks up the DepthImageFormat for the given ImageType and ImageResolution.
		/// </summary>
		/// <param name="imageType">The image type.</param>
		/// <param name="imageResolution">The image resolution.</param>
		/// <returns>The DepthImageFormat for the combination.</returns>
		internal static DepthImageFormat LookUpDepthImageFormat(ImageType imageType, ImageResolution imageResolution)
		{
			if (imageType == ImageType.DepthAndPlayerIndex || imageType == ImageType.Depth)
			{
				switch (imageResolution)
				{
				case ImageResolution.Resolution80x60:
					return DepthImageFormat.Resolution80x60Fps30;
				case ImageResolution.Resolution320x240:
					return DepthImageFormat.Resolution320x240Fps30;
				case ImageResolution.Resolution640x480:
					return DepthImageFormat.Resolution640x480Fps30;
				}
			}
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Returns the required length for a pixel data array based on the provided DepthImageFormat.
		/// </summary>
		/// <param name="format">The DepthImageFormat for which to lookup the required pixel data array length.</param>
		/// <returns>The required length for a pixel data array based on the provided DepthImageFormat.</returns>
		internal static int LookUpPixelDataLength(DepthImageFormat format)
		{
			return format switch
			{
				DepthImageFormat.Resolution640x480Fps30 => 307200, 
				DepthImageFormat.Resolution320x240Fps30 => 76800, 
				DepthImageFormat.Resolution80x60Fps30 => 4800, 
				_ => 0, 
			};
		}
	}
}
