using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct _NUI_SURFACE_DESC
	{
		public uint Width;

		public uint Height;
	}
}
