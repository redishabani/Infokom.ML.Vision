using System;
using System.Runtime.InteropServices;

namespace Microsoft.Kinect
{
	/// <summary>
	/// A frame used specifically for depth images.  
	/// It provides access to the dimensions, format and pixel data for a depth frame,
	/// and allows for mapping of coordinates between skeleton frames and color frames.
	/// </summary>
	public sealed class DepthImageFrame : ImageFrame
	{
		/// <summary>
		/// A bitmask for extracting the player index bit field from the depth value.
		/// </summary>
		public const int PlayerIndexBitmask = 7;

		/// <summary>
		/// The width of the player index bitmask.
		/// </summary>
		public const int PlayerIndexBitmaskWidth = 3;

		/// <summary>
		/// The underlying depth image stream.
		/// </summary>
		private readonly DepthImageStream _depthImageStream;

		/// <summary>
		/// The underlying depth frame data.
		/// </summary>
		private DataPool<int, ImageType, ImageResolution, short[], DepthImagePixel[]>.Entry _frameData;

		/// <summary>
		/// The lock object used to synchronize access as necessary.
		/// </summary>
		private readonly object _dataAccessLock = new object();

		/// <summary>
		/// Gets this frame's Framerate and Resolution.
		/// </summary>
		public DepthImageFormat Format { get; private set; }

		/// <summary>
		/// Gets the depth sensor range with which this frame was captured.
		/// </summary>
		public DepthRange Range
		{
			get
			{
				if (!base.FrameFlags.HasFlag(ImageFrameFlags.NUI_IMAGE_FRAME_FLAG_NEAR_MODE_ENABLED))
				{
					return DepthRange.Default;
				}
				return DepthRange.Near;
			}
		}

		/// <summary>
		/// Gets the maximum reliable Depth value in mm for the depth sensor range setting used to capture this frame.
		/// </summary>
		public int MaxDepth
		{
			get
			{
				if (Range != DepthRange.Near)
				{
					return 4000;
				}
				return 3000;
			}
		}

		/// <summary>
		/// Gets the minimum reliable Depth value in mm for the depth sensor range setting used to capture this frame.
		/// </summary>
		public int MinDepth
		{
			get
			{
				if (Range != DepthRange.Near)
				{
					return 800;
				}
				return 400;
			}
		}

		/// <summary>
		/// Total length of the pixel data buffer of this ImageFrame.
		/// </summary>
		public override int PixelDataLength => DepthImageStream.LookUpPixelDataLength(Format);

		/// <summary>
		/// Gets the depth filter that was applied to the frame.
		/// </summary>
		public IDepthFilter DepthFilter => _depthImageStream.Sensor.NuiSensor.GetDepthFilterForTimestamp(base.Timestamp);

		/// <summary>
		/// The frame's source stream.
		/// </summary>
		internal override ImageStream SourceStream => _depthImageStream;

		/// <summary>
		/// Initializes a new instance of the DepthImageFrame class.
		/// </summary>
		/// <param name="depthImageStream">The related depth stream.</param>
		/// <param name="frameNumber">The current frame number.</param>
		/// <param name="timestamp">The current timestamp.</param>
		/// <param name="frameFlags">The current frame flags.</param>
		/// <param name="pixelData">The pixel data.</param>
		private DepthImageFrame(DepthImageStream depthImageStream, int frameNumber, long timestamp, ImageFrameFlags frameFlags, DataPool<int, ImageType, ImageResolution, short[], DepthImagePixel[]>.Entry pixelData)
			: base(pixelData.Value1, pixelData.Value2, frameNumber, timestamp, frameFlags)
		{
			_depthImageStream = depthImageStream;
			_frameData = pixelData;
			Format = DepthImageStream.LookUpDepthImageFormat(pixelData.Value1, pixelData.Value2);
		}

