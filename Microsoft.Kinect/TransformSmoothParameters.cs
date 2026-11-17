using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This struct is used to setup the skeleton smoothing values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 24)]
	[DebuggerDisplay("Smoothing:{Smoothing} Correction:{Correction} Prediction:{Prediction} JitterRadius:{JitterRadius} MaxDeviationRadius:{MaxDeviationRadius}")]
	public struct TransformSmoothParameters
	{
		/// <summary>
		/// Gets or sets the smoothing value.
		/// </summary>
		public float Smoothing { get; set; }

		/// <summary>
		/// Gets or sets the correction value.
		/// </summary>
		public float Correction { get; set; }

		/// <summary>
		/// Gets or sets the prediction value.
		/// </summary>
		public float Prediction { get; set; }

		/// <summary>
		/// Gets or sets the jitter radius value.
		/// </summary>
		public float JitterRadius { get; set; }

		/// <summary>
		/// Gets or sets the maximum deviation radius value.
		/// </summary>
		public float MaxDeviationRadius { get; set; }

		/// <summary>
		/// Calculates the hash code of the TransformSmoothParameters.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			return Smoothing.GetHashCode() + Correction.GetHashCode() + Prediction.GetHashCode() + JitterRadius.GetHashCode() + MaxDeviationRadius.GetHashCode();
		}

		/// <summary>
		/// Determines if the two objects are equal.
		/// </summary>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>This method returns true if they are equal and false otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is TransformSmoothParameters))
			{
				return false;
			}
			return Equals((TransformSmoothParameters)obj);
		}

		/// <summary>
		/// Determines if the two objects are equal.
		/// </summary>
		/// <param name="smoothParameters">The TransformSmoothParameters to compare to.</param>
		/// <returns>This method returns true if they are equal and false otherwise.</returns>
		public bool Equals(TransformSmoothParameters smoothParameters)
		{
			if (Smoothing.Equals(smoothParameters.Smoothing) && Correction.Equals(smoothParameters.Correction) && Prediction.Equals(smoothParameters.Prediction) && JitterRadius.Equals(smoothParameters.JitterRadius))
			{
				return MaxDeviationRadius.Equals(smoothParameters.MaxDeviationRadius);
			}
			return false;
		}

		/// <summary>
		/// Determines if the two objects are equal.
		/// </summary>
		/// <param name="smoothParameters1">The first TransformSmoothParamters to compare.</param>
		/// <param name="smoothParameters2">The second TransformSmoothParamters to compare.</param>
		/// <returns>This method returns true if they are equal and false otherwise.</returns>
		public static bool operator ==(TransformSmoothParameters smoothParameters1, TransformSmoothParameters smoothParameters2)
		{
			return smoothParameters1.Equals(smoothParameters2);
		}

		/// <summary>
		/// Determines if the two objects are not equal.
		/// </summary>
		/// <param name="smoothParameters1">The first TransformSmoothParamters to compare.</param>
		/// <param name="smoothParameters2">The second TransformSmoothParamters to compare.</param>
		/// <returns>This method returns true if they are not equal and false otherwise.</returns>
		public static bool operator !=(TransformSmoothParameters smoothParameters1, TransformSmoothParameters smoothParameters2)
		{
			return !smoothParameters1.Equals(smoothParameters2);
		}
	}
}
