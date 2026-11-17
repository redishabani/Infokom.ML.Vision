using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct _NUI_COLOR_IMAGE_POINT
	{
		public int x;

		public int y;
	}
}
