using System;
using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Wrapper of a native filter to be applied to depth frame data.
	/// </summary>
	public class NuiDepthFilter : IDepthFilter
	{
		/// <summary>
		/// Gets or sets the native depth filter interface.
		/// </summary>
		public object NativeObject { get; set; }

		/// <summary>
		/// Processes a frame of depth data.
		/// </summary>
		/// <param name="timestamp">Timestamp of the depth frame.</param>
		/// <param name="width">Width of the depth frame, in pixels.</param>
		/// <param name="height">Height of the depth frame, in pixels.</param>
		/// <param name="pixelData">Depth frame data.</param>
		/// <returns>true if the depth frame was modified; false otherwise.</returns>
		public unsafe bool ProcessFrame(long timestamp, int width, int height, DepthImagePixel[] pixelData)
		{
			int pFrameModified = 0;
			INuiDepthFilter nuiDepthFilter = (INuiDepthFilter)NativeObject;
			if (nuiDepthFilter != null)
			{
				fixed (DepthImagePixel* ptr = pixelData)
				{
					IntPtr pDepthImagePixels = (IntPtr)ptr;
					KinectExceptionHelper.CheckHr(nuiDepthFilter.ProcessFrame(new _LARGE_INTEGER
					{
						QuadPart = timestamp
					}, (uint)width, (uint)height, pDepthImagePixels, out pFrameModified));
				}
			}
			return pFrameModified != 0;
		}
	}
}
