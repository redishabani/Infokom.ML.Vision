using System;
using System.Diagnostics;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This struct represents a given X, Y, and depth value in a depth frame.
	/// </summary>
	[DebuggerDisplay("X:{X} Y:{Y} Depth:{Depth} PlayerIndex:{PlayerIndex}")]
	public struct DepthImagePoint
	{
		/// <summary>
		/// Backing field for X.
		/// </summary>
		private int _x;

		/// <summary>
		/// Backing field for Y.
		/// </summary>
		private int _y;

		/// <summary>
		/// Backing field for Depth.
		/// </summary>
		private int _depth;

		/// <summary>
		/// Padding for compat with unmanaged object.
		/// </summary>
		private int _playerIndex;

		/// <summary>
		/// Gets or sets the X location.
		/// </summary>
		public int X
		{
			get
			{
				return _x;
			}
			set
			{
				_x = value;
			}
		}

		/// <summary>
		/// Gets or sets the Y location.
		/// </summary>
		public int Y
		{
			get
			{
				return _y;
			}
			set
			{
				_y = value;
			}
		}

		/// <summary>
		/// Gets or sets the depth value.
		/// </summary>
		public int Depth
		{
			get
			{
				return _depth;
			}
			set
			{
				_depth = value;
			}
		}

		/// <summary>
		/// Gets a reserved value.  Do not use.
		/// </summary>
		[Obsolete("The player index property is reserved. Do not use.")]
		public int PlayerIndex => 0;

		/// <summary>
		/// This gets the hash code for a given depth image point.
		/// </summary>
		/// <returns>The calculated hash code.</returns>
		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode() + Depth.GetHashCode();
		}

		/// <summary>
		/// This method compares two depth image point objects.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>It returns true if they are equal and false otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is DepthImagePoint))
			{
				return false;
			}
			return Equals((DepthImagePoint)obj);
		}

		/// <summary>
		/// This method compares two depth image point objects.
		/// </summary>
		/// <param name="imagePoint">The DepthImagePoint to compare.</param>
		/// <returns>It returns true if they are equal and false otherwise.</returns>
		public bool Equals(DepthImagePoint imagePoint)
		{
			if (X == imagePoint.X && Y == imagePoint.Y)
			{
				return Depth == imagePoint.Depth;
			}
			return false;
		}

		/// <summary>
		/// This method compares two depth image point objects.
		/// </summary>
		/// <param name="imagePoint1">The first DepthImagePoint to compare.</param>
		/// <param name="imagePoint2">The second DepthImagePoint to compare.</param>
		/// <returns>It returns true if they are equal and false otherwise.</returns>
		public static bool operator ==(DepthImagePoint imagePoint1, DepthImagePoint imagePoint2)
		{
			return imagePoint1.Equals(imagePoint2);
		}

		/// <summary>
		/// This method compares two depth image point objects.
		/// </summary>
		/// <param name="imagePoint1">The first DepthImagePoint to compare.</param>
		/// <param name="imagePoint2">The second DepthImagePoint to compare.</param>
		/// <returns>It returns true if they are not equal and false otherwise.</returns>
		public static bool operator !=(DepthImagePoint imagePoint1, DepthImagePoint imagePoint2)
		{
			return !imagePoint1.Equals(imagePoint2);
		}
	}
}
