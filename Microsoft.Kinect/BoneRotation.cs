namespace Microsoft.Kinect
{
	/// <summary>
	/// Rotation of an individual bone.
	/// </summary>
	public class BoneRotation
	{
		/// <summary>
		/// Gets or sets a matrix representation of the bone rotation.
		/// </summary>
		public Matrix4 Matrix { get; set; }

		/// <summary>
		/// Gets or sets a quaternion representation of the bone rotation.
		/// </summary>
		public Vector4 Quaternion { get; set; }
	}
}
