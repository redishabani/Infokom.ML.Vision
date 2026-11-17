using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct _NUI_SKELETON_BONE_ROTATION
	{
		public _Matrix4 rotationMatrix;

		public _Vector4 rotationQuaternion;
	}
}
