using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[ComImport]
	[Guid("13EA17F5-FF2E-4670-9EE5-1297A6E880D1")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface INuiFrameTexture
	{
		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		int BufferLen();

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		int Pitch();

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int LockRect(uint Level, ref _NUI_LOCKED_RECT pLockedRect, ref tagRECT pRect, uint Flags);

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int GetLevelDesc(uint Level, ref _NUI_SURFACE_DESC pDesc);

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int UnlockRect([In] uint Level);
	}
}
