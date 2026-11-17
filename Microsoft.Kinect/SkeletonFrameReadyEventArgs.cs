using System;

namespace Microsoft.Kinect
{
	/// <summary>
	/// The event arguments used when a skeletion frame is ready.
	/// </summary>
	public sealed class SkeletonFrameReadyEventArgs : EventArgs
	{
		/// <summary>
		/// The related skeleton stream.
		/// </summary>
		private readonly SkeletonStream _skeletonStream;

		/// <summary>
		/// The related frame number.
		/// </summary>
		private readonly int _frameNumber;

		/// <summary>
		/// The related timestamp.
		/// </summary>
		private readonly long _timestamp;

		/// <summary>
		/// The tracking mode in which the frame was captured.
		/// </summary>
		private readonly SkeletonTrackingMode _trackingMode;

		/// <summary>
		/// Indicates if the arguments are valid.
		/// </summary>
		private bool _isInvalid;

		/// <summary>
		/// Initializes a new instance of the SkeletonFrameReadyEventArgs class with a reference to the source Stream
		/// and the frame number and timestamp, which allows us to retrieve instance of SkeletonFrames on demand.
		/// </summary>
		/// <param name="skeletonStream">The source SkeletonStream.</param>
		/// <param name="frameNumber">The frameNumber for this event.</param>
		/// <param name="timestamp">The timestamp for this event.</param>
		/// <param name="trackingMode">The tracking mode in which the frame was captured.</param>
		/// <param name="isInvalid">Whether this instance is known a priori to be invalid.</param>
		private SkeletonFrameReadyEventArgs(SkeletonStream skeletonStream, int frameNumber, long timestamp, SkeletonTrackingMode trackingMode, bool isInvalid)
		{
			_skeletonStream = skeletonStream;
			_frameNumber = frameNumber;
			_timestamp = timestamp;
			_trackingMode = trackingMode;
			_isInvalid = isInvalid;
		}

		/// <summary>
		/// Create an instance of SkeletonFrameReadyEventArgs.
		/// This must only be called by the owning SkeletonStream, and only after the stream has 
		/// attempted to retrieve the native frame, otherwise OpenSkeletonFrame will fail.
		/// </summary>
		/// <param name="skeletonImageStream">The skeleton stream.</param>
		/// <param name="frameNumber">The frame number.</param>
		/// <param name="timestamp">The timestamp.</param>
		/// <param name="trackingMode">The tracking mode in which the frame was captured.</param>
		/// <returns>A new SkeletonFrameReadyEventArgs object.</returns>
		internal static SkeletonFrameReadyEventArgs Create(SkeletonStream skeletonImageStream, int frameNumber, long timestamp, SkeletonTrackingMode trackingMode)
		{
			return new SkeletonFrameReadyEventArgs(skeletonImageStream, frameNumber, timestamp, trackingMode, isInvalid: false);
		}

		/// <summary>
		/// Create an instance of SkeletonFrameReadyEventArgs which is known to be invalid.
		/// OpenSkeletonFrame will return null.
		/// </summary>
		/// <returns>An invalid SkeletonFrameReadyEventArgs object.</returns>
		internal static SkeletonFrameReadyEventArgs CreateInvalid()
		{
			return new SkeletonFrameReadyEventArgs(null, 0, 0L, SkeletonTrackingMode.Default, isInvalid: true);
		}

		/// <summary>
		/// Container for one frame's worth of skeleton sensor image data
		/// Can return null if the data is not available.
		/// Upon success, returns the SkeletonFrame corresponding to this event, which must be Disposed.
		/// </summary>
		/// <returns>A new SkeletonFrame.</returns>
		public SkeletonFrame OpenSkeletonFrame()
		{
			if (_isInvalid)
			{
				return null;
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedOpenFrameFromEventInfo(2);
			SkeletonFrame skeletonFrame = SkeletonFrame.Create(_skeletonStream, _frameNumber, _timestamp, _trackingMode);
			if (skeletonFrame == null)
			{
				_isInvalid = true;
			}
			return skeletonFrame;
		}
	}
}
