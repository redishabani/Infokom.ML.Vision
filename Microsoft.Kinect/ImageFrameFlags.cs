using System;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This MUST match the values defined in NUIImageCamera.h.
	/// </summary>
	[Flags]
	internal enum ImageFrameFlags : uint
	{
		/// <summary>
		/// Empty value.
		/// </summary>
		NUI_IMAGE_FRAME_FLAG_NONE = 0u,
		/// <summary>
		/// Depth frame was captured in near mode.
		/// </summary>
		NUI_IMAGE_FRAME_FLAG_NEAR_MODE_ENABLED = 0x20000u
	}
}
