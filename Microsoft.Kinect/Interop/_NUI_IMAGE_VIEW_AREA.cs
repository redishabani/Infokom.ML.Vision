using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct _NUI_IMAGE_VIEW_AREA
	{
		public int eDigitalZoom;

		public int lCenterX;

		public int lCenterY;
	}
}
