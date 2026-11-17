namespace Infokom.ML.Vision.Sensors
{
	/// <summary>
	/// Creates visual frame sources without exposing their concrete implementations to callers.
	/// </summary>
	public static class VisualSensor
	{
		/// <summary>
		/// Opens a video file as a visual sensor.
		/// </summary>
		public static IVisualSensor FromVideoFile(string fileName) => new VideoFilePlayer(fileName);

		/// <summary>
		/// Opens the first connected Kinect color camera, or the camera identified by <paramref name="sensorId"/>.
		/// </summary>
		public static IVisualSensor FromKinectColorCamera(string sensorId = null) => new KinectColorCamera(sensorId);
	}
}