		/// <summary>
		/// Construct a DepthImageFrame for the frameNumber provided, if possible.
		/// If the data is not available for the specified frameNumber, this returns null.
		/// If this method succeeds, the result must be disposed.
		/// </summary>
		/// <param name="depthImageStream">The associated DepthImageStream.</param>
		/// <param name="frameNumber">The frameNumber to retrieve.</param>
		/// <param name="timestamp">The timestamp of the frame to retrieve.</param>
		/// <param name="frameFlags">The flags for the frame.</param>
		/// <returns>Null upon failure, a DepthImageStream upon success, which must be Disposed.</returns>
		internal static DepthImageFrame Create(DepthImageStream depthImageStream, int frameNumber, long timestamp, ImageFrameFlags frameFlags)
		{
			DepthImageFrame result = null;
			DataPool<int, ImageType, ImageResolution, short[], DepthImagePixel[]>.Entry entry = depthImageStream.LockPixelData(frameNumber);
			if (entry != null)
			{
				result = new DepthImageFrame(depthImageStream, frameNumber, timestamp, frameFlags, entry);
			}
			return result;
		}

		/// <summary>
		/// This method provides the DepthImagePixel array that contains the depth image's pixel data. 
		/// </summary>
		/// <returns>A DepthImagePixel array of the depth frame's pixel data.</returns>
		public DepthImagePixel[] GetRawPixelData()
		{
			return _frameData.Value4;
		}

		/// <summary>
		/// Private helper function used by CopyPixelDataTo and Copy DepthImagePixelDataTo.
		/// </summary>
		/// <typeparam name="T">Type of elements to copy.</typeparam>
		/// <param name="pixelData">Destination array.</param>
		/// <param name="copyFunction">Function to perform the copy.</param>
		private void CopyPixelDataTo<T>(T[] pixelData, Action<T[], int> copyFunction)
		{
			if (pixelData == null)
			{
				throw new ArgumentNullException("pixelData");
			}
			if (pixelData.Length != PixelDataLength)
			{
				throw new ArgumentException(Resources.PixelBufferIncorrectLength, "pixelData");
			}
			CopyPixelDataTo(pixelData, pixelData.Length, copyFunction);
		}

		/// <summary>
		/// Private helper function used by CopyPixelDataTo and Copy DepthImagePixelDataTo.
		/// </summary>
		/// <typeparam name="T">Type of data to copy.</typeparam>
		/// <param name="pixelData">Destination array.</param>
		/// <param name="pixelDataLength">The length of the destination array.</param>
		/// <param name="copyFunction">Function to perform the copy.</param>
		private void CopyPixelDataTo<T>(T pixelData, int pixelDataLength, Action<T, int> copyFunction)
		{
			if (pixelDataLength != PixelDataLength)
			{
				throw new ArgumentException(Resources.PixelBufferIncorrectLength, "pixelDataLength");
			}
			lock (_dataAccessLock)
			{
				if (_frameData == null)
				{
					throw new ObjectDisposedException("DepthImageFrame");
				}
				copyFunction(pixelData, pixelDataLength);
			}
		}

		/// <summary>
		/// This method copies the frame's pixel data to a pre-allocated pixel array.
		/// </summary>
		/// <param name="pixelData">The pixel array to receive the data.
		/// It must be exactly PixelDataLength in length.</param>
		public unsafe void CopyPixelDataTo(short[] pixelData)
		{
			CopyPixelDataTo(pixelData, delegate(short[] data, int length)
			{
				fixed (short* value = _frameData.Value3)
				{
					Marshal.Copy((IntPtr)value, data, 0, length);
				}
			});
		}

		/// <summary>
		/// This method copies the frame's pixel data to a pre-allocated pixel array.
		/// </summary>
		/// <param name="pixelData">The pixel array to receive the data.
		/// It must be exactly PixelDataLength in length.</param>
		public unsafe void CopyDepthImagePixelDataTo(DepthImagePixel[] pixelData)
		{
			CopyPixelDataTo(pixelData, delegate(DepthImagePixel[] data, int length)
			{
				fixed (DepthImagePixel* value = _frameData.Value4)
				{
					fixed (DepthImagePixel* ptr = data)
					{
						NativeMethods.CopyMemory((IntPtr)ptr, (IntPtr)value, length * sizeof(DepthImagePixel));
					}
				}
			});
		}

