using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This class contains a collection of bone orientations returned for a given skeleton.
	/// </summary>
	public class BoneOrientationCollection : IEnumerable<BoneOrientation>, IEnumerable
	{
		/// <summary>
		/// The array of joint data.
		/// </summary>
		private readonly BoneOrientation[] _boneOrientations;

		/// <summary>
		/// Indicates whether the bone orientation data is valid (has been recalculated since the joint positions last changed).
		/// </summary>
		private bool _isValid;

		/// <summary>
		/// Back reference to Skeleton class to enable us to re-calculate joint angles.
		/// </summary>
		[NonSerialized]
		[XmlIgnore]
		private readonly Skeleton _skeletonBackReference;

		/// <summary>
		/// Gets the number of bone orientations available.
		/// </summary>
		public int Count => _boneOrientations.Length;

		/// <summary>
		/// Gets or sets a value indicating whether the joint orientations are valid.
		/// </summary>
		internal bool IsValid
		{
			get
			{
				return _isValid;
			}
			set
			{
				_isValid = value;
			}
		}

		/// <summary>
		/// Accesses the requested bone orientation.
		/// </summary>
		/// <param name="jointType">The JointType for the bone being requested.</param>
		/// <returns>The requested bone orientation.</returns>
		public BoneOrientation this[JointType jointType]
		{
			get
			{
				Recalculate();
				return _boneOrientations[(int)jointType];
			}
			set
			{
				if (value == null || value.EndJoint != jointType)
				{
					throw new ArgumentException(Resources.IncorrectJointType);
				}
				Recalculate();
				_isValid = true;
				_boneOrientations[(int)jointType] = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the BoneOrientationCollection class.
		/// </summary>
		/// <param name="backReference">Back reference to Skeleton class.</param>
		internal BoneOrientationCollection(Skeleton backReference)
		{
			_boneOrientations = new BoneOrientation[Skeleton.JointTypeValues.Length];
			foreach (JointType jointTypeValue in Skeleton.JointTypeValues)
			{
				_boneOrientations[(int)jointTypeValue] = new BoneOrientation(jointTypeValue);
			}
			_skeletonBackReference = backReference;
		}

		/// <summary>
		/// This method is used to enumerate the list of bone orientations.
		/// </summary>
		/// <returns>The related enumerator.</returns>
		public IEnumerator GetEnumerator()
		{
			Recalculate();
			return _boneOrientations.GetEnumerator();
		}

		/// <summary>
		/// This method is used to enumerate the list of bone orientations.
		/// </summary>
		/// <returns>The related enumerator.</returns>
		IEnumerator<BoneOrientation> IEnumerable<BoneOrientation>.GetEnumerator()
		{
			Recalculate();
			return ((IEnumerable<BoneOrientation>)_boneOrientations).GetEnumerator();
		}

		/// <summary>
		/// Recalculates the bone orientations.
		/// </summary>
		internal void Recalculate()
		{
			if (!_isValid && _skeletonBackReference != null)
			{
				_isValid = _skeletonBackReference.CalculateBoneOrientations(_boneOrientations);
			}
		}

		/// <summary>
		/// Invalidates the bone orientations, forcing them to be recalculated the next time they are requested.
		/// </summary>
		internal void Invalidate()
		{
			_isValid = false;
		}
	}
}
