using System;
using System.Diagnostics;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This struct is used to describe a skeleton's joint.
	/// </summary>
	[Serializable]
	[DebuggerDisplay("Position:{Position} JointType:{JointType} TrackingState:{TrackingState}")]
	public struct Joint
	{
		/// <summary>
		/// Gets or sets the joint's position.
		/// </summary>
		public SkeletonPoint Position { get; set; }

		/// <summary>
		/// Gets the joint's type.
		/// </summary>
		public JointType JointType { get; internal set; }

		/// <summary>
		/// Gets or sets the tracking state of this joint.
		/// </summary>
		public JointTrackingState TrackingState { get; set; }

		/// <summary>
		/// This method calculates the hash code for a joint.
		/// </summary>
		/// <returns>The joint's hash code.</returns>
		public override int GetHashCode()
		{
			return Position.GetHashCode() + JointType.GetHashCode() + TrackingState.GetHashCode();
		}

		/// <summary>
		/// This method compares two joint objects.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>It returns true if they are equal and false otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Joint))
			{
				return false;
			}
			return Equals((Joint)obj);
		}

		/// <summary>
		/// This method compares two joint objects.
		/// </summary>
		/// <param name="joint">The joint to compare.</param>
		/// <returns>It returns true if they are equal and false otherwise.</returns>
		public bool Equals(Joint joint)
		{
			if (JointType == joint.JointType && Position == joint.Position)
			{
				return TrackingState == joint.TrackingState;
			}
			return false;
		}

		/// <summary>
		/// This method compares two joint objects.
		/// </summary>
		/// <param name="joint1">The first joint to compare.</param>
		/// <param name="joint2">The second joint to compare.</param>
		/// <returns>It returns true if they are equal and false otherwise.</returns>
		public static bool operator ==(Joint joint1, Joint joint2)
		{
			return joint1.Equals(joint2);
		}

		/// <summary>
		/// This method compares two joint objects.
		/// </summary>
		/// <param name="joint1">The first joint to compare.</param>
		/// <param name="joint2">The second joint to compare.</param>
		/// <returns>It returns true if they are not equal and false otherwise.</returns>
		public static bool operator !=(Joint joint1, Joint joint2)
		{
			return !joint1.Equals(joint2);
		}
	}
}
