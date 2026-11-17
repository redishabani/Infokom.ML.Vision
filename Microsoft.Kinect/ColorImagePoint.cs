using System.Diagnostics;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This struct represents a given X, Y in a color frame.
	/// </summary>
	[DebuggerDisplay("X:{X} Y:{Y}")]
	public struct ColorImagePoint
	{
		/// <summary>
		/// Gets or sets the X location.
		/// </summary>
		public int X { get; set; }

		/// <summary>
		/// Gets or sets the Y location.
		/// </summary>
		public int Y { get; set; }

		/// <summary>
		/// This gets the hash code for a given color image point.
		/// </summary>
		/// <returns>The calculated hash code.</returns>
		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode();
		}

		/// <summary>
		/// This method compares two color image point objects.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>It returns true if they are equal and false otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is ColorImagePoint))
			{
				return false;
			}
			return Equals((ColorImagePoint)obj);
		}

		/// <summary>
		/// This method compares two color image point objects.
		/// </summary>
		/// <param name="imagePoint">The ColorImagePoint to compare.</param>
		/// <returns>It returns true if they are equal and false otherwise.</returns>
		public bool Equals(ColorImagePoint imagePoint)
		{
			if (X == imagePoint.X)
			{
				return Y == imagePoint.Y;
			}
			return false;
		}

		/// <summary>
		/// This method compares two color image point objects.
		/// </summary>
		/// <param name="imagePoint1">The first ColorImagePoint to compare.</param>
		/// <param name="imagePoint2">The second ColorImagePoint to compare.</param>
		/// <returns>It returns true if they are equal and false otherwise.</returns>
		public static bool operator ==(ColorImagePoint imagePoint1, ColorImagePoint imagePoint2)
		{
			return imagePoint1.Equals(imagePoint2);
		}

		/// <summary>
		/// This method compares two color image point objects.
		/// </summary>
		/// <param name="imagePoint1">The first ColorImagePoint to compare.</param>
		/// <param name="imagePoint2">The second ColorImagePoint to compare.</param>
		/// <returns>It returns true if they are not equal and false otherwise.</returns>
		public static bool operator !=(ColorImagePoint imagePoint1, ColorImagePoint imagePoint2)
		{
			return !imagePoint1.Equals(imagePoint2);
		}
	}
}
