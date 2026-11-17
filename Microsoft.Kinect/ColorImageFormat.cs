namespace Microsoft.Kinect
{
	/// <summary>
	/// Image formats for the Color Image Stream. It represents Image Data format, Framerate, and Resolution.
	/// </summary>
	public enum ColorImageFormat
	{
		/// <summary>
		/// The image format is undefined.
		/// </summary>
		Undefined,
		/// <summary>
		/// RGB data (32 bits per pixel, layout corresponding to PixelFormats.Bgr32).
		/// Resolution of 640 by 480 at 30 Frames per second.
		/// </summary>
		RgbResolution640x480Fps30,
		/// <summary>
		/// RGB data (32 bits per pixel, layout corresponding to PixelFormats.Bgr32).
		/// Resolution of 1280 by 960 at 12 Frames per second.
		/// </summary>
		RgbResolution1280x960Fps12,
		/// <summary>
		/// The data is collected from the Kinect as YUV, but it is translated to:
		/// RGB data (32 bits per pixel, layout corresponding to PixelFormats.Bgr32).
		/// Resolution of 640 by 480 at 15 Frames per second.
		/// </summary>
		YuvResolution640x480Fps15,
		/// <summary>
		/// YUV data (32 bits per pixel, layout corresponding to D3DFMT_LIN_UYVY).
		/// Resolution of 640 by 480 at 15 Frames per second.
		/// </summary>
		RawYuvResolution640x480Fps15,
		/// <summary>
		/// Infrared data (16 bits per pxel)
		/// Resolution of 640 by 480 at 30 Frames per second.
		/// </summary>
		InfraredResolution640x480Fps30,
		/// <summary>
		/// Bayer data (8 bits per pixel, layout in alternating pixels of red, green and blue).
		/// Resolution of 640 by 480 at 30 Frames per second.
		/// </summary>
		RawBayerResolution640x480Fps30,
		/// <summary>
		/// Bayer data (8 bits per pixel, layout in alternating pixels of red, green and blue).
		/// Resolution of 1280 by 960 at 12 Frames per second.
		/// </summary>
		RawBayerResolution1280x960Fps12
	}
}
