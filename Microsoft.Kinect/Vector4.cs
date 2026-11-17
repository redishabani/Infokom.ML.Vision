using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Vector4 is a 4-element vector.
	/// </summary>
	public struct Vector4
	{
		/// <summary>
		/// Gets or sets the X element.
		/// </summary>
		public float X { get; set; }

		/// <summary>
		/// Gets or sets the Y element.
		/// </summary>
		public float Y { get; set; }

		/// <summary>
		/// Gets or sets the Z element.
		/// </summary>
		public float Z { get; set; }

		/// <summary>
		/// Gets or sets the W element.
		/// </summary>
		public float W { get; set; }

		/// <summary>
		/// Compares two Vector4 objects for equality.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>Returns true if they are equal and false otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Vector4))
			{
				return false;
			}
			return Equals((Vector4)obj);
		}

		/// <summary>
		/// Compares two Vector4 objects for equality.
		/// </summary>
		/// <param name="vector">The Vector4 to compare.</param>
		/// <returns>Returns true if they are equal and false otherwise.</returns>
		public bool Equals(Vector4 vector)
		{
			if (X.Equals(vector.X) && Y.Equals(vector.Y) && Z.Equals(vector.Z))
			{
				return W.Equals(vector.W);
			}
			return false;
		}

		/// <summary>
		/// Gets the hash code for a given Vector4.
		/// </summary>
		/// <returns>The calculated hash code.</returns>
		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode();
		}

		/// <summary>
		/// Compares two Vector4 objects for equality.
		/// </summary>
		/// <param name="vector1">The first Vector4 to compare.</param>
		/// <param name="vector2">The second Vector4 to compare.</param>
		/// <returns>Returns true if they are equal and false otherwise.</returns>
		public static bool operator ==(Vector4 vector1, Vector4 vector2)
		{
			return vector1.Equals(vector2);
		}

		/// <summary>
		/// Compares two Vector4 objects for inequality.
		/// </summary>
		/// <param name="vector1">The first Vector4 to compare.</param>
		/// <param name="vector2">The second Vector4 to compare.</param>
		/// <returns>Returns true if they are not equal and false otherwise.</returns>
		public static bool operator !=(Vector4 vector1, Vector4 vector2)
		{
			return !vector1.Equals(vector2);
		}

		/// <summary>
		/// Returns a Vector4 that is a copy of a native _Vector4 structure.
		/// </summary>
		/// <param name="vector">The native structure.</param>
		/// <returns>The managed structure.</returns>
		internal static Vector4 CopyFrom(ref _Vector4 vector)
		{
			return new Vector4
			{
				X = vector.x,
				Y = vector.y,
				Z = vector.z,
				W = vector.w
			};
		}
	}
}
