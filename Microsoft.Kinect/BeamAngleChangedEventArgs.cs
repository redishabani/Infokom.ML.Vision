using System;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Argument for the BeamAngleChanged event.
	/// </summary>
	public sealed class BeamAngleChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the angle (in degrees) of the direction towards which the audio beam
		/// is pointing, i.e.: the direction towards which the kinect sensor is listening. 
		/// When facing the Kinect:
		/// 0: center
		/// positive angles: right
		/// negative angles: left
		/// Range is -50 to +50 degrees.
		/// </summary>
		public double Angle { get; internal set; }

		/// <summary>
		/// Initializes a new instance of the BeamAngleChangedEventArgs class.
		/// </summary>
		/// <remarks>
		/// This constructor is internal to prevent external creation.
		/// </remarks>
		internal BeamAngleChangedEventArgs()
		{
		}
	}
}
