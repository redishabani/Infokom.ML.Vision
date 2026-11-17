using System;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This MUST match the values defined in NUIImageCamera.h.
	/// </summary>
	[Flags]
	internal enum ImageStreamFlags : uint
	{
		/// <summary>
		/// Empty value.
		/// </summary>
		NUI_IMAGE_STREAM_FLAG_NONE = 0u,
		/// <summary>
		/// Return S_FALSE instead of E_NUI_FRAME_NO_DATA if NuiImageStreamGetNextFrame( ) doesn't have a frame ready and a timeout != INFINITE is used.
		/// </summary>
		NUI_IMAGE_STREAM_FLAG_SUPPRESS_NO_FRAME_DATA = 0x10000u,
		/// <summary>
		/// Set the depth stream to near mode.
		/// </summary>
		NUI_IMAGE_STREAM_FLAG_ENABLE_NEAR_MODE = 0x20000u,
		/// <summary>
		/// Use distinct values for depth values that are either too close, too far or unknown.
		/// </summary>
		NUI_IMAGE_STREAM_FLAG_DISTINCT_OVERFLOW_DEPTH_VALUES = 0x40000u
	}
}
