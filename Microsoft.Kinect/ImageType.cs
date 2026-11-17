namespace Microsoft.Kinect
{
	/// <summary>
	/// Keep in sync with _NUI_IMAGE_TYPE.
	/// </summary>
	internal enum ImageType
	{
		/// <summary>
		/// Depth and Player.
		/// </summary>
		DepthAndPlayerIndex,
		/// <summary>
		/// Color only.
		/// </summary>
		Color,
		/// <summary>
		/// Color in YUV format.
		/// </summary>
		ColorYuv,
		/// <summary>
		/// Color in YUV RAW format.
		/// </summary>
		ColorYuvRaw,
		/// <summary>
		/// Depth only.
		/// </summary>
		Depth,
		/// <summary>
		/// Infrared only.
		/// </summary>
		ColorInfrared,
		/// <summary>
		/// Color in 8-bit Bayer format.
		/// </summary>
		ColorBayer
	}
}
