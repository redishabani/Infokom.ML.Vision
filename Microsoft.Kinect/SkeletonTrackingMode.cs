namespace Microsoft.Kinect
{
	/// <summary>
	/// Available skeleton tracking modes.
	/// </summary>
	public enum SkeletonTrackingMode
	{
		/// <summary>
		/// Default skeleton tracking (tracks all 20 joints).
		/// </summary>
		Default,
		/// <summary>
		/// Seated skeleton tracking (tracks only the upper 10 joints).
		/// </summary>
		Seated
	}
}
