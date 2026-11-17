using System;
using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct _NUI_LOCKED_RECT
	{
		public int Pitch;

		public int size;

		public IntPtr pBits;
	}
}
