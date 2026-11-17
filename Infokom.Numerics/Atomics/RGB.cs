//using static Infokom.Numerics.Atomics.FloatingPointConstant;
using Infokom.Numerics.Extensions;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Infokom.Numerics.Extensions.Constants;
using static Infokom.Numerics.Extensions.Functions;

namespace Infokom.Numerics.Atomics
{
	/// <summary>
	/// Binary data for RGB 24 bit color format.
	/// </summary>
	/// <remarks>
	/// 0xRR_GG_BB
	/// </remarks>
	[StructLayout(LayoutKind.Explicit, Size = 3)]
	public readonly struct RGB : IChromatic<RGB>
	{
		private const float WR = 0.299f;
		private const float WG = 0.587f;
		private const float WB = 0.114f;
		private const float UMAX = 0.436f;
		private const float VMAX = 0.615f;


		private const uint B = 0x00_00_FF;
		private const uint G = 0x00_FF_00;
		private const uint R = 0xFF_00_00;

		[FieldOffset(0)] private readonly byte _b;
		[FieldOffset(1)] private readonly byte _g;
		[FieldOffset(2)] private readonly byte _r;

		[FieldOffset(0)] private readonly InlineArray3<BitVector8> _channels;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private RGB(uint data) => (_r, _g, _b) = ((byte)(data >> 16), (byte)(data >> 8), (byte)data);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private RGB(byte r, byte g, byte b) => (_r, _g, _b) = (r, g, b);

		public (float R, float G, float B) Color
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (_r, _g, _b) / 255f;
		}

		public float Luma
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				var (r, g, b) = this.Color;

				var y = WR * r + WG * g + WB * b;

				return y;
			}
		}

		public (float U, float V) Chroma
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				var (r, g, b) = this.Color;

				var y = this.Luma;
				var u = UMAX * (b - y) / (1 - WB);
				var v = VMAX * (r - y) / (1 - WR);

				return (u, v);
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Deconstruct(out byte r, out byte g, out byte b)
		{
			r = _r;
			g = _g;
			b = _b;
		}

		public override string ToString() => $"#{_r:X2}{_g:X2}{_b:X2}";

		override public int GetHashCode() => HashCode.Combine(_r, _g, _b);
		public bool Equals(RGB other) => _r.Equals(other._r) && _g.Equals(other._g) && _b.Equals(other._b);
		override public bool Equals(object obj) => obj is RGB other && this.Equals(other);
		public static explicit operator RGB(uint data) => new(data);
		public static explicit operator checked RGB(uint data) => (data & 0xFF000000) == 0 ? new RGB(data) : throw new OverflowException($"{data} causes overflow", new ArgumentOutOfRangeException(nameof(data), "Value must be a valid RGB24 color data."));
		public static implicit operator uint(RGB source) => (uint)(source._r << 16) | (uint)(source._g << 8) | (uint)source._b;

		public static implicit operator RGB(ValueTuple<byte, byte, byte> source) => new(source.Item1, source.Item2, source.Item3);


		/// <summary>
		/// Creates a new RGB24 color from normalized float values for red, green, and blue channels.
		/// </summary>
		/// <param name="r">The red component (0.0 to 1.0).</param>
		/// <param name="g">The green component (0.0 to 1.0).</param>
		/// <param name="b">The blue component (0.0 to 1.0).</param>
		/// <returns>A new RGB24 color.</returns>
		/// <remarks>
		/// (r,g,b) is assumed to be normalized so that their sum equals 1.0. If not, a normalization step is performed to ensure the resulting color is valid.
		/// </remarks>
		public static RGB Create(float r, float g, float b)
		{
			(r, g, b) = 255f * (r, g, b) / (r + g + b);

			return new((byte)r, (byte)g, (byte)b);
		}

		public static RGB Create(double r, double g, double b)
		{
			(r, g, b) = 255 * (r, g, b) / (r + g + b);

			return new((byte)r, (byte)g, (byte)b);
		}



		public static RGB operator ~(RGB x) => new(~(uint)x & 0xFFFFFF);
		public static RGB operator &(RGB x, RGB y) => new((uint)x & (uint)y);
		public static RGB operator |(RGB x, RGB y) => new((uint)x | (uint)y);
		public static RGB operator ^(RGB x, RGB y) => new((uint)x ^ (uint)y);
		public static bool operator ==(RGB x, RGB y) => (uint)x == (uint)y;
		public static bool operator !=(RGB x, RGB y) => (uint)x != (uint)y;




		/// <summary>
		/// 
		/// </summary>
		/// <param name="phi"></param>
		/// <param name="rho"></param>
		/// <returns></returns>
		public static RGB Wheel(Angle phi, float rho = 1)
		{
			rho = Math.Clamp(rho, 0, 1);

			// Shift each channel by 120 degrees (2π/3 radians)
			// Mapping cos(x) from [-1, 1] to [0, 1] via (cos(x) + 1) / 2
			var r = rho * (cos(phi                ) + 1) / 2;//phase shift   0
			var g = rho * (cos(phi -     2 * π / 3) + 1) / 2;//phase shift 120
			var b = rho * (cos(phi - 2 * 2 * π / 3) + 1) / 2;//phase shift 240


			return RGB.Create(r, g, b);
		}
	}


}