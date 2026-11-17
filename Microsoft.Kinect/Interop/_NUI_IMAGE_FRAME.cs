using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct _NUI_IMAGE_FRAME
	{
		public _LARGE_INTEGER liTimeStamp;

		public uint dwFrameNumber;

		public _NUI_IMAGE_TYPE eImageType;

		public _NUI_IMAGE_RESOLUTION eResolution;

		[MarshalAs(UnmanagedType.Interface)]
		public INuiFrameTexture pFrameTexture;

		public uint dwFrameFlags;

		public _NUI_IMAGE_VIEW_AREA ViewArea;
	}
}
