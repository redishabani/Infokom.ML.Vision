//using static Infokom.Numerics.Atomics.FloatingPointConstant;
using System.Drawing;
using System.Numerics;

namespace Infokom.Numerics
{
	/// <summary>
	/// χρωματικός (khrōmatikós; "colored")
	/// </summary>
	/// <typeparam name="TChromatic"></typeparam>
	public interface IChromatic<TChromatic> : IEquatable<TChromatic>, IEqualityOperators<TChromatic, TChromatic, bool> where TChromatic : IChromatic<TChromatic>
	{
		public float Luma { get; }

		public (float U, float V) Chroma { get; }

		public (float R, float G, float B) Color { get; }
	}
}