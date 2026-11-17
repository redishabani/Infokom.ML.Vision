using System;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Arguments for the DepthFrameReady events.
	/// </summary>
	public sealed class DepthImageFrameReadyEventArgs : EventArgs
	{
		/// <summary>
		/// The related depth stream.
		/// </summary>
		private readonly DepthImageStream _depthImageStream;

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
		/// Initializes a new instance of the DepthImageFrameReadyEventArgs class with a reference to the source Stream
		/// and the frame number and timestamp, which allows us to retrieve instance of DepthImageFrames on demand.
		/// </summary>
		/// <param name="depthImageStream">The source DepthImageStream.</param>
		/// <param name="frameNumber">The frameNumber for this event.</param>
		/// <param name="timestamp">The timestamp for this event.</param>
		/// <param name="frameFlags">The frame flags for this event.</param>
		/// <param name="isInvalid">Whether this instance is known a priori to be invalid.</param>
		private DepthImageFrameReadyEventArgs(DepthImageStream depthImageStream, int frameNumber, long timestamp, ImageFrameFlags frameFlags, bool isInvalid)
		{
			_depthImageStream = depthImageStream;
			_frameNumber = frameNumber;
			_timestamp = timestamp;
			_frameFlags = frameFlags;
			_isInvalid = isInvalid;
		}

		/// <summary>
		/// Create an instance of DepthImageFrameReadyEventArgs.
		/// This must only be called by the owning DepthImageStream, and only after the stream has 
		/// attempted to retrieve the native frame, otherwise OpenDepthImageFrame will fail.
		/// </summary>
		/// <param name="depthImageStream">The depth image stream.</param>
		/// <param name="frameNumber">The frame number.</param>
		/// <param name="timestamp">The timestamp.</param>
		/// <param name="frameFlags">The frame flags.</param>
		/// <returns>A new DepthImageFrameReadyEventArgs object.</returns>
		internal static DepthImageFrameReadyEventArgs Create(DepthImageStream depthImageStream, int frameNumber, long timestamp, ImageFrameFlags frameFlags)
		{
			return new DepthImageFrameReadyEventArgs(depthImageStream, frameNumber, timestamp, frameFlags, isInvalid: false);
		}

		/// <summary>
		/// Create an instance of DepthImageFrameReadyEventArgs which is known to be invalid.
		/// OpenDepthImageFrame will return null.
		/// </summary>
		/// <returns>An invalid event arguments.</returns>
		internal static DepthImageFrameReadyEventArgs CreateInvalid()
		{
			return new DepthImageFrameReadyEventArgs(null, 0, 0L, ImageFrameFlags.NUI_IMAGE_FRAME_FLAG_NONE, isInvalid: true);
		}

		/// <summary>
		/// Container for one frame's worth of depth sensor image data
		/// Can return null if the data is not available.
		/// Upon success, returns the DepthImageFrame corresponding to this event, which must be Disposed.
		/// </summary>
		/// <returns>An a new depth image frame.</returns>
		public DepthImageFrame OpenDepthImageFrame()
		{
			if (_isInvalid)
			{
				return null;
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedOpenFrameFromEventInfo(1);
			DepthImageFrame depthImageFrame = DepthImageFrame.Create(_depthImageStream, _frameNumber, _timestamp, _frameFlags);
			if (depthImageFrame == null)
			{
				_isInvalid = true;
			}
			return depthImageFrame;
		}
	}
}
