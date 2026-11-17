namespace Microsoft.Kinect
{
	/// <summary>
	/// This allows for flicker reduction due to power line frequency.
	/// </summary>
	public enum PowerLineFrequency
	{
		/// <summary>
		/// Disables flicker reduction.
		/// </summary>
		Disabled,
		/// <summary>
		/// Used for power line frequencies of 50 hertz.
		/// </summary>
		FiftyHertz,
		/// <summary>
		/// Used for power line frequencies of 60 hertz.
		/// </summary>
		SixtyHertz
	}
}
