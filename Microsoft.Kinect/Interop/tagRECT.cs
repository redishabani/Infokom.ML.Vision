using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct tagRECT
	{
		public int left;

		public int top;

		public int right;

		public int bottom;
	}
}
