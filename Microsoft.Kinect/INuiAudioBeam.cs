using System.Runtime.InteropServices;

namespace Microsoft.Kinect
{
	/// <summary>
	///   Provides ability to get/set audio beam direction for Kinect™
	///   sensor, as well as the ability to get the position of current sound
	///   source relative to sensor.
	/// </summary>
	[ComImport]
	[Guid("8C3CEBFA-A35D-497E-BC9A-E9752A8155E0")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComVisible(true)]
	internal interface INuiAudioBeam
	{
		/// <summary>
		///   Gets the currently selected beam direction (i.e., angle that
		///   Kinect™ audio sensor gives relative preference over other
		///   angles).
		/// </summary>
		/// <param name="angle">
		///   Receives the horizontal angle (in degrees) of the currently selected
		///   beam in camera coordinates, where the x-z axes define the horizontal
		///   plane. The angle is relative to the z-axis, which is perpendicular to
		///   the Kinect sensor. 
		/// </param>
		void GetBeam(out double angle);

		/// <summary>
		///   Sets the currently selected beam direction (i.e., angle that
		///   Kinect™ audio sensor gives relative preference over other
		///   angles).
		/// </summary>
		/// <param name="angle">
		///   The horizontal angle (in degrees) to set as the new beam angle, in
		///   camera coordinates, where the x-z axes define the horizontal plane.
		///   The angle is relative to the z-axis, which is perpendicular to the
		///   Kinect™ sensor.
		/// </param>
		void SetBeam(double angle);

		/// <summary>
		/// Provides the best estimate of the sound source location relative to
		/// the Kinect™ sensor.
		/// </summary>
		/// <param name="angle">
		/// Receives the estimated horizontal angle (in degrees) to the sound
		/// source, in camera coordinates, where the x-z axes define the horizontal
		/// plane. The angle is relative to the z-axis, which is perpendicular to
		/// the Kinect™ sensor. 
		/// </param>
		/// <param name="confidence">
		/// Receives a measure of the confidence (in [0.0,1.0] interval) in the
		/// estimated value for <paramref name="angle" />.
		/// </param>
		void GetPosition(out double angle, out double confidence);
	}
}
