using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Maps between Skeleton, Depth, and Color space for a KinectSensor.
	/// </summary>
	public sealed class CoordinateMapper : INotifyPropertyChanged
	{
		/// <summary>
		/// Privately held reference to the KinectSensor for performing mapping operations.
		/// </summary>
		private readonly NuiCoordinateMapper _coordinateMapper;

		/// <summary>
		/// Gets the binary paramters associated with this instance of CoordinateMapper.
		/// </summary>
		/// <returns>Binary parameters.</returns>
		public ReadOnlyCollection<byte> ColorToDepthRelationalParameters => new ReadOnlyCollection<byte>(_coordinateMapper.GetColorToDepthRelationalParameters().ToList());

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Initializes a new instance of the CoordinateMapper class from the given parameters.
		/// </summary>
		/// <param name="parameters">Binary parameters.</param>
		public CoordinateMapper(IEnumerable<byte> parameters)
		{
			_coordinateMapper = new NuiCoordinateMapper(parameters.ToArray());
			_coordinateMapper.RelationalParametersChanged += CoordinateMapper_RelationalParametersChanged;
		}

		/// <summary>
		/// Method to handle RelationalParametersChanged event.  Re-fires to INPC.
		/// </summary>
		/// <param name="sender">The object that sent the event.</param>
		/// <param name="e">Generic event parameters.</param>
		private void CoordinateMapper_RelationalParametersChanged(object sender, EventArgs e)
		{
			if (this.PropertyChanged != null)
			{
				PropertyChangedEventArgs e2 = new PropertyChangedEventArgs("ColorToDepthRelationalParameters");
				this.PropertyChanged(this, e2);
			}
		}

		/// <summary>
		/// Initializes a new instance of the CoordinateMapper class.
		/// </summary>
		/// <param name="sensor">The KinectSensor to perform mappings for.</param>
		public CoordinateMapper(KinectSensor sensor)
		{
			if (sensor == null)
			{
				throw new ArgumentNullException("sensor");
			}
			_coordinateMapper = sensor.NuiSensor.NuiGetCoordinateMapper();
			_coordinateMapper.RelationalParametersChanged += CoordinateMapper_RelationalParametersChanged;
		}

		/// <summary>
		/// Based off the function NuiTransformDepthImageToSkeleton from NuiSkeleton.h, which is inline
		/// so it is neither directly possible nor is it performant to p-invoke it from managed.
		/// </summary>
		/// <param name="fDepthX">X coordinate from the depth image.</param>
		/// <param name="fDepthY">Y coordinate from the depth image.</param>
		/// <param name="depthPixelDatum">Depth value from the depth image.</param>
		/// <returns>A Vector indicating the point on the skeleton corresponding to the depth pixel.</returns>
		private static _Vector4 ManagedNuiTransformDepthImageToSkeleton(float fDepthX, float fDepthY, int depthPixelDatum)
		{
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedNuiTransformDepthImageToSkeletonStart();
			float num = (float)depthPixelDatum / 1000f;
			float x = (fDepthX - 0.5f) * (0.003501f * num) * 320f;
			float y = (0.5f - fDepthY) * (0.003501f * num) * 240f;
			_Vector4 result = default(_Vector4);
			result.x = x;
			result.y = y;
			result.z = num;
			result.w = 1f;
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedNuiTransformDepthImageToSkeletonEnd();
			return result;
		}

		/// <summary>
		/// Based off the function NuiTransformSkeletonToDepthImage from NuiSkeleton.h, which is inline
		/// so it is neither directly possible nor is it performant to p-invoke it from managed.
		/// </summary>
		/// <param name="vPoint">Vector from skeleton to transform.</param>
		/// <param name="pfDepthX">Resulting X coordinate for skeleton point.</param>
		/// <param name="pfDepthY">Resulting Y coordinate for skeleton point.</param>
		/// <param name="pfDepthValue">Resulting depth value for skeleton point.</param>
		internal static void ManagedNuiTransformSkeletonToDepthImage(_Vector4 vPoint, out float pfDepthX, out float pfDepthY, out float pfDepthValue)
		{
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedNuiTransformSkeletonToDepthImageStart();
			if (vPoint.z > float.Epsilon)
			{
				pfDepthX = 0.5f + vPoint.x * 285.63f / (vPoint.z * 320f);
				pfDepthY = 0.5f - vPoint.y * 285.63f / (vPoint.z * 240f);
				pfDepthValue = vPoint.z * 1000f;
			}
			else
			{
				pfDepthX = 0f;
				pfDepthY = 0f;
				pfDepthValue = 0f;
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteManagedNuiTransformSkeletonToDepthImageEnd();
		}

		/// <summary>
		/// Extracts the ImageType from the ColorImageFormat.
		/// </summary>
		/// <param name="colorImageFormat">The ColorImageFormat to convert.</param>
		/// <returns>The converted ImageType.</returns>
		internal static ImageType ImageTypeFromColorImageFormat(ColorImageFormat colorImageFormat)
		{
			if (colorImageFormat != ColorImageFormat.InfraredResolution640x480Fps30)
			{
				return ImageType.Color;
			}
			return ImageType.ColorInfrared;
		}

		/// <summary>
		/// Maps the entire color frame to depth space. Each element will be the result
		/// of mapping the corresponding color pixel into the specified depth frame. 
		/// Will be equal in length to the number of color pixels. Any color pixel 
		/// without a corresponding depth pixel will result in a sentinel value. The resulting
		/// depth points can be tested by calling KinectSensor.IsKnownPoint().
		/// </summary>
		/// <param name="colorImageFormat">The desired target image format.</param>
		/// <param name="depthImageFormat">The depth format of the source.</param>
		/// <param name="depthPixels">
		/// The depth frame pixel data as retrieved from DepthImageFrame.CopyPixelDataTo.
		/// Must be equal in length to Width*Height of the depth format specified by depthImageFormat.
		/// </param>
		/// <param name="depthPoints">
		/// The DepthImagePoint array to receive the data. Must be equal in length to
		/// Width*Height of the color format specified by colorImageFormat.
		/// </param>
		/// <remarks>
		/// <para>Because depth image data and color image data come from separate sensors, pixels in the
		/// two images may not always line up exactly. The two sensors may have different fields of view,
		/// or may not be aimed precisely in the same direction. This means that a point near the edge of
		/// the depth image may correspond to a pixel just beyond the edge of the color image, or vice
		/// versa.</para>
		/// <para>This function accepts coordinates outside the bounds of the depth image. It may return
		/// pixels outside the color image. This means that you can use data from the two images in
		/// combination, even when the two images do not line up completely. You must verify that the
		/// coordinates that are returned lie within the color image before using the coordinates to
		/// reference pixels in that color image.</para>
		/// <para>The depth image coordinates you specify are not required to be within the bounds of the
		/// depth frame image, but they should not be too far outside the depth frame image bounds. If the
		/// coordinates are far outside the depth frame image, they are unlikely to map to coordinates
		/// inside the bounds of the color image. This function will then return color image coordinates
		/// that are unlikely to be useful.</para>
		/// </remarks>
		public void MapColorFrameToDepthFrame(ColorImageFormat colorImageFormat, DepthImageFormat depthImageFormat, DepthImagePixel[] depthPixels, DepthImagePoint[] depthPoints)
		{
			ImageType eColorType = ImageTypeFromColorImageFormat(colorImageFormat);
			if (colorImageFormat == ColorImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "colorImageFormat");
			}
			if (depthImageFormat == DepthImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "depthImageFormat");
			}
			if (depthPixels == null)
			{
				throw new ArgumentNullException("depthPixels");
			}
			if (depthPixels.Length != DepthImageStream.LookUpPixelDataLength(depthImageFormat))
			{
				throw new ArgumentException(Resources.PixelBufferIncorrectLength, "depthPixels");
			}
			if (depthPoints == null)
			{
				throw new ArgumentNullException("depthPoints");
			}
			if (depthPoints.Length != ColorImageStream.LookUpPixelDataLength(colorImageFormat))
			{
				throw new ArgumentException(Resources.PixelBufferIncorrectLength, "depthPoints");
			}
			_coordinateMapper.MapColorFrameToDepthFrame((_NUI_IMAGE_TYPE)eColorType, (_NUI_IMAGE_RESOLUTION)ColorImageStream.LookUpImageResolution(colorImageFormat), (_NUI_IMAGE_RESOLUTION)DepthImageStream.LookUpImageResolution(depthImageFormat), depthPixels, depthPoints);
		}

		/// <summary>
		/// Maps the entire color frame to skeleton space. Each element will be the result of
		/// mapping the corresponding color pixel into the specified depth frame and then into
		/// skeleton space. Will be equal in length to the number of color pixels. Any color
		/// pixel without a corresponding depth pixel will result in a sentinel value. The resulting
		/// skeleton points can be tested by calling KinectSensor.IsKnownPoint().
		/// </summary>
		/// <param name="colorImageFormat">The desired target image format.</param>
		/// <param name="depthImageFormat">The depth format of the source.</param>
		/// <param name="depthPixels">
		/// The depth frame pixel data, as retrieved from DepthImageFrame.CopyPixelDataTo.
		/// Must be equal in length to Width*Height of the depth format specified by depthImageFormat.
		/// </param>
		/// <param name="skeletonPoints">
		/// The SkeletonPoint array to receive the data. Must be equal in length to Width*Height
		/// of the color format specified by colorImageFormat.
		/// </param>
		/// <remarks>
		/// <para>Because depth image data and color image data come from separate sensors, pixels in the
		/// two images may not always line up exactly. The two sensors may have different fields of view,
		/// or may not be aimed precisely in the same direction. This means that a point near the edge of
		/// the depth image may correspond to a pixel just beyond the edge of the color image, or vice
		/// versa.</para>
		/// <para>This function accepts coordinates outside the bounds of the depth image. It may return
		/// pixels outside the color image. This means that you can use data from the two images in
		/// combination, even when the two images do not line up completely. You must verify that the
		/// coordinates that are returned lie within the color image before using the coordinates to
		/// reference pixels in that color image.</para>
		/// <para>The depth image coordinates you specify are not required to be within the bounds of the
		/// depth frame image, but they should not be too far outside the depth frame image bounds. If the
		/// coordinates are far outside the depth frame image, they are unlikely to map to coordinates
		/// inside the bounds of the color image. This function will then return color image coordinates
		/// that are unlikely to be useful.</para>
		/// </remarks>
		public void MapColorFrameToSkeletonFrame(ColorImageFormat colorImageFormat, DepthImageFormat depthImageFormat, DepthImagePixel[] depthPixels, SkeletonPoint[] skeletonPoints)
		{
			ImageType eColorType = ImageTypeFromColorImageFormat(colorImageFormat);
			if (colorImageFormat == ColorImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "colorImageFormat");
			}
			if (depthImageFormat == DepthImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "depthImageFormat");
			}
			if (depthPixels == null)
			{
				throw new ArgumentNullException("depthPixels");
			}
			if (depthPixels.Length != DepthImageStream.LookUpPixelDataLength(depthImageFormat))
			{
				throw new ArgumentException(Resources.PixelBufferIncorrectLength, "depthPixels");
			}
			if (skeletonPoints == null)
			{
				throw new ArgumentNullException("skeletonPoints");
			}
			if (skeletonPoints.Length != ColorImageStream.LookUpPixelDataLength(colorImageFormat))
			{
				throw new ArgumentException(Resources.PixelBufferIncorrectLength, "skeletonPoints");
			}
			_coordinateMapper.MapColorFrameToSkeletonFrame((_NUI_IMAGE_TYPE)eColorType, (_NUI_IMAGE_RESOLUTION)ColorImageStream.LookUpImageResolution(colorImageFormat), (_NUI_IMAGE_RESOLUTION)DepthImageStream.LookUpImageResolution(depthImageFormat), depthPixels, skeletonPoints);
		}

		/// <summary>
		/// Maps the entire depth frame to color space.  Each element will be the result of
		/// mapping the corresponding DepthImagePixel into color space.  Will be equal in length
		/// to the number of DepthImagePixels. Any DepthImagePixel without a corresponding
		/// color pixel will result in a sentinel value. The resulting ColorImagePoints can
		/// be tested by calling KinectSensor.IsKnownPoint().
		/// </summary>
		/// <param name="depthImageFormat">The depth format of the source.</param>
		/// <param name="depthPixels">
		/// The depth frame pixel data, as retrieved from DepthImageFrame.CopyPixelDataTo.
		/// Must be equal in length to Width*Height of the depth format specified by depthImageFormat.
		/// </param>        
		/// <param name="colorImageFormat">The desired target image format.</param>
		/// <param name="colorPoints">
		/// The ColorImagePoint array to receive the data.  Must be equal in length to Width*Height
		/// of the depth format specified by depthImageFormat.
		/// </param>
		/// <remarks>
		/// <para>Because depth image data and color image data come from separate sensors, pixels in the
		/// two images may not always line up exactly. The two sensors may have different fields of view,
		/// or may not be aimed precisely in the same direction. This means that a point near the edge of
		/// the depth image may correspond to a pixel just beyond the edge of the color image, or vice
		/// versa.</para>
		/// <para>This function accepts coordinates outside the bounds of the depth image. It may return
		/// pixels outside the color image. This means that you can use data from the two images in
		/// combination, even when the two images do not line up completely. You must verify that the
		/// coordinates that are returned lie within the color image before using the coordinates to
		/// reference pixels in that color image.</para>
		/// <para>The depth image coordinates you specify are not required to be within the bounds of the
		/// depth frame image, but they should not be too far outside the depth frame image bounds. If the
		/// coordinates are far outside the depth frame image, they are unlikely to map to coordinates
		/// inside the bounds of the color image. This function will then return color image coordinates
		/// that are unlikely to be useful.</para>
		/// </remarks>
		public void MapDepthFrameToColorFrame(DepthImageFormat depthImageFormat, DepthImagePixel[] depthPixels, ColorImageFormat colorImageFormat, ColorImagePoint[] colorPoints)
		{
			ImageType eColorType = ImageTypeFromColorImageFormat(colorImageFormat);
			if (depthImageFormat == DepthImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "depthImageFormat");
			}
			if (depthPixels == null)
			{
				throw new ArgumentNullException("depthPixels");
			}
			if (depthPixels.Length != DepthImageStream.LookUpPixelDataLength(depthImageFormat))
			{
				throw new ArgumentException(Resources.PixelBufferIncorrectLength, "depthPixels");
			}
			if (colorImageFormat == ColorImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "colorImageFormat");
			}
			if (colorPoints == null)
			{
				throw new ArgumentNullException("colorPoints");
			}
			if (colorPoints.Length != depthPixels.Length)
			{
				throw new ArgumentException(Resources.PixelBufferIncorrectLength, "colorPoints");
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteMapDepthFrameToColorFrameStart();
			_coordinateMapper.MapDepthFrameToColorFrame((_NUI_IMAGE_RESOLUTION)DepthImageStream.LookUpImageResolution(depthImageFormat), depthPixels, (_NUI_IMAGE_TYPE)eColorType, (_NUI_IMAGE_RESOLUTION)ColorImageStream.LookUpImageResolution(colorImageFormat), colorPoints);
			//TO DO: 
			//KinectEtwProvider.EventWriteMapDepthFrameToColorFrameEnd();
		}

		/// <summary>
		/// Maps the entire depth frame to skeleton space. Each element will be the result of mapping
		/// the corresponding DepthImagePixel from the specified depth frame into skeleton space. Will be
		/// equal in length to the number of depth pixels. Any depth pixel without corresponding skeleton
		/// point will result in a sentinel value.  The resulting SkeletonPoints can be tested by calling
		/// KinectSensor.IsKnownPoint().
		/// </summary>
		/// <param name="depthImageFormat">The depth format of the source.</param>
		/// <param name="depthPixels">
		/// The depth frame pixel data, as retrieved from DepthImageFrame.CopyPixelDataTo.
		/// Must be equal in length to Width*Height of the depth format specified by depthImageFormat.
		/// </param>
		/// <param name="skeletonPoints">
		/// The SkeletonPoint array to receive the data. Must be equal in length to Width*Height of the
		/// depth format specified by depthImageFormat.
		/// </param>
		public void MapDepthFrameToSkeletonFrame(DepthImageFormat depthImageFormat, DepthImagePixel[] depthPixels, SkeletonPoint[] skeletonPoints)
		{
			if (depthImageFormat == DepthImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "depthImageFormat");
			}
			if (depthPixels == null)
			{
				throw new ArgumentNullException("depthPixels");
			}
			if (depthPixels.Length != DepthImageStream.LookUpPixelDataLength(depthImageFormat))
			{
				throw new ArgumentException(Resources.PixelBufferIncorrectLength, "depthPixels");
			}
			if (skeletonPoints == null)
			{
				throw new ArgumentNullException("skeletonPoints");
			}
			if (depthPixels.Length != skeletonPoints.Length)
			{
				throw new ArgumentException(Resources.PixelBufferIncorrectLength, "skeletonPoints");
			}
			_coordinateMapper.MapDepthFrameToSkeletonFrame((_NUI_IMAGE_RESOLUTION)DepthImageStream.LookUpImageResolution(depthImageFormat), depthPixels, skeletonPoints);
		}

		/// <summary>
		/// Maps from a DepthImagePoint to the corresponding ColorImagePoint.
		/// </summary>
		/// <param name="depthImageFormat">The depth format of the source.</param>
		/// <param name="depthPoint">The depth point to map.</param>
		/// <param name="colorImageFormat">The desired target image format.</param>
		/// <returns>The ColorImagePoint corresponding to the point in DepthImage space.</returns>
		/// <remarks>
		/// <para>Because depth image data and color image data come from separate sensors, pixels in the
		/// two images may not always line up exactly. The two sensors may have different fields of view,
		/// or may not be aimed precisely in the same direction. This means that a point near the edge of
		/// the depth image may correspond to a pixel just beyond the edge of the color image, or vice
		/// versa.</para>
		/// <para>This function accepts coordinates outside the bounds of the depth image. It may return
		/// pixels outside the color image. This means that you can use data from the two images in
		/// combination, even when the two images do not line up completely. You must verify that the
		/// coordinates that are returned lie within the color image before using the coordinates to
		/// reference pixels in that color image.</para>
		/// <para>The depth image coordinates you specify are not required to be within the bounds of the
		/// depth frame image, but they should not be too far outside the depth frame image bounds. If the
		/// coordinates are far outside the depth frame image, they are unlikely to map to coordinates
		/// inside the bounds of the color image. This function will then return color image coordinates
		/// that are unlikely to be useful.</para>
		/// </remarks>
		public ColorImagePoint MapDepthPointToColorPoint(DepthImageFormat depthImageFormat, DepthImagePoint depthPoint, ColorImageFormat colorImageFormat)
		{
			ImageType eColorType = ImageTypeFromColorImageFormat(colorImageFormat);
			if (depthImageFormat == DepthImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "depthImageFormat");
			}
			if (colorImageFormat == ColorImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "colorImageFormat");
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteMapDepthToColorImagePointStart();
			_NUI_DEPTH_IMAGE_POINT depthPoint2 = new _NUI_DEPTH_IMAGE_POINT
			{
				x = depthPoint.X,
				y = depthPoint.Y,
				depth = depthPoint.Depth
			};
			_coordinateMapper.MapDepthPointToColorPoint((_NUI_IMAGE_RESOLUTION)DepthImageStream.LookUpImageResolution(depthImageFormat), ref depthPoint2, (_NUI_IMAGE_TYPE)eColorType, (_NUI_IMAGE_RESOLUTION)ColorImageStream.LookUpImageResolution(colorImageFormat), out var colorPoint);
			//TO DO: 
			//KinectEtwProvider.EventWriteMapDepthToColorImagePointEnd();
			return new ColorImagePoint
			{
				X = colorPoint.x,
				Y = colorPoint.y
			};
		}

		/// <summary>
		/// Maps from a DepthImagePoint to the corresponding SkeletonPoint.
		/// </summary>
		/// <param name="depthImageFormat">The the depth format to use in the conversion.</param>
		/// <param name="depthImagePoint">The depth image point for the depth frame.</param>
		/// <returns>The skeleton point for the given DepthImagePoint and format.</returns>
		public SkeletonPoint MapDepthPointToSkeletonPoint(DepthImageFormat depthImageFormat, DepthImagePoint depthImagePoint)
		{
			if (depthImageFormat == DepthImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "depthImageFormat");
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteMapDepthToSkeletonPointStart();
			ImageResolution resolution = DepthImageStream.LookUpImageResolution(depthImageFormat);
			ImageStream.ResolutionToHeightWidth(resolution, out var width, out var height);
			_Vector4 vector = ManagedNuiTransformDepthImageToSkeleton((float)depthImagePoint.X / (float)width, (float)depthImagePoint.Y / (float)height, depthImagePoint.Depth);
			SkeletonPoint result = new SkeletonPoint
			{
				X = vector.x,
				Y = vector.y,
				Z = vector.z
			};
			//TO DO: 
			//KinectEtwProvider.EventWriteMapDepthToSkeletonPointEnd();
			return result;
		}

		/// <summary>
		/// Maps from a SkeletonPoint to the corresponding ColorImagePoint.
		/// </summary>
		/// <param name="skeletonPoint">The SkeletonPoint to map.</param>
		/// <param name="colorImageFormat">The desired target image format.</param>
		/// <returns>The ColorImagePoint for the given skeleton point and format.</returns>
		/// <remarks>
		/// <para>Because depth image data and color image data come from separate sensors, pixels in the
		/// two images may not always line up exactly. The two sensors may have different fields of view,
		/// or may not be aimed precisely in the same direction. This means that a point near the edge of
		/// the depth image may correspond to a pixel just beyond the edge of the color image, or vice
		/// versa.</para>
		/// <para>This function accepts coordinates outside the bounds of the depth image. It may return
		/// pixels outside the color image. This means that you can use data from the two images in
		/// combination, even when the two images do not line up completely. You must verify that the
		/// coordinates that are returned lie within the color image before using the coordinates to
		/// reference pixels in that color image.</para>
		/// <para>The depth image coordinates you specify are not required to be within the bounds of the
		/// depth frame image, but they should not be too far outside the depth frame image bounds. If the
		/// coordinates are far outside the depth frame image, they are unlikely to map to coordinates
		/// inside the bounds of the color image. This function will then return color image coordinates
		/// that are unlikely to be useful.</para>
		/// </remarks>
		public ColorImagePoint MapSkeletonPointToColorPoint(SkeletonPoint skeletonPoint, ColorImageFormat colorImageFormat)
		{
			if (colorImageFormat == ColorImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "colorImageFormat");
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteMapSkeletonPointToColorStart();
			_Vector4 vPoint = default(_Vector4);
			vPoint.x = skeletonPoint.X;
			vPoint.y = skeletonPoint.Y;
			vPoint.z = skeletonPoint.Z;
			vPoint.w = 1f;
			ManagedNuiTransformSkeletonToDepthImage(vPoint, out var pfDepthX, out var pfDepthY, out var pfDepthValue);
			ColorImagePoint result = MapDepthPointToColorPoint(DepthImageFormat.Resolution640x480Fps30, new DepthImagePoint
			{
				X = (int)(pfDepthX * 640f),
				Y = (int)(pfDepthY * 480f),
				Depth = (int)pfDepthValue
			}, colorImageFormat);
			//TO DO: 
			//KinectEtwProvider.EventWriteMapSkeletonPointToColorEnd();
			return result;
		}

		/// <summary>
		/// Maps from a SkeletonPoint to the corresponding DepthImagePoint.
		/// </summary>
		/// <param name="skeletonPoint">The supplied skeleton point.</param>
		/// <param name="depthImageFormat">The depth format to convert to.</param>
		/// <returns>The DepthImagePoint for the given skeleton point.</returns>
		public DepthImagePoint MapSkeletonPointToDepthPoint(SkeletonPoint skeletonPoint, DepthImageFormat depthImageFormat)
		{
			if (depthImageFormat == DepthImageFormat.Undefined)
			{
				throw new ArgumentException(Resources.ImageFormatNotSupported, "depthImageFormat");
			}
			//TO DO: 
			//KinectEtwProvider.EventWriteMapSkeletonPointToDepthStart();
			_Vector4 vPoint = default(_Vector4);
			vPoint.x = skeletonPoint.X;
			vPoint.y = skeletonPoint.Y;
			vPoint.z = skeletonPoint.Z;
			vPoint.w = 1f;
			ManagedNuiTransformSkeletonToDepthImage(vPoint, out var pfDepthX, out var pfDepthY, out var pfDepthValue);
			ImageResolution resolution = DepthImageStream.LookUpImageResolution(depthImageFormat);
			ImageStream.ResolutionToHeightWidth(resolution, out var width, out var height);
			DepthImagePoint result = new DepthImagePoint
			{
				X = (int)(pfDepthX * (float)width + 0.5f),
				Y = (int)(pfDepthY * (float)height + 0.5f),
				Depth = (int)(pfDepthValue + 0.5f)
			};
			//TO DO: 
			//KinectEtwProvider.EventWriteMapSkeletonPointToDepthEnd();
			return result;
		}
	}
}
