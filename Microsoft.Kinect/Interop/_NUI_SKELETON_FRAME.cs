using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct _NUI_SKELETON_FRAME
	{
		public _LARGE_INTEGER liTimeStamp;

		public uint dwFrameNumber;

		public uint dwFlags;

		public _Vector4 vFloorClipPlane;

		public _Vector4 vNormalToGravity;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		public _NUI_SKELETON_DATA[] SkeletonData;
	}
}
