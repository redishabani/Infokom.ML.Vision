//using static Infokom.Numerics.Atomics.FloatingPointConstant;
using Infokom.Numerics.Extensions;

using System.Numerics;
using System.Runtime.InteropServices;

namespace Infokom.Numerics.Atomics
{

	/// <summary>
	/// Binary data for ARGB32 color format.
	/// </summary>
	/// <remarks>
	/// 0xAA_RR_GG_BB
	/// </remarks>
	[StructLayout(LayoutKind.Explicit)]
	public readonly struct ARGB : IChromatic<ARGB>
	{
		private const float WR = 0.299f;
		private const float WG = 0.587f;
		private const float WB = 0.114f;
		private const float UMAX = 0.436f;
		private const float VMAX = 0.615f;


		private const uint B = 0x00_00_00_FF;
		private const uint G = 0x00_00_FF_00;
		private const uint R = 0x00_FF_00_00;
		private const uint A = 0xFF_00_00_00;

		[FieldOffset(0)] private readonly uint _data;
		[FieldOffset(0)] private readonly byte _r;
		[FieldOffset(1)] private readonly byte _g;
		[FieldOffset(2)] private readonly byte _b;
		[FieldOffset(3)] private readonly byte _a;

		private ARGB(uint data) => _data = data;

		public (float R, float G, float B) Color => (_r, _g, _b) / 255f;

		public float Luma
		{
			get
			{
				var (r, g, b) = this.Color;

				var y = WR * r + WG * g + WB * b;

				return y;
			}
		}

		public (float U, float V) Chroma
		{
			get
			{
				var (r, g, b) = this.Color;

				var y = this.Luma;
				var u = UMAX * (b - y) / (1 - WB);
				var v = VMAX * (r - y) / (1 - WR);

				return (u, v);
			}
		}


		public override int GetHashCode() => _data.GetHashCode();
		public bool Equals(ARGB target) => this._data.Equals(target._data);
		public override bool Equals(object obj) => obj is ARGB other && this.Equals(other);

		public static bool operator ==(ARGB source, ARGB target) => source._data == target._data;
		public static bool operator !=(ARGB source, ARGB target) => source._data != target._data;

		public static implicit operator ARGB(uint data) => new(data);
		public static implicit operator uint(ARGB color) => color._data;

		public static ARGB operator ~(ARGB x) => new(~x._data);
		public static ARGB operator &(ARGB x, ARGB y) => new(x._data & y._data);
		public static ARGB operator |(ARGB x, ARGB y) => new(x._data | y._data);
		public static ARGB operator ^(ARGB x, ARGB y) => new(x._data ^ y._data);
	}


}