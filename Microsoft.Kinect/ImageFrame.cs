using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Microsoft.Kinect
{
	/// <summary>
	/// A container for per-frame sensor data buffers.
	/// </summary>
	public abstract class ImageFrame : IDisposable
	{
		/// <summary>
		/// Tracks whether this instance is disposed.  0 == false, 1 == true.
		/// We need to use int (vs. bool) for Interlocked.Exchange.
		/// </summary>
		private int _isDisposed;

		/// <summary>
		/// Gets the time of this ImageFrame.
		/// </summary>
		public long Timestamp { get; private set; }

		/// <summary>
		/// Gets the frame identification.
		/// </summary>
		public int FrameNumber { get; private set; }

		/// <summary>
		/// Gets the width in pixels of this ImageFrame.
		/// </summary>
		public int Width { get; private set; }

		/// <summary>
		/// Gets the height in pixels of this ImageFrame.
		/// </summary>
		public int Height { get; private set; }

		/// <summary>
		/// Gets the bytes per pixel of this ImageFrame.
		/// </summary>
		public int BytesPerPixel { get; private set; }

		/// <summary>
		/// Gets the total length of the pixel data buffer of this ImageFrame.
		/// </summary>
		public abstract int PixelDataLength { get; }

		/// <summary>
		/// Gets the frame's source stream.
		/// </summary>
		internal abstract ImageStream SourceStream { get; }

		/// <summary>
		/// Gets the frame's flags.
		/// </summary>
		internal ImageFrameFlags FrameFlags { get; private set; }

		/// <summary>
		/// Initializes a new instance of the ImageFrame class that represents this frame's data.
		/// </summary>
		/// <param name="imageType">The ImageType for this frame.</param>
		/// <param name="imageResolution">The ImageResolution for this frame.</param>
		/// <param name="frameNumber">The frame number of this frame.</param>
		/// <param name="timestamp">The timestamp of this frame.</param>
		/// <param name="frameFlags">The frame flags of this frame.</param>
		internal ImageFrame(ImageType imageType, ImageResolution imageResolution, int frameNumber, long timestamp, ImageFrameFlags frameFlags)
		{
			ImageStream.ResolutionToHeightWidth(imageResolution, out var width, out var height);
			int bytesPerPixel = ImageStream.ImageTypeToBytesPerPixel(imageType);
			Width = width;
			Height = height;
			BytesPerPixel = bytesPerPixel;
			FrameNumber = frameNumber;
			Timestamp = timestamp;
			FrameFlags = frameFlags;
		}

		/// <summary>
		/// Disposes the image frame.
		/// </summary>
		public void Dispose()
		{
			if (Interlocked.Exchange(ref _isDisposed, 1) == 0)
			{
				Dispose(disposing: true);
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Finalizes an instance of the ImageFrame class.
		/// This is used to notify the developer that they forgot to dispose the frame.
		/// </summary>
		~ImageFrame()
		{
			Debugger.Log(0, "Performance", string.Format(CultureInfo.InvariantCulture, "{0}\n", new object[1] { Resources.ImageFrameNotDisposed }));
			Dispose(disposing: false);
		}

		/// <summary>
		/// To be implemented by derived classes to return data to free lists.
		/// </summary>
		/// <param name="disposing">Specify true to indicate that the class should clean up all resources.</param>
		protected abstract void Dispose(bool disposing);
	}
}
