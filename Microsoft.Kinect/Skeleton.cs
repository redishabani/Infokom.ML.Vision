using System;
using System.Xml.Serialization;
using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This class is used to track all of the known data about a skeleton
	/// returned from the skeleton stream.
	/// </summary>
	[Serializable]
	public class Skeleton
	{
		/// <summary>
		/// An array of all possible JointType values.
		/// </summary>
		internal static readonly Array JointTypeValues = Enum.GetValues(typeof(JointType));

		/// <summary>
		/// Backing store for the BoneOrientations property.
		/// </summary>
		[NonSerialized]
		[XmlIgnore]
		private BoneOrientationCollection _boneOrientations;

		/// <summary>
		/// Gets or sets the skeleton's current tracking state.
		/// </summary>
		public SkeletonTrackingState TrackingState { get; set; }

		/// <summary>
		/// Gets or sets the skeleton's tracking ID.
		/// </summary>
		public int TrackingId { get; set; }

		/// <summary>
		/// Gets or sets the skeleton's position.
		/// </summary>
		public SkeletonPoint Position { get; set; }

		/// <summary>
		/// Gets or sets the skeleton's joints.
		/// </summary>
		public JointCollection Joints { get; protected set; }

		/// <summary>
		/// Gets or sets the skeleton's bone orientations.
		/// </summary>
		public BoneOrientationCollection BoneOrientations
		{
			get
			{
				if (_boneOrientations == null)
				{
					_boneOrientations = new BoneOrientationCollection(this);
				}
				_boneOrientations.Recalculate();
				return _boneOrientations;
			}
			protected set
			{
				_boneOrientations = value;
			}
		}

		/// <summary>
		/// Gets or sets the edges that this skeleton is clipped on.
		/// </summary>
		public FrameEdges ClippedEdges { get; set; }

		/// <summary>
		/// Initializes a new instance of the Skeleton class.
		/// </summary>
		/// <param name="nativeSkeleton">The native skeleton data.</param>
		internal Skeleton(_NUI_SKELETON_DATA nativeSkeleton)
		{
			CopyFrom(nativeSkeleton);
		}

		/// <summary>
		/// Initializes a new instance of the Skeleton class.
		/// </summary>
		/// <param name="sourceData">The source skeleton to copy the data from.</param>
		internal Skeleton(Skeleton sourceData)
		{
			CopyFrom(sourceData);
		}

		/// <summary>
		/// Initializes a new instance of the Skeleton class with a default JointCollection.
		/// </summary>
		public Skeleton()
		{
			Joints = new JointCollection(this);
			Joint value = default(Joint);
			foreach (JointType jointTypeValue in JointTypeValues)
			{
				JointType jointType = (value.JointType = jointTypeValue);
				Joints[jointType] = value;
			}
		}

		/// <summary>
		/// This method copies the data from the native data.
		/// </summary>
		/// <param name="nativeSkeleton">The native skeleton data.</param>
		internal void CopyFrom(_NUI_SKELETON_DATA nativeSkeleton)
		{
			TrackingState = (SkeletonTrackingState)nativeSkeleton.eTrackingState;
			TrackingId = (int)nativeSkeleton.dwTrackingID;
			Position = new SkeletonPoint
			{
				X = nativeSkeleton.Position.x,
				Y = nativeSkeleton.Position.y,
				Z = nativeSkeleton.Position.z
			};
			ClippedEdges = (FrameEdges)nativeSkeleton.dwQualityFlags;
			Joints = new JointCollection(this);
			Joint value = default(Joint);
			foreach (JointType jointTypeValue in JointTypeValues)
			{
				JointType jointType = (value.JointType = jointTypeValue);
				value.TrackingState = (JointTrackingState)nativeSkeleton.eSkeletonPositionTrackingState[(int)jointType];
				_Vector4 vector = nativeSkeleton.SkeletonPositions[(int)jointType];
				value.Position = new SkeletonPoint
				{
					X = vector.x,
					Y = vector.y,
					Z = vector.z
				};
				Joints[jointType] = value;
			}
		}

		/// <summary>
		/// This method copies the data from another skeleton.
		/// </summary>
		/// <param name="sourceData">The source skeleton.</param>
		internal void CopyFrom(Skeleton sourceData)
		{
			TrackingState = sourceData.TrackingState;
			TrackingId = sourceData.TrackingId;
			Position = sourceData.Position;
			ClippedEdges = sourceData.ClippedEdges;
			if (Joints == null)
			{
				Joints = new JointCollection(this);
			}
			foreach (JointType jointTypeValue in JointTypeValues)
			{
				Joint value = new Joint
				{
					Position = sourceData.Joints[jointTypeValue].Position,
					JointType = sourceData.Joints[jointTypeValue].JointType,
					TrackingState = sourceData.Joints[jointTypeValue].TrackingState
				};
				Joints[jointTypeValue] = value;
			}
			if (sourceData._boneOrientations != null && sourceData._boneOrientations.IsValid)
			{
				if (_boneOrientations == null)
				{
					_boneOrientations = new BoneOrientationCollection(this);
				}
				_boneOrientations.IsValid = true;
				{
					foreach (JointType jointTypeValue2 in JointTypeValues)
					{
						BoneOrientation boneOrientation = new BoneOrientation(jointTypeValue2);
						boneOrientation.CopyFrom(sourceData._boneOrientations[jointTypeValue2]);
						_boneOrientations[jointTypeValue2] = boneOrientation;
					}
					return;
				}
			}
			_boneOrientations = null;
		}

		/// <summary>
		/// Invalidates the bone orientations, forcing them to be recalculated the next time they are requested.
		/// </summary>
		internal void InvalidateBoneOrientations()
		{
			if (_boneOrientations != null)
			{
				_boneOrientations.Invalidate();
			}
		}

		/// <summary>
		/// This method calculates the hierarchical and absolute bone rotations for each joint in the skeleton.
		/// </summary>
		/// <param name="boneOrientations">The array of bone orientations to be updated.</param>
		/// <returns>Returns true if joint calculation successful, false otherwise.</returns>
		internal bool CalculateBoneOrientations(BoneOrientation[] boneOrientations)
		{
			if (TrackingState != SkeletonTrackingState.NotTracked && (Joints[JointType.HipCenter].TrackingState != JointTrackingState.NotTracked || Joints[JointType.ShoulderCenter].TrackingState != JointTrackingState.NotTracked))
			{
				_NUI_SKELETON_DATA pSkeletonData = new _NUI_SKELETON_DATA
				{
					eTrackingState = _NUI_SKELETON_TRACKING_STATE.NUI_SKELETON_TRACKED,
					dwTrackingID = (uint)TrackingId,
					Position = 
					{
						x = Position.X,
						y = Position.Y,
						z = Position.Z
					},
					eSkeletonPositionTrackingState = new _NUI_SKELETON_POSITION_TRACKING_STATE[JointTypeValues.Length],
					SkeletonPositions = new _Vector4[JointTypeValues.Length]
				};
				foreach (JointType jointTypeValue in JointTypeValues)
				{
					if (Joints[jointTypeValue].TrackingState == JointTrackingState.NotTracked)
					{
						pSkeletonData.eSkeletonPositionTrackingState[(int)jointTypeValue] = _NUI_SKELETON_POSITION_TRACKING_STATE.NUI_SKELETON_POSITION_NOT_TRACKED;
					}
					else if (Joints[jointTypeValue].TrackingState == JointTrackingState.Inferred)
					{
						pSkeletonData.eSkeletonPositionTrackingState[(int)jointTypeValue] = _NUI_SKELETON_POSITION_TRACKING_STATE.NUI_SKELETON_POSITION_INFERRED;
					}
					else
					{
						pSkeletonData.eSkeletonPositionTrackingState[(int)jointTypeValue] = _NUI_SKELETON_POSITION_TRACKING_STATE.NUI_SKELETON_POSITION_TRACKED;
					}
					pSkeletonData.SkeletonPositions[(int)jointTypeValue].x = Joints[jointTypeValue].Position.X;
					pSkeletonData.SkeletonPositions[(int)jointTypeValue].y = Joints[jointTypeValue].Position.Y;
					pSkeletonData.SkeletonPositions[(int)jointTypeValue].z = Joints[jointTypeValue].Position.Z;
				}
				_NUI_SKELETON_BONE_ORIENTATION[] array = new _NUI_SKELETON_BONE_ORIENTATION[JointTypeValues.Length];
				int num = NativeMethods.NuiSkeletonCalculateBoneOrientations(ref pSkeletonData, array);
				foreach (JointType jointTypeValue2 in JointTypeValues)
				{
					boneOrientations[(int)jointTypeValue2].CopyFrom(ref array[(int)jointTypeValue2]);
				}
				if (num == 0)
				{
					return true;
				}
			}
			foreach (BoneOrientation boneOrientation in boneOrientations)
			{
				boneOrientation.SetDefaults();
			}
			return false;
		}
	}
}