		/// <summary>
		/// This method copies the frame's pixel data to a pre-allocated byte array.
		/// </summary>
		/// <param name="pixelData">The IntPtr of the byte array.</param>
		/// <param name="pixelDataLength">The count of Int16s to copy to pixelData.  This must be equal to the frame’s PixelDataLength.</param>
		public void CopyPixelDataTo(IntPtr pixelData, int pixelDataLength)
		{
			if (pixelData == IntPtr.Zero)
			{
				throw new ArgumentNullException("pixelData");
			}
			CopyPixelDataTo(pixelData, pixelDataLength, delegate(IntPtr data, int length)
			{
				Marshal.Copy(_frameData.Value3, 0, data, length);
			});
		}

		/// <summary>
		/// This method copies the frame's DepthImagePixel data to a pre-allocated byte array.
		/// </summary>
		/// <param name="pixelData">The IntPtr of the byte array.</param>
		/// <param name="pixelDataLength">The count of DepthImagePixels to copy to pixelData.  This must be equal to the frame’s PixelDataLength.</param>
		public unsafe void CopyDepthImagePixelDataTo(IntPtr pixelData, int pixelDataLength)
		{
			if (pixelData == IntPtr.Zero)
			{
				throw new ArgumentNullException("pixelData");
			}
			CopyPixelDataTo(pixelData, pixelDataLength, delegate(IntPtr data, int length)
			{
				fixed (DepthImagePixel* value = _frameData.Value4)
				{
					NativeMethods.CopyMemory(data, (IntPtr)value, length * sizeof(DepthImagePixel));
				}
			});
		}

		/// <summary>
		/// This maps a depth coordinate to a color coordinate.
		/// </summary>
		/// <param name="depthX">The X coordinate of the depth frame.</param>
		/// <param name="depthY">The Y coordinate of the depth frame.</param>
		/// <param name="colorImageFormat">The color format being used.</param>
		/// <returns>An ImagePoint that contains the X, Y locations in the color frame.</returns>
		[Obsolete("This method is replaced by Microsoft.Kinect.CoordinateMapper.MapDepthPointToColorPoint", false)]
		public ColorImagePoint MapToColorImagePoint(int depthX, int depthY, ColorImageFormat colorImageFormat)
		{
			if (colorImageFormat == ColorImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "colorImageFormat");
			}
			short depthPixelValue = 0;
			if (depthX >= 0 && depthX < base.Width && depthY >= 0 && depthY < base.Height)
			{
				depthPixelValue = _frameData.Value3[depthY * base.Width + depthX];
			}
			return _depthImageStream.Sensor.MapDepthToColorImagePoint(Format, depthX, depthY, depthPixelValue, colorImageFormat);
		}

		/// <summary>
		/// Looks up the depth frame coordinates for a given skeleton point.
		/// </summary>
		/// <param name="skeletonPoint">The supplied skeleton point.</param>
		/// <returns>The ImagePoint that contains the X, Y and depth value of the given skeleton point.</returns>
		[Obsolete("This method is replaced by Microsoft.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint", false)]
		public DepthImagePoint MapFromSkeletonPoint(SkeletonPoint skeletonPoint)
		{
			return _depthImageStream.Sensor.MapSkeletonPointToDepth(skeletonPoint, Format);
		}

		/// <summary>
		/// Looks up the skeleton point location of the given depth X, Y.
		/// </summary>
		/// <param name="depthX">The X coordinate of the depth frame.</param>
		/// <param name="depthY">The Y coordinate of the depth frame.</param>
		/// <returns>The skeleton point for the given X, Y.</returns>
		[Obsolete("This method is replaced by Microsoft.Kinect.CoordinateMapper.MapDepthPointToSkeletonPoint", false)]
		public SkeletonPoint MapToSkeletonPoint(int depthX, int depthY)
		{
			short depthPixelValue = 0;
			if (depthX >= 0 && depthX < base.Width && depthY >= 0 && depthY < base.Height)
			{
				depthPixelValue = _frameData.Value3[depthY * base.Width + depthX];
			}
			return _depthImageStream.Sensor.MapDepthToSkeletonPoint(Format, depthX, depthY, depthPixelValue);
		}

		/// <summary>
		/// Disposes the frame object.
		/// </summary>
		/// <param name="disposing">Specify true to indicate that the class should clean up all resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (_dataAccessLock)
				{
					_depthImageStream.UnlockPixelData(_frameData);
					_frameData = null;
				}
			}
		}
	}
}
