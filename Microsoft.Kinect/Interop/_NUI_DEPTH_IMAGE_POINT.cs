using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct _NUI_DEPTH_IMAGE_POINT
	{
		public int x;

		public int y;

		public int depth;

		public int reserved;
	}
}
