using System;
using System.Runtime.InteropServices;

namespace Microsoft.Kinect
{
	/// <summary>
	/// A frame used specifically for color images.
	/// It provides access to the dimensions, format and pixel data for a color frame.
	/// </summary>
	public sealed class ColorImageFrame : ImageFrame
	{
		/// <summary>
		/// The underlying stream object.
		/// </summary>
		private readonly ColorImageStream _colorImageStream;

		/// <summary>
		/// The related frame data.
		/// </summary>
		private DataPool<int, ImageType, ImageResolution, byte[], bool?>.Entry _frameData;

		/// <summary>
		/// The lock object used to synchronize access as necessary.
		/// </summary>
		private readonly object _dataAccessLock = new object();

		/// <summary>
		/// Gets this frame's Framerate and Resolution.
		/// </summary>
		public ColorImageFormat Format { get; private set; }

		/// <summary>
		/// Gets the total length of the pixel data buffer of this ImageFrame.
		/// </summary>
		public override int PixelDataLength => base.Width * base.Height * base.BytesPerPixel;

		/// <summary>
		/// The frame's source stream.
		/// </summary>
		internal override ImageStream SourceStream => this._colorImageStream;

		/// <summary>
		/// Initializes a new instance of the ColorImageFrame class.
		/// </summary>
		/// <param name="colorImageStream">The underlying stream object.</param>
		/// <param name="frameNumber">The current frame number.</param>
		/// <param name="timestamp">The timestamp of the frame.</param>
		/// <param name="frameFlags">The frame flags.</param>
		/// <param name="pixelData">The actual pixel data.</param>
		private ColorImageFrame(ColorImageStream colorImageStream, int frameNumber, long timestamp, ImageFrameFlags frameFlags, DataPool<int, ImageType, ImageResolution, byte[], bool?>.Entry pixelData)
			: base(pixelData.Value1, pixelData.Value2, frameNumber, timestamp, frameFlags)
		{
			this._colorImageStream = colorImageStream;
			this._frameData = pixelData;
			this.Format = ColorImageStream.LookUpColorImageFormat(pixelData.Value1, pixelData.Value2);
		}

		/// <summary>
		/// Construct a ColorImageFrame for the frameNumber provided, if possible.
		/// If the data is not available for the specified frameNumber, this returns null.
		/// If this method succeeds, the result must be disposed.
		/// </summary>
		/// <param name="colorImageStream">The associated ColorImageStream.</param>
		/// <param name="frameNumber">The frameNumber to retrieve.</param>
		/// <param name="timestamp">The timestamp of the frame to retrieve.</param>
		/// <param name="frameFlags">The frame flags of the frame.</param>
		/// <returns>Null upon failure, a ColorImageStream upon success, which must be Disposed.</returns>
		internal static ColorImageFrame Create(ColorImageStream colorImageStream, int frameNumber, long timestamp, ImageFrameFlags frameFlags)
		{
			ColorImageFrame result = null;
			DataPool<int, ImageType, ImageResolution, byte[], bool?>.Entry entry = colorImageStream.LockPixelData(frameNumber);
			if (entry != null)
			{
				result = new ColorImageFrame(colorImageStream, frameNumber, timestamp, frameFlags, entry);
			}
			return result;
		}

		/// <summary>
		/// This method provides the raw array that contains the color frame's pixel data.
		/// </summary>
		/// <returns>A byte array of the color frame's pixel data.</returns>
		public byte[] Data => this._frameData.Value3;

		/// <summary>
		/// This method copies the frame's pixel data to a pre-allocated byte array.
		/// </summary>
		/// <param name="target">The byte array to receive the data.
		/// It must be exactly PixelDataLength in length.</param>
		public unsafe void WriteTo(byte[] target)
		{
			ArgumentNullException.ThrowIfNull(target);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(target.Length, this.PixelDataLength);


			lock (this._dataAccessLock)
			{
				if (this._frameData == null)
					throw new ObjectDisposedException("ColorImageFrame");

				fixed (byte* value = this._frameData.Value3)
				{
					Marshal.Copy((IntPtr)value, target, 0, target.Length);
				}
			}
		}


		public void WriteTo(Array target)
		{
			ArgumentNullException.ThrowIfNull(target);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(target.Length, this.PixelDataLength);


			lock (this._dataAccessLock)
			{
				if (this._frameData == null)
				{
					throw new ObjectDisposedException("ColorImageFrame");
				}

				Marshal.Copy(this._frameData.Value3, 0, Marshal.UnsafeAddrOfPinnedArrayElement(target, 0), this.PixelDataLength);
			}
		}

		/// <summary>
		/// This method copies the frame's pixel data to a pre-allocated byte array.
		/// </summary>
		/// <param name="target">The IntPtr of the byte array.</param>
		/// <param name="length">The count of Bytes to copy to pixelData.  This must be equal to the frame’s PixelDataLength.</param>
		public void WriteTo(IntPtr target, int length)
		{
			ArgumentOutOfRangeException.ThrowIfEqual(target, 0, nameof(target));
			ArgumentOutOfRangeException.ThrowIfGreaterThan(length, this.PixelDataLength);
			lock (this._dataAccessLock)
			{
				if (this._frameData == null)
				{
					throw new ObjectDisposedException("ColorImageFrame");
				}
				Marshal.Copy(this._frameData.Value3, 0, target, length);
			}
		}


		/// <summary>
		/// Disposes the frame object.
		/// </summary>
		/// <param name="disposing">Specify true to indicate that the class should clean up all resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (this._dataAccessLock)
				{
					this._colorImageStream.UnlockPixelData(this._frameData);
					this._frameData = null;
				}
			}
		}
	}
}
