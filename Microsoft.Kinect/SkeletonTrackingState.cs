namespace Microsoft.Kinect
{
	/// <summary>
	/// These are the possible states for a tracked skeleton.
	/// </summary>
	public enum SkeletonTrackingState
	{
		/// <summary>
		/// This means the skeleton is not being tracked.
		/// </summary>
		NotTracked,
		/// <summary>
		/// The skeleton is being tracked, but we only know the 
		/// general position, and we do not know the specific
		/// joint locations.
		/// </summary>
		PositionOnly,
		/// <summary>
		/// The skeleton is being tracked and the joint data is
		/// available for consumption.
		/// </summary>
		Tracked
	}
}
