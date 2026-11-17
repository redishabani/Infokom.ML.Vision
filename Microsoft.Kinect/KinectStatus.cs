namespace Microsoft.Kinect
{
	/// <summary>
	/// Status of the KinectSensor.
	/// </summary>
	public enum KinectStatus
	{
		/// <summary>
		/// The status is undefined.
		/// </summary>
		Undefined,
		/// <summary>
		/// USB unplugged or device not found.
		/// </summary>
		Disconnected,
		/// <summary>
		/// Kinect is fully connected and ready to use.
		/// </summary>
		Connected,
		/// <summary>
		/// The device has been attached and is initializing.
		/// </summary>
		Initializing,
		/// <summary>
		/// The device returned an unexpected error.
		/// </summary>
		Error,
		/// <summary>
		/// Audio not found.  Device likely unpowered.
		/// </summary>
		NotPowered,
		/// <summary>
		/// Some part of the devices is not connected.
		/// </summary>
		NotReady,
		/// <summary>
		/// This device is not a genuine Kinect.
		/// </summary>
		DeviceNotGenuine,
		/// <summary>
		/// This device is not supported.
		/// </summary>
		DeviceNotSupported,
		/// <summary>
		/// There is not enough bandwidth on the USB hub.
		/// </summary>
		InsufficientBandwidth
	}
}
