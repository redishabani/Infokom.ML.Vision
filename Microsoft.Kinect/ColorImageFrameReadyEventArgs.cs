using System;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Arguments for the ColorFrameReady events.
	/// </summary>
	public sealed class ColorImageFrameReadyEventArgs : EventArgs
	{
		/// <summary>
		/// The related color stream.
		/// </summary>
		private readonly ColorImageStream _colorImageStream;

		/// <summary>
		/// The related frame number.
		/// </summary>
		private readonly int _frameNumber;

		/// <summary>
		/// The related timestamp.
		/// </summary>
		private readonly long _timestamp;

		/// <summary>
		/// The frame flags.
		/// </summary>
		private readonly ImageFrameFlags _frameFlags;

		/// <summary>
		/// Indicates if the arguments are valid.
		/// </summary>
		private bool _isInvalid;

		/// <summary>
		/// Initializes a new instance of the ColorImageFrameReadyEventArgs class with a reference to the source Stream
		/// and the frame number and timestamp, which allows us to retrieve instance of ColorImageFrames on demand.
		/// </summary>
		/// <param name="colorImageStream">The source ColorImageStream.</param>
		/// <param name="frameNumber">The frameNumber for this event.</param>
		/// <param name="timestamp">The timestamp for this event.</param>
		/// <param name="frameFlags">The frame flags for this event.</param>
		/// <param name="isInvalid">Whether this instance is known a priori to be invalid.</param>
		private ColorImageFrameReadyEventArgs(ColorImageStream colorImageStream, int frameNumber, long timestamp, ImageFrameFlags frameFlags, bool isInvalid)
		{
			_colorImageStream = colorImageStream;
			_frameNumber = frameNumber;
			_timestamp = timestamp;
			_frameFlags = frameFlags;
			_isInvalid = isInvalid;
		}

		/// <summary>
		/// Create an instance of ColorImageFrameReadyEventArgs.
		/// This must only be called by the owning ColorImageStream, and only after the stream has 
		/// attempted to retrieve the native frame, otherwise OpenColorImageFrame will fail.
		/// </summary>
		/// <param name="colorImageStream">The color stream.</param>
		/// <param name="frameNumber">The frame number.</param>
		/// <param name="timestamp">The timestamp.</param>
		/// <param name="frameFlags">The frame flags.</param>
		/// <returns>A new ColorImageFrameReadyEventArgs object.</returns>
		internal static ColorImageFrameReadyEventArgs Create(ColorImageStream colorImageStream, int frameNumber, long timestamp, ImageFrameFlags frameFlags)
		{
			return new ColorImageFrameReadyEventArgs(colorImageStream, frameNumber, timestamp, frameFlags, isInvalid: false);
		}

		/// <summary>
		/// Create an instance of ColorImageFrameReadyEventArgs which is known to be invalid.
		/// OpenColorImageFrame will return null.
		/// </summary>
		/// <returns>An invalid ColorImageFrameReadyEventArgs.</returns>
		internal static ColorImageFrameReadyEventArgs CreateInvalid()
		{
			return new ColorImageFrameReadyEventArgs(null, 0, 0L, ImageFrameFlags.NUI_IMAGE_FRAME_FLAG_NONE, isInvalid: true);
		}

		/// <summary>
		/// Container for one frame's worth of color sensor image data
		/// Can return null if the data is not available.
		/// Upon success, returns the ColorImageFrame corresponding to this event, which must be Disposed.
		/// </summary>
		/// <returns>A new ColorImageFrame.</returns>
		public ColorImageFrame OpenColorImageFrame()
		{
			if (_isInvalid)
			{
				return null;
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedOpenFrameFromEventInfo(0);
			ColorImageFrame colorImageFrame = ColorImageFrame.Create(_colorImageStream, _frameNumber, _timestamp, _frameFlags);
			if (colorImageFrame == null)
			{
				_isInvalid = true;
			}
			return colorImageFrame;
		}
	}
}
