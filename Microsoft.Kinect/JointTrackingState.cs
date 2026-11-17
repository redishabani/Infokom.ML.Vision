namespace Microsoft.Kinect
{
	/// <summary>
	/// The tracking state of a specific joint.
	/// </summary>
	public enum JointTrackingState
	{
		/// <summary>
		/// The joint is not tracked and no data is known about this joint.
		/// </summary>
		NotTracked,
		/// <summary>
		/// The joint location is inferred. The data should be used with the
		/// understanding that the confidence level of the location is very low.
		/// </summary>
		Inferred,
		/// <summary>
		/// The joint is tracked and the data can be trusted.
		/// </summary>
		Tracked
	}
}
