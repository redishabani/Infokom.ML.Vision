using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Kinect
{
	/// <summary>
	///   A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		/// <summary>
		///   Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("Microsoft.Kinect.Resources", typeof(Resources).Assembly);
					resourceMan = resourceManager;
				}
				return resourceMan;
			}
		}

		/// <summary>
		///   Overrides the current thread's CurrentUICulture property for all
		///   resource lookups using this strongly typed resource class.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		/// <summary>
		///   Looks up a localized string similar to A bad index or id was used when referencing a Kinect sensor..
		/// </summary>
		internal static string BadIndex => ResourceManager.GetString("BadIndex", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to This API cannot be called when an event listener has been set..
		/// </summary>
		internal static string CannotPollAndUseEvents => ResourceManager.GetString("CannotPollAndUseEvents", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The capture has already been started.
		/// </summary>
		internal static string CaptureAlreadyStarted => ResourceManager.GetString("CaptureAlreadyStarted", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The capture has not been started.
		/// </summary>
		internal static string CaptureNotStarted => ResourceManager.GetString("CaptureNotStarted", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The Color Stream must be Enabled to get next Color image frame. Call KinectSensor.ColorStream.Enable().
		/// </summary>
		internal static string ColorStreamMustBeEnabled => ResourceManager.GetString("ColorStreamMustBeEnabled", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Depth filter must implement the native INuiDepthFilter interface.
		/// </summary>
		internal static string DepthFilterMustImplementNativeInterface => ResourceManager.GetString("DepthFilterMustImplementNativeInterface", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to This Depth Stream must be Enabled to get next Depth image frame. Call KinectSensor.DepthStream.Enable().
		/// </summary>
		internal static string DepthStreamMustBeEnabled => ResourceManager.GetString("DepthStreamMustBeEnabled", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The attached Kinect device is not genuine..
		/// </summary>
		internal static string DeviceNotGenuine => ResourceManager.GetString("DeviceNotGenuine", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The attached Kinect device is not supported..
		/// </summary>
		internal static string DeviceNotSupported => ResourceManager.GetString("DeviceNotSupported", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Elevation angle must be between Elevation Minimum/Maximum.
		/// </summary>
		internal static string ElevationIncorrect => ResourceManager.GetString("ElevationIncorrect", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Native DLL failed in get device name.
		/// </summary>
		internal static string FailedToGetDeviceName => ResourceManager.GetString("FailedToGetDeviceName", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to This API has returned an exception from an HRESULT: 0x{0:X}.
		/// </summary>
		internal static string GenericException => ResourceManager.GetString("GenericException", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The feature is not supported by this version of the hardware.
		/// </summary>
		internal static string HardwareFeatureUnavailable => ResourceManager.GetString("HardwareFeatureUnavailable", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The specified Image Format is not supported..
		/// </summary>
		internal static string ImageFormatNotSupported => ResourceManager.GetString("ImageFormatNotSupported", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Warning: An ImageFrame instance was not Disposed..
		/// </summary>
		internal static string ImageFrameNotDisposed => ResourceManager.GetString("ImageFrameNotDisposed", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to JointType index value must match Joint.JointType.
		/// </summary>
		internal static string IncorrectJointType => ResourceManager.GetString("IncorrectJointType", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Unexpected value for BeamAngleMode.
		/// </summary>
		internal static string InvalidBeamAngleMode => ResourceManager.GetString("InvalidBeamAngleMode", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Unexpected value for EchoCancellationMode.
		/// </summary>
		internal static string InvalidEchoCancellationMode => ResourceManager.GetString("InvalidEchoCancellationMode", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Kinect is already in use in another process..
		/// </summary>
		internal static string KinectInUse => ResourceManager.GetString("KinectInUse", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Kinect must be running to control the motor.
		/// </summary>
		internal static string KinectMustBeRunning => ResourceManager.GetString("KinectMustBeRunning", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Kinect is not ready.
		/// </summary>
		internal static string KinectNotReady => ResourceManager.GetString("KinectNotReady", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The Kinect sensor plugged into your computer is for use on the Xbox 360.
		///             You may continue using your Xbox 360 Kinect sensor on your computer for development purposes.
		///             Microsoft does not guarantee full compatibility for Kinect for Windows applications and the Xbox 360 Kinect Sensor..
		/// </summary>
		internal static string KinectNotSupportedDeveloper => ResourceManager.GetString("KinectNotSupportedDeveloper", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The Kinect sensor plugged into your computer is for use on the Xbox 360.
		///             To run a Kinect for Windows application, you must plug in a Kinect for Windows sensor..
		/// </summary>
		internal static string KinectNotSupportedNonDeveloper => ResourceManager.GetString("KinectNotSupportedNonDeveloper", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to NativeStreamHandle can't be Zero.
		/// </summary>
		internal static string NativeStreamCantBeZero => ResourceManager.GetString("NativeStreamCantBeZero", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Feature mode needs to be set to True before setting this property.
		/// </summary>
		internal static string NeedFeatureMode => ResourceManager.GetString("NeedFeatureMode", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Could not find a Kinect audio device.
		/// </summary>
		internal static string NoDevicesFound => ResourceManager.GetString("NoDevicesFound", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The operation is not supported.
		/// </summary>
		internal static string NotSupported => ResourceManager.GetString("NotSupported", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The data buffer length must match the length required by the associated ImageFormat..
		/// </summary>
		internal static string PixelBufferIncorrectLength => ResourceManager.GetString("PixelBufferIncorrectLength", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to KinectSensor must be running to get next frame. Call KinectSensor.Start().
		/// </summary>
		internal static string SensorMustBeRunning => ResourceManager.GetString("SensorMustBeRunning", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to KinectSensor must be running in order to start audio stream.
		/// </summary>
		internal static string SensorMustBeRunningForAudio => ResourceManager.GetString("SensorMustBeRunningForAudio", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to The data buffer must have length equal to SkeletonArrayLength..
		/// </summary>
		internal static string SkeletonBufferIncorrectLength => ResourceManager.GetString("SkeletonBufferIncorrectLength", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to SkeletonEngine must be Enabled to get next skeleton frame. Call KinectSensor.SkeletonEngine.Enable().
		/// </summary>
		internal static string SkeletonEngineMustBeEnabled => ResourceManager.GetString("SkeletonEngineMustBeEnabled", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Warning: A SkeletonFrame instance was not Disposed..
		/// </summary>
		internal static string SkeletonFrameNotDisposed => ResourceManager.GetString("SkeletonFrameNotDisposed", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Could not access the hardware.
		/// </summary>
		internal static string UnauthorizedAccess => ResourceManager.GetString("UnauthorizedAccess", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Unexpected depth range.
		/// </summary>
		internal static string UnexpectedDepthRange => ResourceManager.GetString("UnexpectedDepthRange", resourceCulture);

		/// <summary>
		///   Looks up a localized string similar to Unexpected UniqueDeviceName format.
		/// </summary>
		internal static string UnexpectedUniqueDeviceName => ResourceManager.GetString("UnexpectedUniqueDeviceName", resourceCulture);

		internal Resources()
		{
		}
	}
}
