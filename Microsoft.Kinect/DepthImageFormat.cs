namespace Microsoft.Kinect
{
	/// <summary>
	/// Image formats for the Depth Image Stream. It represents Framerate, and Resolution.
	/// </summary>
	public enum DepthImageFormat
	{
		/// <summary>
		/// The image format is undefined.
		/// </summary>
		Undefined,
		/// <summary>
		/// Resolution of 640 by 480 at 30 Frames per second.
		/// </summary>
		Resolution640x480Fps30,
		/// <summary>
		/// Resolution of 320 by 240 at 30 Frames per second.
		/// </summary>
		Resolution320x240Fps30,
		/// <summary>
		/// Resolution of 80 by 60 at 30 Frames per second.
		/// </summary>
		Resolution80x60Fps30
	}
}
