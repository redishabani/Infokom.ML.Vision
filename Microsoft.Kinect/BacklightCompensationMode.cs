namespace Microsoft.Kinect
{
	/// <summary>
	/// This enum has all of the available backlight compensation modes.
	/// </summary>
	public enum BacklightCompensationMode
	{
		/// <summary>
		/// Favors the average brightness.
		/// </summary>
		AverageBrightness = 0,
		/// <summary>
		/// Favors the center of the scene.
		/// </summary>
		CenterPriority = 1,
		/// <summary>
		/// Favors lowlights.
		/// </summary>
		LowlightsPriority = 2,
		/// <summary>
		/// Favors the center only.
		/// </summary>
		CenterOnly = 4
	}
}
