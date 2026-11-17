using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct _NUI_SKELETON_BONE_ORIENTATION
	{
		public _NUI_SKELETON_POSITION_INDEX endJoint;

		public _NUI_SKELETON_POSITION_INDEX startJoint;

		public _NUI_SKELETON_BONE_ROTATION hierarchicalRotation;

		public _NUI_SKELETON_BONE_ROTATION absoluteRotation;
	}
}
