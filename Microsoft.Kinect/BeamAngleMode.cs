namespace Microsoft.Kinect
{
	/// <summary>
	/// Type of beam angle determination.
	/// </summary>
	public enum BeamAngleMode
	{
		/// <summary>
		/// Automatic Beam determination based on fixed coefficients.
		/// </summary>
		Automatic,
		/// <summary>
		/// Automatic Beam determination based on dynamic coefficients.
		/// </summary>
		Adaptive,
		/// <summary>
		/// User set beam angle.
		/// </summary>
		Manual
	}
}
