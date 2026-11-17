using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Translates HRESULTs from native NUI API's and COM interfaces to managed exceptions.
	/// </summary>
	internal static class KinectExceptionHelper
	{
		/// <summary>
		/// The sensor does not support the requested feature (e.g. near mode).
		/// </summary>
		private const int E_NUI_HARDWARE_FEATURE_UNAVAILABLE = -2097086449;

		/// <summary>
		/// The given sensor is not a genuine Kinect™ sensor.
		/// </summary>
		private const int E_NUI_NOTGENUINE = -2097086458;

		/// <summary>
		/// The given sensor is not a supported Kinect™ sensor.  Probably an XBox Kinect™ on a non-developer machine.
		/// </summary>
		private const int E_NUI_NOTSUPPORTED = -2097086456;

		/// <summary>
		/// Invalid index sent to NuiCreateSensorByIndex or NuiCreateSensorById.
		/// </summary>
		private const int E_NUI_BADINDEX = -2097085051;

		/// <summary>
		/// A Win32 API returned ERROR_REQUEST_ABORTED.
		/// </summary>
		private const int E_WIN32_ERROR_REQUEST_ABORTED = -2147023661;

		/// <summary>
		/// This means that the per-device mutex has already been created in NuiInitialize, and therefore
		/// we conclude that the device is already in use.
		/// </summary>
		private const int E_NUI_DEVICE_IN_USE = -2097086455;

		/// <summary>
		/// Throws a managed exception for the given native HRESULT from the NUI API.
		/// </summary>
		/// <param name="hr">Native HRESULT returned from API.</param>
		internal static void CheckHr(int hr)
		{
			if (hr >= 0)
			{
				return;
			}
			switch (hr)
			{
			case -2097085051:
				throw new InvalidOperationException(Resources.BadIndex);
			case -2097086458:
				throw new InvalidOperationException(Resources.DeviceNotGenuine);
			case -2097086456:
				throw new InvalidOperationException(Resources.DeviceNotSupported);
			case -2097086449:
				throw new InvalidOperationException(Resources.HardwareFeatureUnavailable);
			case -2097086455:
				throw new IOException(Resources.KinectInUse);
			case -2147023661:
				throw new UnauthorizedAccessException(Resources.UnauthorizedAccess);
			}
			Exception exceptionForHR = Marshal.GetExceptionForHR(hr);
			if (exceptionForHR != null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Resources.GenericException, new object[1] { hr }), exceptionForHR);
			}
		}
	}
}
