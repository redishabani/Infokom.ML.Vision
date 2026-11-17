using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[ComImport]
	[Guid("1D7C07DD-2304-49BB-9B7F-2FDC6E00C1B2")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface INuiDepthFilter
	{
		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int ProcessFrame([In] _LARGE_INTEGER liTimeStamp, [In] uint Width, [In] uint Height, [In] IntPtr pDepthImagePixels, out int pFrameModified);
	}
}
