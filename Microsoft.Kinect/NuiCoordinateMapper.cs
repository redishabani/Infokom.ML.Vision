using System;
using System.Runtime.InteropServices;
using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// A class for performing mapping between depth space, color space, and skeleton space.
	/// </summary>
	internal class NuiCoordinateMapper : INuiCoordinateMapperParametersChangedEvent
	{
		/// <summary>
		/// Privately cached copy of INuiCoordinateMapper interface.
		/// </summary>
		private INuiCoordinateMapper _nuiCoordinateMapper;

		/// <summary>
		/// Event variable for RelationalParametersChanged.  Calls marshalled to main thread.
		/// </summary>
		private ContextEventHandler<EventArgs> _relationalParametersChangedHandler;

		/// <summary>
		/// Fires when the paramaters have been changed (e.g. by KinectStudio).
		/// </summary>
		public event EventHandler<EventArgs> RelationalParametersChanged
		{
			add
			{
				_relationalParametersChangedHandler.AddHandler(value);
			}
			remove
			{
				_relationalParametersChangedHandler.RemoveHandler(value);
			}
		}

		/// <summary>
		/// Initializes a new instance of the NuiCoordinateMapper class.
		/// </summary>
		/// <param name="nuiCoordinateMapper">The INuiCoordinateMapper interface to wrap.</param>
		public NuiCoordinateMapper(INuiCoordinateMapper nuiCoordinateMapper)
		{
			_nuiCoordinateMapper = nuiCoordinateMapper;
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the NuiCoordinateMapper class.
		/// </summary>
		/// <param name="parameters">The INuiCoordinateMapper interface to wrap.</param>
		public unsafe NuiCoordinateMapper(byte[] parameters)
		{
			fixed (byte* ptr = parameters)
			{
				KinectExceptionHelper.CheckHr(NativeMethods.NuiCreateCoordinateMapperFromParameters((uint)parameters.Length, (IntPtr)ptr, out _nuiCoordinateMapper));
			}
			Init();
		}

		/// <summary>
		/// Common constructor functionality.
		/// </summary>
		internal void Init()
		{
			_relationalParametersChangedHandler = new ContextEventHandler<EventArgs>();
			KinectExceptionHelper.CheckHr(_nuiCoordinateMapper.NotifyParametersChanged(this));
		}

		/// <summary>
		/// Returns the relational parameters from native and copys them into a byte array.
		/// </summary>
		/// <returns>The relational parameters.</returns>
		public byte[] GetColorToDepthRelationalParameters()
		{
			IntPtr ppData = IntPtr.Zero;
			try
			{
				KinectExceptionHelper.CheckHr(_nuiCoordinateMapper.GetColorToDepthRelationalParameters(out var pDataByteCount, out ppData));
				byte[] array = new byte[pDataByteCount];
				Marshal.Copy(ppData, array, 0, (int)pDataByteCount);
				return array;
			}
			finally
			{
				Marshal.FreeCoTaskMem(ppData);
			}
		}

		/// <summary>
		/// Gets the pixel coordinates in color space that correspond to the specified pixel coordinates in depth space, using the specified depth resolution. 
		/// </summary>
		/// <param name="eDepthResolution">The resolution of the depth image, as a NUI_IMAGE_RESOLUTION enumeration constant.</param>
		/// <param name="depthPoint">The DepthImagePoint to map to color space.</param>
		/// <param name="eColorType">Color type.</param>
		/// <param name="eColorResolution">The resolution of the color image, as a NUI_IMAGE_RESOLUTION enumeration constant.</param>
		/// <param name="colorPoint">The ColorImagePoint to recieve the mapped value.</param>
		public void MapDepthPointToColorPoint(_NUI_IMAGE_RESOLUTION eDepthResolution, ref _NUI_DEPTH_IMAGE_POINT depthPoint, _NUI_IMAGE_TYPE eColorType, _NUI_IMAGE_RESOLUTION eColorResolution, out _NUI_COLOR_IMAGE_POINT colorPoint)
		{
			KinectExceptionHelper.CheckHr(_nuiCoordinateMapper.MapDepthPointToColorPoint(eDepthResolution, ref depthPoint, eColorType, eColorResolution, out colorPoint));
		}

		/// <summary>
		/// Converts a frame's worth of data from depth space to color space. 
		/// </summary>
		/// <param name="eDepthResolution">The resolution of the depth image, as a NUI_IMAGE_RESOLUTION enumeration constant.</param>
		/// <param name="depthPixels">The data from an entire depth frame .</param>
		/// <param name="eColorType">Color type.</param>
		/// <param name="eColorResolution">The resolution of the color image, as a NUI_IMAGE_RESOLUTION enumeration constant.</param>
		/// <param name="colorPoints">Array to hold the color frame X,Y coordinates for each pixel in the depth frame.</param>
		public unsafe void MapDepthFrameToColorFrame(_NUI_IMAGE_RESOLUTION eDepthResolution, DepthImagePixel[] depthPixels, _NUI_IMAGE_TYPE eColorType, _NUI_IMAGE_RESOLUTION eColorResolution, ColorImagePoint[] colorPoints)
		{
			fixed (DepthImagePixel* ptr = depthPixels)
			{
				fixed (ColorImagePoint* ptr2 = colorPoints)
				{
					KinectExceptionHelper.CheckHr(_nuiCoordinateMapper.MapDepthFrameToColorFrame(eDepthResolution, (uint)depthPixels.Length, (IntPtr)ptr, eColorType, eColorResolution, (uint)colorPoints.Length, (IntPtr)ptr2));
				}
			}
		}

		/// <summary>
		/// Gets the pixel coordinates in skeleton space that correspond to the specified pixel in color space, using the provided color and depth resolutions.
		/// </summary>
		/// <param name="eColorType">Color type.</param>
		/// <param name="eColorResolution">The resolution of the color image, as a NUI_IMAGE_RESOLUTION enumeration constant.</param>
		/// <param name="eDepthResolution">The resolution of the depth image, as a NUI_IMAGE_RESOLUTION enumeration constant.</param>
		/// <param name="depthPixels">The input array of DepthImagePixels to map from, size should be equal to that specified by eDepthResolution.</param>
		/// <param name="skeletonPoints">The output array of SkeletonPoints, size should be equal to that specified by eColorResolution.</param>
		public unsafe void MapColorFrameToSkeletonFrame(_NUI_IMAGE_TYPE eColorType, _NUI_IMAGE_RESOLUTION eColorResolution, _NUI_IMAGE_RESOLUTION eDepthResolution, DepthImagePixel[] depthPixels, SkeletonPoint[] skeletonPoints)
		{
			fixed (DepthImagePixel* ptr = depthPixels)
			{
				fixed (SkeletonPoint* ptr2 = skeletonPoints)
				{
					KinectExceptionHelper.CheckHr(_nuiCoordinateMapper.MapColorFrameToSkeletonFrame(eColorType, eColorResolution, eDepthResolution, (uint)depthPixels.Length, (IntPtr)ptr, (uint)skeletonPoints.Length, (IntPtr)ptr2));
				}
			}
		}

		/// <summary>
		/// Gets the pixel coordinates in skeleton space that correspond to the specified pixel in depth space, using the provided depth resolution.
		/// </summary>
		/// <param name="eDepthResolution">The resolution of the depth image, as a NUI_IMAGE_RESOLUTION enumeration constant.</param>
		/// <param name="depthPixels">The input array of DepthImagePixels to map from, size should be equal to that specified by eDepthResolution.</param>
		/// <param name="skeletonPoints">The output array of SkeletonPoints, size should be equal to that specified by eDepthResolution.</param>
		public unsafe void MapDepthFrameToSkeletonFrame(_NUI_IMAGE_RESOLUTION eDepthResolution, DepthImagePixel[] depthPixels, SkeletonPoint[] skeletonPoints)
		{
			fixed (DepthImagePixel* ptr = depthPixels)
			{
				fixed (SkeletonPoint* ptr2 = skeletonPoints)
				{
					KinectExceptionHelper.CheckHr(_nuiCoordinateMapper.MapDepthFrameToSkeletonFrame(eDepthResolution, (uint)depthPixels.Length, (IntPtr)ptr, (uint)skeletonPoints.Length, (IntPtr)ptr2));
				}
			}
		}

		/// <summary>
		/// Gets the pixel coordinates in depth space that correspond to the specified pixel coordinates in color space, using the provided color and depth resolutions.
		/// </summary>
		/// <param name="eColorType">Color type.</param>
		/// <param name="eColorResolution">The resolution of the color image, as a NUI_IMAGE_RESOLUTION enumeration constant.</param>
		/// <param name="eDepthResolution">The resolution of the depth image, as a NUI_IMAGE_RESOLUTION enumeration constant.</param>
		/// <param name="depthPixels">The input array of DepthImagePixels to map from, size should be equal to that specified by eDepthResolution.</param>
		/// <param name="depthPoints">The output array of DepthImagePoints, size should be equal to that specified by eColorResolution.</param>
		public unsafe void MapColorFrameToDepthFrame(_NUI_IMAGE_TYPE eColorType, _NUI_IMAGE_RESOLUTION eColorResolution, _NUI_IMAGE_RESOLUTION eDepthResolution, DepthImagePixel[] depthPixels, DepthImagePoint[] depthPoints)
		{
			fixed (DepthImagePixel* ptr = depthPixels)
			{
				fixed (DepthImagePoint* ptr2 = depthPoints)
				{
					KinectExceptionHelper.CheckHr(_nuiCoordinateMapper.MapColorFrameToDepthFrame(eColorType, eColorResolution, eDepthResolution, (uint)depthPixels.Length, (IntPtr)ptr, (uint)depthPoints.Length, (IntPtr)ptr2));
				}
			}
		}

		/// <summary>
		/// Called by the native runtime when the underlying parameters have changed.
		/// </summary>
		/// <returns>Returns S_OK always.</returns>
		int INuiCoordinateMapperParametersChangedEvent.Invoke()
		{
			_relationalParametersChangedHandler.Invoke(this, new EventArgs());
			return 0;
		}
	}
}
