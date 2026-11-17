using System;
using System.Runtime.InteropServices;
using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Represents the stream of Color Image data from the Kinect.
	/// It should only be used as a property of a KinectSensor.
	/// </summary>
	public sealed class ColorImageStream : ImageStream
	{
		/// <summary>
		/// The current image type of this stream.
		/// </summary>
		private ImageType _imageType;

		/// <summary>
		/// The current image format of this stream.
		/// </summary>
		private ColorImageFormat _colorImageFormat;

		/// <summary>
		/// The related color data pool.
		/// </summary>
		private readonly DataPool<int, ImageType, ImageResolution, byte[]> _colorDataPool;

		/// <summary>
		/// The color camera settings field.
		/// </summary>
		private ColorCameraSettings _cameraSettings;

		/// <summary>
		/// Gets the nominal focal length of color image in pixels, adjusted for resolution.
		/// </summary>
		public float NominalFocalLengthInPixels => this.ScaleValue(531.15f);

		/// <summary>
		/// Gets the inverse of the NominalFocalLengthInPixels, or 1/NominalFocalLengthInPixels.
		/// </summary>
		public float NominalInverseFocalLengthInPixels => 1f / this.NominalFocalLengthInPixels;

		/// <summary>
		/// Gets the nominal diagonal field of view of color image.
		/// </summary>
		public float NominalDiagonalFieldOfView => this.ScaleValue(73.9f);

		/// <summary>
		/// Gets the nominal horizontal field of view of color image.
		/// </summary>
		public float NominalHorizontalFieldOfView => this.ScaleValue(62f);

		/// <summary>
		/// Gets the nominal vertical field of view of color image.
		/// </summary>
		public float NominalVerticalFieldOfView => this.ScaleValue(48.6f);

		/// <summary>
		/// Gets the color camera settings.
		/// </summary>
		public ColorCameraSettings CameraSettings
		{
			get
			{
				if (this._cameraSettings == null)
				{
					this._cameraSettings = new ColorCameraSettings(base.Sensor);
				}
				return this._cameraSettings;
			}
		}

		/// <summary>
		/// Gets the current Framerate and Resolution.
		/// </summary>
		public ColorImageFormat Format
		{
			get
			{
				return this._colorImageFormat;
			}
			private set
			{
				this._colorImageFormat = value;
				switch (this._colorImageFormat)
				{
				case ColorImageFormat.RgbResolution640x480Fps30:
				case ColorImageFormat.RgbResolution1280x960Fps12:
						this._imageType = ImageType.Color;
					base.FrameBytesPerPixel = 4;
					break;
				case ColorImageFormat.YuvResolution640x480Fps15:
						this._imageType = ImageType.ColorYuv;
					base.FrameBytesPerPixel = 4;
					break;
				case ColorImageFormat.RawYuvResolution640x480Fps15:
						this._imageType = ImageType.ColorYuvRaw;
					base.FrameBytesPerPixel = 2;
					break;
				case ColorImageFormat.InfraredResolution640x480Fps30:
						this._imageType = ImageType.ColorInfrared;
					base.FrameBytesPerPixel = 2;
					break;
				case ColorImageFormat.RawBayerResolution640x480Fps30:
				case ColorImageFormat.RawBayerResolution1280x960Fps12:
						this._imageType = ImageType.ColorBayer;
					base.FrameBytesPerPixel = 1;
					break;
				default:
					throw new ArgumentException("Invalid ColorImageFormat.", "value");
				}
				base.Resolution = LookUpImageResolution(this.Format);
				this.FillHeightWidth();
				base.FramePixelDataLength = base.FrameWidth * base.FrameHeight * base.FrameBytesPerPixel;
			}
		}

		/// <summary>
		/// Gets the ImageType for the stream.
		/// </summary>
		internal override ImageType ImageType => this._imageType;

		/// <summary>
		/// This method returns the resolution scale of the value provided.
		/// </summary>
		/// <param name="value">The source value.</param>
		/// <returns>The scaled value based on a 640x480 resolution.</returns>
		private float ScaleValue(float value)
		{
			if (this.Format == ColorImageFormat.RgbResolution1280x960Fps12 || this.Format == ColorImageFormat.RawBayerResolution1280x960Fps12)
			{
				return 2f * value;
			}
			return value;
		}

		/// <summary>
		/// Initializes a new instance of the ColorImageStream class.
		/// </summary>
		/// <param name="mainNui">The sensor this stream relates to.</param>
		internal ColorImageStream(KinectSensor mainNui) : base(mainNui)
		{
			this._colorDataPool = new DataPool<int, ImageType, ImageResolution, byte[]>(base.BufferCount);
			this.Format = ColorImageFormat.RgbResolution640x480Fps30;
		}

		/// <summary>
		/// Enables the Color Image Stream for this Kinect Sensor using the default of
		/// ColorImageFormat.RgbResolution640x480Fps30.
		/// </summary>
		public void Enable()
		{
			this.Enable(ColorImageFormat.RgbResolution640x480Fps30);
		}

		/// <summary>
		/// Enables the Color Image Stream for this Kinect Sensor, with the specified format.
		/// </summary>
		/// <param name="format">The Image Data format, Framerate, and Resolution.</param>
		public void Enable(ColorImageFormat format)
		{
			if (format == ColorImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "format");
			}
			if (!base.Sensor.IsRunning || !base.IsEnabled || this.Format != format)
			{
				this.Format = format;
				this.InternalEnable();
			}
		}

		/// <summary>
		/// Retrieves the next ColorImageFrame from the sensor.  The ColorImageFrame must be disposed.
		/// </summary>
		/// <param name="millisecondsWait">The time to wait for a new frame from the sensor, in milliseconds.</param>
		/// <returns>The next ColorImageFrame, or null if the timeout has expired.</returns>
		public ColorImageFrame OpenNextFrame(int millisecondsWait)
		{
			ColorImageFrame result = null;
			if (!base.Sensor.IsRunning)
			{
				throw new InvalidOperationException(Resources.SensorMustBeRunning);
			}
			if (!base.IsEnabled)
			{
				throw new InvalidOperationException(Resources.ColorStreamMustBeEnabled);
			}
			if (base.Sensor.HasColorInvocations)
			{
				throw new InvalidOperationException(Resources.CannotPollAndUseEvents);
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedOpenNextFrameInfo(0, millisecondsWait);
			if (this.TryGetNextFrameInternal(millisecondsWait, out var frameNumber, out var timestamp, out var frameFlags))
			{
				result = ColorImageFrame.Create(this, frameNumber, timestamp, frameFlags);
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
		internal override void StorePixels(int frameNumber, _NUI_LOCKED_RECT lockedRect, ImageType imageType, ImageResolution resolution, IntPtr nativeStreamHandle, ref _NUI_IMAGE_FRAME pNativeFrame)
		{
			DataPool<int, ImageType, ImageResolution, byte[], bool?>.Entry entry = this._colorDataPool.CheckOutFreeEntryForUpdate();
			entry.Key = frameNumber;
			entry.Value1 = imageType;
			entry.Value2 = resolution;
			ImageStream.ResolutionToHeightWidth(resolution, out var width, out var height);
			int num = ImageStream.ImageTypeToBytesPerPixel(imageType);
			int num2 = width * height * num;
			if (entry.Value3 == null || entry.Value3.Length != num2)
			{
				entry.Value3 = new byte[num2];
			}
			if (num2 == lockedRect.size)
			{
				Marshal.Copy(lockedRect.pBits, entry.Value3, 0, entry.Value3.Length);
			}
			this._colorDataPool.CheckInEntryForUpdate(entry);
		}

		/// <summary>
		/// This method locks the pixel data for a given frame number.
		/// </summary>
		/// <param name="frameNumber">The frame number.</param>
		/// <returns>A locked pixel data if available.</returns>
		internal DataPool<int, ImageType, ImageResolution, byte[], bool?>.Entry LockPixelData(int frameNumber)
		{
			this._colorDataPool.TryLockEntry(frameNumber, out var entry);
			return entry;
		}

		/// <summary>
		/// This method unlocks the pixel data.
		/// </summary>
		/// <param name="entry">The data to unlock.</param>
		internal void UnlockPixelData(DataPool<int, ImageType, ImageResolution, byte[], bool?>.Entry entry)
		{
			this._colorDataPool.UnlockEntry(entry);
		}

		/// <summary>
		/// This method looks up the image resolution based on a given color format.
		/// </summary>
		/// <param name="format">The color format.</param>
		/// <returns>The ImageResolution for the ColorImageFormat.</returns>
		internal static ImageResolution LookUpImageResolution(ColorImageFormat format)
		{
			switch (format)
			{
			case ColorImageFormat.RgbResolution640x480Fps30:
			case ColorImageFormat.YuvResolution640x480Fps15:
			case ColorImageFormat.RawYuvResolution640x480Fps15:
			case ColorImageFormat.InfraredResolution640x480Fps30:
			case ColorImageFormat.RawBayerResolution640x480Fps30:
				return ImageResolution.Resolution640x480;
			case ColorImageFormat.RgbResolution1280x960Fps12:
			case ColorImageFormat.RawBayerResolution1280x960Fps12:
				return ImageResolution.Resolution1280x960;
			default:
				return ImageResolution.Invalid;
			}
		}

		/// <summary>
		/// This method looks up the ColorImageFormat for the given ImageType and ImageResolution.
		/// </summary>
		/// <param name="imageType">The image type.</param>
		/// <param name="imageResolution">The image resolution.</param>
		/// <returns>The ColorImageFormat for the combination.</returns>
		internal static ColorImageFormat LookUpColorImageFormat(ImageType imageType, ImageResolution imageResolution)
		{
			switch (imageType)
			{
			case ImageType.Color:
				switch (imageResolution)
				{
				case ImageResolution.Resolution640x480:
					return ColorImageFormat.RgbResolution640x480Fps30;
				case ImageResolution.Resolution1280x960:
					return ColorImageFormat.RgbResolution1280x960Fps12;
				}
				break;
			case ImageType.ColorYuv:
				if (imageResolution == ImageResolution.Resolution640x480)
				{
					return ColorImageFormat.YuvResolution640x480Fps15;
				}
				break;
			case ImageType.ColorYuvRaw:
				if (imageResolution == ImageResolution.Resolution640x480)
				{
					return ColorImageFormat.RawYuvResolution640x480Fps15;
				}
				break;
			case ImageType.ColorInfrared:
				if (imageResolution == ImageResolution.Resolution640x480)
				{
					return ColorImageFormat.InfraredResolution640x480Fps30;
				}
				break;
			case ImageType.ColorBayer:
				switch (imageResolution)
				{
				case ImageResolution.Resolution640x480:
					return ColorImageFormat.RawBayerResolution640x480Fps30;
				case ImageResolution.Resolution1280x960:
					return ColorImageFormat.RawBayerResolution1280x960Fps12;
				}
				break;
			}
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Returns the required length for a pixel data array based on the provided ColorImageFormat.
		/// </summary>
		/// <param name="format">The ColorImageFormat for which to lookup the required pixel data array length.</param>
		/// <returns>The required length for a pixel data array based on the provided ColorImageFormat.</returns>
		internal static int LookUpPixelDataLength(ColorImageFormat format)
		{
			return LookUpImageResolution(format) switch
			{
				ImageResolution.Resolution640x480 => 307200, 
				ImageResolution.Resolution1280x960 => 1228800, 
				_ => 0, 
			};
		}
	}
}
