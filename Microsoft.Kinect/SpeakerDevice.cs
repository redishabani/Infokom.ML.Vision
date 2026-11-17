using System.Runtime.InteropServices;

namespace Microsoft.Kinect
{
	/// <summary>
	/// A structure used by NuiGetSpeakerDevices to get the number of speaker devices.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct SpeakerDevice
	{
		/// <summary>
		/// String length for speaker device name.
		/// </summary>
		private const int MaxStrLen = 512;

		/// <summary>
		/// The device friendly name.
		/// </summary>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string DeviceName;

		/// <summary>
		/// This device index can be passed to KinectAudioSource.SpeakerIndex.
		/// </summary>
		[MarshalAs(UnmanagedType.I4)]
		public int DeviceIndex;

		/// <summary>
		/// A Boolean value that indicates whether the speaker device is the default. 
		/// </summary>
		[MarshalAs(UnmanagedType.Bool)]
		public bool IsDefault;
	}
}
