using System;
using System.Runtime.InteropServices;
using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Native API declarations from Kinect10.dll and KinectAudio10.dll.
	/// </summary>
	internal static class NativeMethods
	{
		/// <summary>
		/// Declaration of native NuiStatusCallback typedef.
		/// </summary>
		/// <param name="status">HRESULT status from the sensor.</param>
		/// <param name="instanceName">Sensor hub device instance id.</param>
		/// <param name="uniqueDeviceName">Sensor camera device instance id.  May be null.</param>
		/// <param name="pUserData">User data registered with NuiSetDeviceStatusCallback.</param>
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void NuiStatusCallback(int status, [MarshalAs(UnmanagedType.LPWStr)] string instanceName, [MarshalAs(UnmanagedType.LPWStr)] string uniqueDeviceName, IntPtr pUserData);

		/// <summary>
		/// Success HRESULT code.
		/// </summary>
		internal const int S_OK = 0;

		/// <summary>
		/// Sets a callback function that gets notified when the sensor connection status changes. 
		/// Use this method to handle the case of a user connecting or disconnecting a Kinect sensor. 
		/// </summary>
		/// <param name="pCallback">Function pointer to the callback function.</param>
		/// <param name="pUserData">Pointer to optional context data that will be passed to the callback.</param>
		[DllImport("Kinect10.dll", CallingConvention = CallingConvention.StdCall)]
		internal static extern void NuiSetDeviceStatusCallback([In] NuiStatusCallback pCallback, IntPtr pUserData);

		/// <summary>
		/// Gets device information for the connected Kinect sensors. 
		/// </summary>
		/// <param name="deviceBuffer">Pointer to an array of NUI_MICROPHONE_ARRAY_DEVICE structures, 
		/// allocated by the caller, each of which receives the device information for a single connected 
		/// Kinect sensor. If you set this parameter to NULL, the piDeviceCount parameter will still 
		/// receive the number of connected Kinect sensors.</param>
		/// <param name="size">Size of the array pointed to by the pDeviceInfo parameter.</param>
		/// <param name="count">Receives the number of connected Kinect sensors. When this function returns, 
		/// this parameter is set to the number of structures in the array pointed to by the pDeviceInfo 
		/// parameter that contain valid information.</param>
		[DllImport("KinectAudio10.dll", CallingConvention = CallingConvention.StdCall)]
		internal static extern void NuiGetMicrophoneArrayDevices([Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] MicrophoneArrayDevice[] deviceBuffer, [In] int size, [In][Out] ref int count);

		/// <summary>
		/// Gets the active speaker devices found on the system. 
		/// </summary>
		/// <param name="deviceBuffer">Pointer to an array of NUI_SPEAKER_DEVICE structures, 
		/// allocated by the caller, each of which receives the device information for a single 
		/// connected speaker device. If you set this parameter to NULL, the piDeviceCount parameter 
		/// will still receive the number of connected speaker devices.</param>
		/// <param name="size">Size of the array pointed to by the pDeviceInfo parameter.</param>
		/// <param name="count">Receives the number of connected speaker devices. When this function returns, 
		/// this parameter is set to the number of structures in the array pointed to by the pDeviceInfo 
		/// parameter that contain valid information. </param>
		[DllImport("KinectAudio10.dll", CallingConvention = CallingConvention.StdCall)]
		internal static extern void NuiGetSpeakerDevices([Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] SpeakerDevice[] deviceBuffer, [In] int size, [In][Out] ref int count);

		/// <summary>
		/// Gets the number of kinects on the machine, including disconnected ones.
		/// </summary>
		/// <param name="pCount">Pointer to an integer in which to return the number of Kinect sensors.</param>
		/// <returns>Returns S_OK if successful; otherwise, returns a failure code.</returns>
		[DllImport("Kinect10.dll", CallingConvention = CallingConvention.StdCall)]
		internal static extern int NuiGetSensorCount(out int pCount);

		/// <summary>
		/// Creates an instance of the Kinect sensor with the specified index so that an application can open and use it.
		/// </summary>
		/// <remarks>
		/// Applications that support more than one Kinect sensor call NuiCreateSensorByIndex to access the 
		/// second and subsequent devices. This function returns a pointer to an INuiSensor interface, 
		/// which provides functions that are identical to those in NuiApi.h, NuiImageCamera.h, and NuiSkeleton.h., 
		/// but operate on a particular device instance. 
		/// </remarks>
		/// <param name="index">The zero-based index of the Kinect sensor to open. Valid values range from zero to one less than the value returned by the NuiGetSensorCount function.</param>
		/// <param name="pNuiSensor">A pointer that receives a reference to the created INuiSensor interface.</param>
		/// <returns>Returns S_OK if successful; otherwise, returns one of the failure codes in the following table.
		/// E_OUTOFMEMORY - The maximum number of Kinect sensors are already open.  
		/// E_POINTER - The ppNuiSensor parameter does not represent a valid address. 
		/// </returns>
		[DllImport("Kinect10.dll", CallingConvention = CallingConvention.StdCall)]
		internal static extern int NuiCreateSensorByIndex(int index, out INuiSensor pNuiSensor);

		/// <summary>
		/// Creates an instance of the Kinect sensor with the specified ID so that an application can open and use it. 
		/// </summary>
		/// <remarks>Applications that support more than one Kinect sensor call NuiCreateSensorById to access the 
		/// second and subsequent devices. This function returns a pointer to an INuiSensor interface, which provides 
		/// functions that are identical to those in NuiApi.h, NuiImageCamera.h, and NuiSkeleton.h., but operate on a 
		/// particular device instance. 
		/// </remarks>
		/// <param name="instanceName">A pointer to the ID of the Kinect sensor to open.</param>
		/// <param name="pNuiSensor">A pointer that receives a reference to the created INuiSensor interface.</param>
		/// <returns>Returns S_OK if successful; otherwise, returns one of the failure codes in the following table. 
		/// E_OUTOFMEMORY - The maximum number of Kinect sensors are already open.  
		/// E_POINTER - The ppNuiSensor parameter does not represent a valid address. 
		/// </returns>
		[DllImport("Kinect10.dll", CallingConvention = CallingConvention.StdCall)]
		internal static extern int NuiCreateSensorById([MarshalAs(UnmanagedType.LPWStr)] string instanceName, out INuiSensor pNuiSensor);

		/// <summary>
		/// Associates the calling thread with the specified task.
		/// </summary>
		/// <param name="taskName">
		/// The name of the task to be performed. This name must match the name of one of the subkeys of the following
		/// key HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks.
		/// </param>
		/// <param name="taskIndex">
		/// The unique task identifier. The first time this function is called, this value must be 0 on input. The
		/// index value is returned on output and can be used as input in subsequent calls.
		/// </param>
		/// <returns>
		/// If the function succeeds, it returns a handle to the task. 
		/// If the function fails, it returns 0. To retrieve extended error information, call GetLastError.
		/// </returns>
		[DllImport("avrt.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "AvSetMmThreadCharacteristicsW")]
		internal static extern int AvSetMmThreadCharacteristics([MarshalAs(UnmanagedType.LPWStr)] string taskName, [In][Out] ref int taskIndex);

		/// <summary>
		/// Indicates that a thread is no longer performing work associated with the specified task.
		/// </summary>
		/// <param name="avrtHandle">
		/// A handle to the task. This handle is returned by the AvSetMmThreadCharacteristics or
		/// AvSetMmMaxThreadCharacteristics function.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// </returns>
		/// <remarks>
		/// This function must be called from the same thread that called the AvSetMmThreadCharacteristics or
		/// AvSetMmMaxThreadCharacteristics function to create the handle. Otherwise, the function will fail.
		/// </remarks>
		[DllImport("avrt.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AvRevertMmThreadCharacteristics(int avrtHandle);

		/// <summary>
		/// Utility to retrieve the address of gFailureStack in Kinect10.dll to allow inspection of the 
		/// failure "flight recorder".
		/// </summary>
		/// <returns>
		/// The address of Kinect10!gFailureStack as a UInt64.  
		/// In 32 bit, only the lower 4 bytes are used.
		/// </returns>
		[DllImport("Kinect10.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "#1")]
		internal static extern ulong NuiDebugGetFailureStack();

		/// <summary>
		/// Calculate Bone Orientations.
		/// </summary>
		/// <param name="pSkeletonData">Pointer to the skeleton data to calculate joint angles for.</param>
		/// <param name="pBoneOrientations">
		/// Pointer to an array of _NUI_SKELETON_BONE_ORIENTATION of dimension NUI_SKELETON_POSITION_COUNT (20).
		/// This array must be allocated by the user before calling this function.
		/// </param>
		/// <returns>
		/// Returns S_OK if successful; otherwise, returns one of the failure codes in the following table. 
		/// E_INVALIDARG - The <paramref name="pSkeletonData" /> parameter is NULL.
		/// E_POINTER - The <paramref name="pBoneOrientations" /> parameter is NULL.
		/// S_FALSE - The joints required to calculate the skeleton root are not visible. Hierarchical bone rotations are set to Identity.
		/// </returns>
		[DllImport("Kinect10.dll", CallingConvention = CallingConvention.StdCall)]
		internal static extern int NuiSkeletonCalculateBoneOrientations([In] ref _NUI_SKELETON_DATA pSkeletonData, [Out] _NUI_SKELETON_BONE_ORIENTATION[] pBoneOrientations);

		[DllImport("Kinect10.dll", CallingConvention = CallingConvention.StdCall)]
		internal static extern int NuiCreateCoordinateMapperFromParameters([In] uint length, [In] IntPtr pMapperParameters, out INuiCoordinateMapper pCoordinateMapper);

		[DllImport("kernel32.dll")]
		internal static extern void CopyMemory(IntPtr destination, IntPtr source, int length);
	}
}
