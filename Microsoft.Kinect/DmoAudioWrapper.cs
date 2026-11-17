using System.Globalization;
using System.Threading;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Default facade will create a DMO object using registered COM server, and will call native layer directly.
	/// </summary>
	internal class DmoAudioWrapper : INativeAudioWrapper
	{
		/// <summary>
		/// <see cref="T:Microsoft.Kinect.KinectSensor" /> used to get audio microphone device and audio DMO information.
		/// </summary>
		private readonly KinectSensor _sensor;

		/// <summary>
		/// Initializes a new instance of the DmoAudioWrapper class.
		/// </summary>
		/// <param name="sensor">
		/// <see cref="T:Microsoft.Kinect.KinectSensor" /> used to get audio microphone device and audio DMO information.
		/// </param>
		public DmoAudioWrapper(KinectSensor sensor)
		{
			_sensor = sensor;
		}

		/// <summary>
		/// Creates an instance of Kinect DMO object.
		/// </summary>
		/// <returns>
		/// DMO object created. Can never be <code>null</code>.
		/// </returns>
		public object CreateDmoInstance()
		{
			if (_sensor == null || _sensor.NuiSensor == null)
			{
				return null;
			}
			return _sensor.NuiSensor.NuiGetAudioSource();
		}

		/// <summary>
		/// Destroy DMO object previously created via <see cref="M:Microsoft.Kinect.DmoAudioWrapper.CreateDmoInstance" />.
		/// </summary>
		/// <param name="dmo">DMO object which should be destroyed.</param>
		/// <remarks>
		/// Implementation does nothing, since DMO object is managed by
		/// underlying <see cref="T:Microsoft.Kinect.KinectSensor" />.
		/// </remarks>
		public void DestroyDmoInstance(object dmo)
		{
		}

		/// <summary>
		/// Get metadata about the available microphone array devices.
		/// </summary>
		/// <param name="deviceBuffer">
		/// Array to hold audio device metadata. If <code>null</code>, only <paramref name="count" /> is populated.
		/// </param>
		/// <param name="count">
		/// Number of available microphone array devices.
		/// </param>
		public void GetMicrophoneArrayDevices(MicrophoneArrayDevice[] deviceBuffer, ref int count)
		{
			NativeMethods.NuiGetMicrophoneArrayDevices(deviceBuffer, (deviceBuffer != null) ? deviceBuffer.Length : 0, ref count);
		}

		/// <summary>
		/// Get metadata about the available speaker devices.
		/// </summary>
		/// <param name="deviceBuffer">
		/// Array to hold audio device metadata. If <code>null</code>, only <paramref name="count" /> is populated.
		/// </param>
		/// <param name="count">
		/// Number of available speaker devices.
		/// </param>
		public void GetSpeakerDevices(SpeakerDevice[] deviceBuffer, ref int count)
		{
			NativeMethods.NuiGetSpeakerDevices(deviceBuffer, (deviceBuffer != null) ? deviceBuffer.Length : 0, ref count);
		}

		/// <summary>
		/// Determines if the specified microphone device corresponds to the Kinect™ sensor
		/// associated with this <see cref="T:Microsoft.Kinect.INativeAudioWrapper" />.
		/// </summary>
		/// <param name="device">
		/// Microphone array device to match against this sensor.
		/// </param>
		/// <returns>
		/// <code>true</code> if microphone array device matches the associated Kinect™ sensor.
		/// <code>false</code> otherwise.
		/// </returns>
		/// <remarks>
		/// <code>false</code> is always returned for <code>null</code> device IDs.
		/// </remarks>
		public bool MatchAudioDeviceToSensor(MicrophoneArrayDevice device)
		{
			if (_sensor == null || _sensor.NuiSensor == null)
			{
				return false;
			}
			string text = _sensor.NuiSensor.NuiAudioArrayId();
			string deviceID = device.DeviceID;
			if (text == null || deviceID == null)
			{
				return false;
			}
			string[] array = text.ToUpper(CultureInfo.InvariantCulture).Split('\\');
			string[] array2 = deviceID.ToUpper(CultureInfo.InvariantCulture).Split('#');
			if (array.Length > 2 && array2.Length > 2 && array[1] == array2[1] && array[2] == array2[2])
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Create an event used to signal the <see cref="T:Microsoft.Kinect.KinectAudioSource" /> to stop streaming.
		/// </summary>
		/// <returns>
		/// A new <see cref="T:System.Threading.ManualResetEvent" />.
		/// </returns>
		public ManualResetEvent CreateStreamStopEvent()
		{
			return new ManualResetEvent(initialState: false);
		}
	}
}
