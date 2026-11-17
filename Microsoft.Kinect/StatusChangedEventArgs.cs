using System;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Event data for the StatusChanged event.
	/// </summary>
	public sealed class StatusChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the reported status for the sensor.
		/// </summary>
		public KinectStatus Status { get; internal set; }

		/// <summary>
		/// Gets the sensor that has changed status.
		/// </summary>
		public KinectSensor Sensor { get; internal set; }

		/// <summary>
		/// Initializes a new instance of the StatusChangedEventArgs class.
		/// <remarks>Internal ctor to prevent external creation.</remarks>
		/// </summary>
		internal StatusChangedEventArgs()
		{
		}
	}
}
