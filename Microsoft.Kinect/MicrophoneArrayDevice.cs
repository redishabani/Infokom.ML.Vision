using System.Runtime.InteropServices;

namespace Microsoft.Kinect
{
	/// <summary>
	/// A structure used by NuiGetMicrophoneArrayDevices to return the Kinect sensors found on the system.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct MicrophoneArrayDevice
	{
		/// <summary>
		/// Maximum length for device friendly name.
		/// </summary>
		private const int MaxStrLen = 512;

		/// <summary>
		/// The device friendly name.
		/// </summary>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string DeviceName;

		/// <summary>
		/// Device unique identifier.
		/// </summary>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string DeviceID;

		/// <summary>
		/// This device index can be passed to KinectAudioSource.MicrophoneIndex.
		/// </summary>
		[MarshalAs(UnmanagedType.I4)]
		public int DeviceIndex;
	}
}
