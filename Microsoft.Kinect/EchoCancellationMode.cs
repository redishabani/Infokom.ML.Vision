namespace Microsoft.Kinect
{
	/// <summary>
	/// Type of audio echo cancellation.
	/// </summary>
	public enum EchoCancellationMode
	{
		/// <summary>
		/// No echo cancellation or suppression.
		/// </summary>
		None,
		/// <summary>
		/// Echo cancellation only.
		/// </summary>
		CancellationOnly,
		/// <summary>
		/// Echo cancellation and echo suppression.
		/// </summary>
		CancellationAndSuppression
	}
}
