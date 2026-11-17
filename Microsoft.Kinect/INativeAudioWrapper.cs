using System.Threading;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Wrapper interface used to abstract DMO object creation, destruction and other native calls, to allow for mocking.
	/// </summary>
	internal interface INativeAudioWrapper
	{
		/// <summary>
		/// Creates an instance of Kinect DMO object.
		/// </summary>
		/// <returns>
		/// DMO object created. Can never be <code>null</code>.
		/// </returns>
		object CreateDmoInstance();

		/// <summary>
		/// Destroy DMO object previously created via <see cref="M:Microsoft.Kinect.INativeAudioWrapper.CreateDmoInstance" />.
		/// </summary>
		/// <param name="dmo">DMO object which should be destroyed.</param>
		void DestroyDmoInstance(object dmo);

		/// <summary>
		/// Get metadata about the available microphone array devices.
		/// </summary>
		/// <param name="deviceBuffer">
		/// Array to hold audio device metadata. If <code>null</code>, only <paramref name="count" /> is populated.
		/// </param>
		/// <param name="count">
		/// Number of available microphone array devices.
		/// </param>
		void GetMicrophoneArrayDevices(MicrophoneArrayDevice[] deviceBuffer, ref int count);

		/// <summary>
		/// Get metadata about the available speaker devices.
		/// </summary>
		/// <param name="deviceBuffer">
		/// Array to hold audio device metadata. If <code>null</code>, only <paramref name="count" /> is populated.
		/// </param>
		/// <param name="count">
		/// Number of available speaker devices.
		/// </param>
		void GetSpeakerDevices(SpeakerDevice[] deviceBuffer, ref int count);

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
		bool MatchAudioDeviceToSensor(MicrophoneArrayDevice device);

		/// <summary>
		/// Create an event used to signal the <see cref="T:Microsoft.Kinect.KinectAudioSource" /> to stop streaming.
		/// </summary>
		/// <returns>
		/// A new <see cref="T:System.Threading.ManualResetEvent" />.
		/// </returns>
		ManualResetEvent CreateStreamStopEvent();
	}
}
