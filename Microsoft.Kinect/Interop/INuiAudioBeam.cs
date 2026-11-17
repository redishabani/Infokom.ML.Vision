using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[ComImport]
	[Guid("8C3CEBFA-A35D-497E-BC9A-E9752A8155E0")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface INuiAudioBeam
	{
		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int GetBeam(out double angle);

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int SetBeam([In] double angle);

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int GetPosition(out double angle, out double confidence);
	}
}
