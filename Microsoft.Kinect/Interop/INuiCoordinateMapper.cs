using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("618E3670-BA84-4405-898A-3FF64446157C")]
	internal interface INuiCoordinateMapper
	{
		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int GetColorToDepthRelationalParameters(out uint pDataByteCount, out IntPtr ppData);

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int NotifyParametersChanged([In][MarshalAs(UnmanagedType.Interface)] INuiCoordinateMapperParametersChangedEvent pCallback);

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int MapColorFrameToDepthFrame([In] _NUI_IMAGE_TYPE eColorType, [In] _NUI_IMAGE_RESOLUTION eColorResolution, [In] _NUI_IMAGE_RESOLUTION eDepthResolution, [In] uint cDepthPixels, [In] IntPtr pDepthPixels, [In] uint cDepthPoints, [In] IntPtr pDepthPoints);

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int MapColorFrameToSkeletonFrame([In] _NUI_IMAGE_TYPE eColorType, [In] _NUI_IMAGE_RESOLUTION eColorResolution, [In] _NUI_IMAGE_RESOLUTION eDepthResolution, [In] uint cDepthPixels, [In] IntPtr pDepthPixels, [In] uint cSkeletonPoints, [In] IntPtr pSkeletonPoints);

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int MapDepthFrameToColorFrame([In] _NUI_IMAGE_RESOLUTION eDepthResolution, [In] uint cDepthPixels, [In] IntPtr pDepthPixels, [In] _NUI_IMAGE_TYPE eColorType, [In] _NUI_IMAGE_RESOLUTION eColorResolution, [In] uint cColorPoints, [In] IntPtr pColorPoints);

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int MapDepthFrameToSkeletonFrame([In] _NUI_IMAGE_RESOLUTION eDepthResolution, [In] uint cDepthPixels, [In] IntPtr pDepthPixels, [In] uint cSkeletonPoints, [In] IntPtr pSkeletonPoints);

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int MapDepthPointToColorPoint([In] _NUI_IMAGE_RESOLUTION eDepthResolution, [In] ref _NUI_DEPTH_IMAGE_POINT pDepthPoint, [In] _NUI_IMAGE_TYPE eColorType, [In] _NUI_IMAGE_RESOLUTION eColorResolution, out _NUI_COLOR_IMAGE_POINT pColorPoint);

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int MapDepthPointToSkeletonPoint([In] _NUI_IMAGE_RESOLUTION eDepthResolution, [In] ref _NUI_DEPTH_IMAGE_POINT pDepthPoint, out _Vector4 pSkeletonPoint);

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int MapSkeletonPointToColorPoint([In] ref _Vector4 pSkeletonPoint, [In] _NUI_IMAGE_TYPE eColorType, [In] _NUI_IMAGE_RESOLUTION eColorResolution, out _NUI_COLOR_IMAGE_POINT pColorPoint);

		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[return: MarshalAs(UnmanagedType.Error)]
		int MapSkeletonPointToDepthPoint([In] ref _Vector4 pSkeletonPoint, [In] _NUI_IMAGE_RESOLUTION eDepthResolution, out _NUI_DEPTH_IMAGE_POINT pDepthPoint);
	}
}
