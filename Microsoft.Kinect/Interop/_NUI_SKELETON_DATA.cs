using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct _NUI_SKELETON_DATA
	{
		public _NUI_SKELETON_TRACKING_STATE eTrackingState;

		public uint dwTrackingID;

		public uint dwEnrollmentIndex;

		public uint dwUserIndex;

		public _Vector4 Position;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		public _Vector4[] SkeletonPositions;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		public _NUI_SKELETON_POSITION_TRACKING_STATE[] eSkeletonPositionTrackingState;

		public uint dwQualityFlags;
	}
}
