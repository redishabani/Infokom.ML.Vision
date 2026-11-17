namespace Microsoft.Kinect
{
	/// <summary>
	/// A structure representing a pixel in a DepthImageFrame.
	/// </summary>
	public struct DepthImagePixel
	{
		/// <summary>
		/// The value for an unknown depth.
		/// </summary>
		private const int _UnknownDepth = 0;

		/// <summary>
		/// Gets or sets the player index for the pixel.
		/// </summary>
		public short PlayerIndex { get; set; }

		/// <summary>
		/// Gets or sets the depth (in millimeters) for the pixel.
		/// </summary>
		public short Depth { get; set; }

		/// <summary>
		/// Gets a value indicating whether the Depth value is a known value.
		/// </summary>
		public bool IsKnownDepth => Depth != 0;

		/// <summary>
		/// This gets the hash code for a given depth image pixel.
		/// </summary>
		/// <returns>The calculated hash code.</returns>
		public override int GetHashCode()
		{
			return Depth.GetHashCode() + PlayerIndex.GetHashCode();
		}

		/// <summary>
		/// This method compares two depth image pixel objects.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>It returns true if they are equal and false otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is DepthImagePixel))
			{
				return false;
			}
			return Equals((DepthImagePixel)obj);
		}

		/// <summary>
		/// This method compares two depth image pixel objects.
		/// </summary>
		/// <param name="pixel">The DepthImagePixel to compare.</param>
		/// <returns>It returns true if they are equal and false otherwise.</returns>
		public bool Equals(DepthImagePixel pixel)
		{
			if (Depth == pixel.Depth)
			{
				return PlayerIndex == pixel.PlayerIndex;
			}
			return false;
		}

		/// <summary>
		/// This method compares two depth image pixel objects.
		/// </summary>
		/// <param name="pixel1">The first DepthImagePixel to compare.</param>
		/// <param name="pixel2">The second DepthImagePixel to compare.</param>
		/// <returns>It returns true if they are equal and false otherwise.</returns>
		public static bool operator ==(DepthImagePixel pixel1, DepthImagePixel pixel2)
		{
			return pixel1.Equals(pixel2);
		}

		/// <summary>
		/// This method compares two depth image pixel objects.
		/// </summary>
		/// <param name="pixel1">The first DepthImagePixel to compare.</param>
		/// <param name="pixel2">The second DepthImagePixel to compare.</param>
		/// <returns>It returns true if they are not equal and false otherwise.</returns>
		public static bool operator !=(DepthImagePixel pixel1, DepthImagePixel pixel2)
		{
			return !pixel1.Equals(pixel2);
		}
	}
}
