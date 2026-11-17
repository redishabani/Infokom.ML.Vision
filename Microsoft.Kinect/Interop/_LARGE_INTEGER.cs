using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct _LARGE_INTEGER
	{
		public long QuadPart;
	}
}
