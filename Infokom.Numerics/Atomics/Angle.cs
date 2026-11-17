//using static Infokom.Numerics.Atomics.FloatingPointConstant;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

using static Infokom.Numerics.Extensions.Constants;

namespace Infokom.Numerics.Atomics
{

	public readonly struct Angle : IEquatable<Angle>, IFormattable
	{
		private const double DEPTH = 256;
		private const double ε = 1 / DEPTH;

		private readonly byte _data;		

		private Angle(byte data) => _data = data;



		public override string ToString() => $"{_data * τ * ε} [rad]";

		public string ToString(string format) => format switch
		{
			null => this.ToString(),
			"rad" => $"{_data * (2 * ε)}π [rad]",
			"deg" => $"{_data * ε * 360} [deg]",
			"rot" => $"{_data * ε} [rot]",
			_ => throw new FormatException($"Invalid format string: {format}"),
		};

		string IFormattable.ToString(string format, IFormatProvider formatProvider) => this.ToString(format);


		public override int GetHashCode() => _data;
		bool IEquatable<Angle>.Equals(Angle other) => _data == other._data;
		public override bool Equals(object obj) => obj is Angle other && ((IEquatable<Angle>)this).Equals(other);
		public static bool operator ==(Angle α, Angle β) => α._data == β._data;
		public static bool operator !=(Angle α, Angle β) => α._data != β._data;

		public static bool operator <(Angle α, Angle β) => α._data < β._data;
		public static bool operator >(Angle α, Angle β) => α._data > β._data;
		public static bool operator <=(Angle α, Angle β) => α._data <= β._data;
		public static bool operator >=(Angle α, Angle β) => α._data >= β._data;



		/// <summary>
		/// Angle its measurement in radians.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Angle Rad(double value) => new((byte)(value % DEPTH / (2 * π * ε) ));

		/// <summary>
		/// Angle from its measurement in degrees.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Angle Deg(double value) => new((byte)(value / (360 * ε)));

		/// <summary>
		/// Angle from its measurement in turns.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Angle Rot(double value) => new((byte)(value / ε));

		public static readonly Angle Epsilon = new(1);
		public static readonly Angle Minimum = new(byte.MinValue);
		public static readonly Angle Maximum = new(byte.MaxValue);



		public static implicit operator double(Angle source) => (source._data * (2 * π * ε));
		public static implicit operator Angle(double source) => Rad(source);
	}


	








	
}
