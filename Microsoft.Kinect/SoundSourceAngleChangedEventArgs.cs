using System;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Argument for the SoundSourceAngleChanged event.
	/// </summary>
	public sealed class SoundSourceAngleChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the angle (in degrees) from kinect sensor towards the current sound source. 
		/// When facing the Kinect:
		/// 0: center
		/// positive angles: right
		/// negative angles: left
		/// Range is -50 to +50 degrees.
		/// </summary>
		public double Angle { get; internal set; }

		/// <summary>
		/// Gets a measure of our confidence in the sound source angle.
		/// This value is in the range [0,1], 1 being the highest possible confidence.
		/// </summary>
		public double ConfidenceLevel { get; internal set; }

		/// <summary>
		/// Initializes a new instance of the SoundSourceAngleChangedEventArgs class.
		/// </summary>
		/// <remarks>
		/// This constructor is internal to prevent external creation.
		/// </remarks>
		internal SoundSourceAngleChangedEventArgs()
		{
		}
	}
}
