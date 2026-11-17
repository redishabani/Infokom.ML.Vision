namespace Microsoft.Kinect
{
	/// <summary>
	/// The interface of a filter to be applied to depth frame data.
	/// </summary>
	public interface IDepthFilter
	{
		/// <summary>
		/// Gets or sets the native depth filter interface.
		/// </summary>
		object NativeObject { get; set; }

		/// <summary>
		/// Processes a frame of depth data.
		/// </summary>
		/// <param name="timestamp">Timestamp of the depth frame.</param>
		/// <param name="width">Width of the depth frame, in pixels.</param>
		/// <param name="height">Height of the depth frame, in pixels.</param>
		/// <param name="pixelData">Depth frame data.</param>
		/// <returns>true if the depth frame was modified; false otherwise.</returns>
		bool ProcessFrame(long timestamp, int width, int height, DepthImagePixel[] pixelData);
	}
}
