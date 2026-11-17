using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Orientation information for an individual bone in a skeleton.
	/// </summary>
	public class BoneOrientation
	{
		/// <summary>
		/// Joint types representing the starting joint for each bone (the parent of the bone's endpoint joint).
		/// </summary>
		private static readonly JointType[] c_parents = new JointType[20]
		{
			JointType.HipCenter,
			JointType.HipCenter,
			JointType.Spine,
			JointType.ShoulderCenter,
			JointType.ShoulderCenter,
			JointType.ShoulderLeft,
			JointType.ElbowLeft,
			JointType.WristLeft,
			JointType.ShoulderCenter,
			JointType.ShoulderRight,
			JointType.ElbowRight,
			JointType.WristRight,
			JointType.HipCenter,
			JointType.HipLeft,
			JointType.KneeLeft,
			JointType.AnkleLeft,
			JointType.HipCenter,
			JointType.HipRight,
			JointType.KneeRight,
			JointType.AnkleRight
		};

		/// <summary>
		/// Gets the skeleton joint at which the bone ends.
		/// </summary>
		public JointType EndJoint { get; internal set; }

		/// <summary>
		/// Gets the skeleton joint at which the bone starts.
		/// </summary>
		public JointType StartJoint { get; internal set; }

		/// <summary>
		/// Gets or sets the rotation of the bone relative to its "parent" bone in the skeleton.
		/// </summary>
		public BoneRotation HierarchicalRotation { get; set; }

		/// <summary>
		/// Gets or sets the rotation of the bone relative to the camera coordinate system.
		/// </summary>
		public BoneRotation AbsoluteRotation { get; set; }

		/// <summary>
		/// Initializes a new instance of the BoneOrientation class.
		/// </summary>
		/// <param name="jointType">The joint corresponding to the endpoint of the bone.</param>
		public BoneOrientation(JointType jointType)
		{
			EndJoint = jointType;
			StartJoint = c_parents[(int)jointType];
			HierarchicalRotation = new BoneRotation();
			AbsoluteRotation = new BoneRotation();
			SetDefaults();
		}

		/// <summary>
		/// Sets the confidence and rotations of the BoneOrientation to their default values.
		/// </summary>
		internal void SetDefaults()
		{
			HierarchicalRotation.Matrix = Matrix4.Identity;
			HierarchicalRotation.Quaternion = new Vector4
			{
				W = 1f
			};
			AbsoluteRotation.Matrix = Matrix4.Identity;
			AbsoluteRotation.Quaternion = new Vector4
			{
				W = 1f
			};
		}

		/// <summary>
		/// Copies data from a native _NUI_SKELETON_BONE_ORIENTATION structure.
		/// </summary>
		/// <param name="nativeBoneOrientation">The native structure.</param>
		internal void CopyFrom(ref _NUI_SKELETON_BONE_ORIENTATION nativeBoneOrientation)
		{
			HierarchicalRotation.Matrix = Matrix4.CopyFrom(ref nativeBoneOrientation.hierarchicalRotation.rotationMatrix);
			HierarchicalRotation.Quaternion = Vector4.CopyFrom(ref nativeBoneOrientation.hierarchicalRotation.rotationQuaternion);
			AbsoluteRotation.Matrix = Matrix4.CopyFrom(ref nativeBoneOrientation.absoluteRotation.rotationMatrix);
			AbsoluteRotation.Quaternion = Vector4.CopyFrom(ref nativeBoneOrientation.absoluteRotation.rotationQuaternion);
		}

		/// <summary>
		/// Copies data from another Bone Orientation class.
		/// </summary>
		/// <param name="sourceBoneOrientation">The bone orientation to copy.</param>
		internal void CopyFrom(BoneOrientation sourceBoneOrientation)
		{
			HierarchicalRotation.Matrix = sourceBoneOrientation.HierarchicalRotation.Matrix;
			HierarchicalRotation.Quaternion = sourceBoneOrientation.HierarchicalRotation.Quaternion;
			AbsoluteRotation.Matrix = sourceBoneOrientation.AbsoluteRotation.Matrix;
			AbsoluteRotation.Quaternion = sourceBoneOrientation.AbsoluteRotation.Quaternion;
		}
	}
}
