using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal struct _NUI_DEPTH_IMAGE_PIXEL
	{
		public ushort playerIndex;

		public ushort depth;
	}
}
