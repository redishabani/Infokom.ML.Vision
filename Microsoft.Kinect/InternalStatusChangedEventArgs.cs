using System;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Internal class for arguments passed from NuiSensor.
	/// </summary>
	internal class InternalStatusChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the device instance id for the sensor hub.
		/// </summary>
		public string DeviceConnectionId { get; set; }

		/// <summary>
		/// Gets or sets the device instance id for the sensor camera.
		/// </summary>
		public string UniqueKinectId { get; set; }

		/// <summary>
		/// Gets or sets the mapped status for the sensor.
		/// </summary>
		public KinectStatus KinectStatus { get; set; }
	}
}
