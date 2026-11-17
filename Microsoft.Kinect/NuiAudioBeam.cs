using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Provides the location of a sound source.
	/// Wrapper for INuiAudioBeam COM interface.
	/// </summary>
	internal class NuiAudioBeam : INuiAudioBeam
	{
		/// <summary>
		/// The wrapped audio device.
		/// </summary>
		private readonly Microsoft.Kinect.Interop.INuiAudioBeam _beam;

		/// <summary>
		/// Gets the wrapped audio device.
		/// </summary>
		public object Wrapped => _beam;

		/// <summary>
		/// Initializes a new instance of the NuiAudioBeam class with the given audio device.
		/// </summary>
		/// <param name="beam">The audio device to wrap.</param>
		public NuiAudioBeam(Microsoft.Kinect.Interop.INuiAudioBeam beam)
		{
			_beam = beam;
		}

		/// <summary>
		/// Gets the currently selected beam direction.
		/// </summary>
		/// <param name="angle">Receives the horizontal angle (in radians) of the currently selected 
		/// beam in camera coordinates, where the x-z axes define the horizontal plane. The angle is 
		/// relative to the z-axis, which is perpendicular to the Kinect sensor.</param>
		public void GetBeam(out double angle)
		{
			KinectExceptionHelper.CheckHr(_beam.GetBeam(out angle));
		}

		/// <summary>
		/// Sets the currently selected beam direction.
		/// The KinectAudio DMO supports 11 beams. They have fixed directions ranging from −0.875 
		/// radians to +0.875 radians in .0175 radian increments, which corresponds to approximately 
		/// −50 degrees to +50 degrees, in ten degree increments.
		/// </summary>
		/// <param name="angle">The beam angle, in radians.</param>
		/// <remarks>Important : To use SetBeam, you must set the KinectAudio DMO's MFPKEY_WMAAECMA_FEATR_MICARR_MODE 
		/// property key to MIC_ARRAY_MODE.MICARRAY_EXTERN_BEAM. </remarks>
		public void SetBeam(double angle)
		{
			KinectExceptionHelper.CheckHr(_beam.SetBeam(angle));
		}

		/// <summary>
		/// Provides the best estimate of the sound source location, as determined by the KinectAudio DMO's sound source localizer.
		/// </summary>
		/// <param name="angle">Receives the estimated horizontal angle (in radians) to the sound source 
		/// in camera coordinates, where the x-z axes define the horizontal plane. The angle is relative 
		/// to the z-axis, which is perpendicular to the Kinect sensor.</param>
		/// <param name="confidence">Receives the confidence value of the estimate.</param>
		/// <remarks>You must call IMediaObject::ProcessOutput before calling GetPosition. This method 
		/// drives the processing of audio samples through the audio pipeline, which is required for 
		/// determining the source direction. GetPosition will be successful only if ProcessOutput has 
		/// returned S_OK. Do not simply check for success, because ProcessOutput can also return S_FALSE 
		/// if it lacks sufficient data to proceed. If ProcessOutput returns S_FALSE, continue calling the 
		/// method until it returns S_OK, and then call GetPosition. 
		/// GetPosition is based on a measurement queue. If the queue empties, the function returns zero 
		/// for both parameters.
		/// </remarks>
		public void GetPosition(out double angle, out double confidence)
		{
			KinectExceptionHelper.CheckHr(_beam.GetPosition(out angle, out confidence));
		}
	}
}
