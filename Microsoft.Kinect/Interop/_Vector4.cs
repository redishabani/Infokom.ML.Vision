using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct _Vector4
	{
		public float x;

		public float y;

		public float z;

		public float w;
	}
}
