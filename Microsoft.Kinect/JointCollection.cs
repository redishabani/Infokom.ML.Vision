using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This class contains a collection of joints returned for a given skeleton.
	/// </summary>
	[Serializable]
	public class JointCollection : IEnumerable<Joint>, IEnumerable
	{
		/// <summary>
		/// The array of joint data.
		/// </summary>
		private readonly Joint[] _skeletonData;

		/// <summary>
		/// Back reference to Skeleton class to enable us to flag writes to joints and re-calculate joint angles.
		/// </summary>
		[NonSerialized]
		[XmlIgnore]
		private readonly Skeleton _skeletonBackReference;

		/// <summary>
		/// Gets the number of joints available.
		/// </summary>
		public int Count => _skeletonData.Length;

		/// <summary>
		/// Accesses the requested joint.
		/// </summary>
		/// <param name="jointType">The JointType being requested.</param>
		/// <returns>The requested joint.</returns>
		public Joint this[JointType jointType]
		{
			get
			{
				return _skeletonData[(int)jointType];
			}
			set
			{
				if (value.JointType != jointType)
				{
					throw new ArgumentException(Resources.IncorrectJointType);
				}
				if (_skeletonBackReference != null)
				{
					_skeletonBackReference.InvalidateBoneOrientations();
				}
				_skeletonData[(int)jointType] = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the JointCollection class.
		/// </summary>
		/// <param name="backReference">Back reference to Skeleton class.</param>
		internal JointCollection(Skeleton backReference)
		{
			_skeletonData = new Joint[Skeleton.JointTypeValues.Length];
			_skeletonBackReference = backReference;
		}

		/// <summary>
		/// This method is used to enumerate the list of joints.
		/// </summary>
		/// <returns>The related enumerator.</returns>
		public IEnumerator GetEnumerator()
		{
			return _skeletonData.GetEnumerator();
		}

		/// <summary>
		/// This method is used to enumerate the list of joints.
		/// </summary>
		/// <returns>The related enumerator.</returns>
		IEnumerator<Joint> IEnumerable<Joint>.GetEnumerator()
		{
			return ((IEnumerable<Joint>)_skeletonData).GetEnumerator();
		}
	}
}
