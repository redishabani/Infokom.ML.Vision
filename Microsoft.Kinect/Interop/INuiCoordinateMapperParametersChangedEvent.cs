using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[ComImport]
	[Guid("BABEBBEC-C71B-4EE8-9F15-FC5C6C948622")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface INuiCoordinateMapperParametersChangedEvent
	{
		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int Invoke();
	}
}
