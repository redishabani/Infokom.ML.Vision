using System;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Arguments for the Kinect sensor's AllFramesReady event.
	/// </summary>
	public sealed class AllFramesReadyEventArgs : EventArgs
	{
		/// <summary>
		/// The related color image frame event arguments.
		/// </summary>
		private readonly ColorImageFrameReadyEventArgs _colorArgs;

		/// <summary>
		/// The related depth image frame event arguments.
		/// </summary>
		private readonly DepthImageFrameReadyEventArgs _depthArgs;

		/// <summary>
		/// The related skeleton frame event arguments.
		/// </summary>
		private readonly SkeletonFrameReadyEventArgs _skeletonArgs;

		/// <summary>
		/// Initializes a new instance of the AllFramesReadyEventArgs class.
		/// </summary>
		/// <param name="colorArgs">The related color event arguments.</param>
		/// <param name="depthArgs">The related depth event arguments.</param>
		/// <param name="skeletonArgs">The related skeletion event arguments.</param>
		internal AllFramesReadyEventArgs(ColorImageFrameReadyEventArgs colorArgs, DepthImageFrameReadyEventArgs depthArgs, SkeletonFrameReadyEventArgs skeletonArgs)
		{
			_colorArgs = colorArgs;
			_depthArgs = depthArgs;
			_skeletonArgs = skeletonArgs;
		}

		/// <summary>
		/// The most recent frame from the Kinect sensor's color image stream.
		/// Can be null if the data is no longer available.  
		/// Upon success, the value must be Disposed.
		/// </summary>
		/// <returns>A new ColorImageFrame.</returns>
		public ColorImageFrame OpenColorImageFrame()
		{
			if (_colorArgs == null)
			{
				return null;
			}

			//TO DO: 
			//KinectEtwProvider.EventWriteManagedOpenFrameFromAllFramesEventInfo(0);
			return _colorArgs.OpenColorImageFrame();
		}

		/// <summary>
		/// The most recent frame from the Kinect sensor's depth image stream.
		/// Can be null if the data is no longer available.  
		/// Upon success, the value must be Disposed.
		/// </summary>
		/// <returns>A new DepthImageFrame.</returns>
		public DepthImageFrame OpenDepthImageFrame()
		{
			if (_depthArgs == null)
			{
				return null;
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedOpenFrameFromAllFramesEventInfo(1);
			return _depthArgs.OpenDepthImageFrame();
		}

		/// <summary>
		/// The most recent frame from the Kinect sensor's skeleton stream.
		/// Can be null if the data is no longer available.  
		/// Upon success, the value must be Disposed.
		/// </summary>
		/// <returns>A new SkeletonFrame.</returns>
		public SkeletonFrame OpenSkeletonFrame()
		{
			if (_skeletonArgs == null)
			{
				return null;
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedOpenFrameFromAllFramesEventInfo(2);
			return _skeletonArgs.OpenSkeletonFrame();
		}
	}
}
