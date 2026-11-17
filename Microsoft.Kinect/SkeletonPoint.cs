using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This struct describes a 3 dimensional point in skeleton space.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 4, Size = 16)]
	[DebuggerDisplay("X:{X} Y:{Y} Z:{Z}")]
	public struct SkeletonPoint
	{
		/// <summary>
		/// Gets or sets the X coordinate of the skeleton point.
		/// </summary>
		public float X { get; set; }

		/// <summary>
		/// Gets or sets the Y coordinate of the skeleton point.
		/// </summary>
		public float Y { get; set; }

		/// <summary>
		/// Gets or sets the Z coordinate of the skeleton point.
		/// </summary>
		public float Z { get; set; }

		/// <summary>
		/// This method compares two skeleton point objects.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>It returns true if they are equal and false otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is SkeletonPoint))
			{
				return false;
			}
			return Equals((SkeletonPoint)obj);
		}

		/// <summary>
		/// This method compares two skeleton point objects.
		/// </summary>
		/// <param name="skeletonPoint">The SkeletonPoint to compare.</param>
		/// <returns>It returns true if they are equal and false otherwise.</returns>
		public bool Equals(SkeletonPoint skeletonPoint)
		{
			if (X.Equals(skeletonPoint.X) && Y.Equals(skeletonPoint.Y))
			{
				return Z.Equals(skeletonPoint.Z);
			}
			return false;
		}

		/// <summary>
		/// This gets the hash code for a given skeleton point.
		/// </summary>
		/// <returns>The calculated hash code.</returns>
		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
		}

		/// <summary>
		/// This method compares two skeleton point objects.
		/// </summary>
		/// <param name="skeletonPoint1">The first SkeletonPoint to compare.</param>
		/// <param name="skeletonPoint2">The second SkeletonPoint to compare.</param>
		/// <returns>It returns true if they are equal and false otherwise.</returns>
		public static bool operator ==(SkeletonPoint skeletonPoint1, SkeletonPoint skeletonPoint2)
		{
			return skeletonPoint1.Equals(skeletonPoint2);
		}

		/// <summary>
		/// This method compares two skeleton point objects.
		/// </summary>
		/// <param name="skeletonPoint1">The first SkeletonPoint to compare.</param>
		/// <param name="skeletonPoint2">The second SkeletonPoint to compare.</param>
		/// <returns>It returns true if they are not equal and false otherwise.</returns>
		public static bool operator !=(SkeletonPoint skeletonPoint1, SkeletonPoint skeletonPoint2)
		{
			return !skeletonPoint1.Equals(skeletonPoint2);
		}
	}
}
