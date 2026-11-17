using System.Numerics;
using System.Runtime.InteropServices;

namespace Infokom.Numerics.Atomics
{
	[StructLayout(LayoutKind.Sequential)]
	public readonly record struct Size<Tx> where Tx : unmanaged, INumber<Tx>
	{
		private readonly Tx _x;

		private Size(Tx x) => _x = x;

		public Tx X => _x;

		public override string ToString() => $"({_x})";

		public static Size<Tx> Create(Tx x)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(x, nameof(x));

			return new(x);
		}


		public static bool operator <(Size<Tx> a, Size<Tx> b) => a._x < b._x;
		public static bool operator <=(Size<Tx> a, Size<Tx> b) => a._x <= b._x;
		public static bool operator >(Size<Tx> a, Size<Tx> b) => a._x > b._x;
		public static bool operator >=(Size<Tx> a, Size<Tx> b) => a._x >= b._x;

		public static bool operator <(Size<Tx> a, Tx b) => a._x < b;
		public static bool operator <=(Size<Tx> a, Tx b) => a._x <= b;
		public static bool operator >(Size<Tx> a, Tx b) => a._x > b;
		public static bool operator >=(Size<Tx> a, Tx b) => a._x >= b;

		public static bool operator <(Tx a, Size<Tx> b) => a < b._x;
		public static bool operator <=(Tx a, Size<Tx> b) => a <= b._x;
		public static bool operator >(Tx a, Size<Tx> b) => a > b._x;
		public static bool operator >=(Tx a, Size<Tx> b) => a >= b._x;

		public static implicit operator Size<Tx>(Tx x) => new(x);
	}

	[StructLayout(LayoutKind.Sequential)]
	public readonly struct Size<Tx, Ty> where Tx : unmanaged, INumber<Tx> where Ty : unmanaged, INumber<Ty>
	{
		private readonly Tx _x;
		private readonly Ty _y;

		public Size(Tx x, Ty y) => (_x, _y) = (x, y);

		public Tx X => _x;

		public Ty Y => _y;

		public void Deconstruct(out Tx x, out Ty y) => (x, y) = (_x, _y);

		public override string ToString() => $"({_x} × {_y})";

		public int Dimension => Tx.IsNegative(this.X) && Ty.IsNegative(this.Y) ? -2 :
			Tx.IsNegative(this.X) || Ty.IsNegative(this.Y) ? -1 :
			Tx.IsZero(this.X) && Ty.IsZero(this.Y) ? 0 :
			Tx.IsZero(this.X) || Ty.IsZero(this.Y) ? 1 : 2;

		public static implicit operator Size<Tx, Ty>(ValueTuple<Tx, Ty> source) => new(source.Item1, source.Item2);
	}

	public readonly struct Size<Tx, Ty, Tz> where Tx : unmanaged, INumber<Tx> where Ty : unmanaged, INumber<Ty> where Tz : unmanaged, INumber<Tz>
	{
		private readonly Tx _x;
		private readonly Ty _y;
		private readonly Tz _z;

		private Size(Tx x, Ty y, Tz z) => (_x, _y, _z) = (x, y, z);

		public Tx X => _x;

		public Ty Y => _y;

		public Tz Z => _z;

		public void Deconstruct(out Tx x, out Ty y, out Tz z) => (x, y, z) = (_x, _y, _z);

		public override string ToString() => $"{_x} × {_y} × {_z}";

		public static Size<Tx, Ty, Tz> Create(Tx x, Ty y, Tz z)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(x, nameof(x));
			ArgumentOutOfRangeException.ThrowIfNegative(y, nameof(y));
			ArgumentOutOfRangeException.ThrowIfNegative(z, nameof(z));

			return new(x, y, z);
		}

		public static implicit operator Size<Tx, Ty, Tz>(ValueTuple<Tx, Ty, Tz> source) => new(source.Item1, source.Item2, source.Item3);
	}

	public static class Size
	{
		extension<T>(Size<T, T> source) where T : unmanaged, INumber<T>
		{
			public T Width => source.X;

			public T Height => source.Y;
		}

		extension<T>(Size<T, T, T> source) where T : unmanaged, INumber<T>
		{
			public T Width => source.X;

			public T Height => source.Y;

			public T Depth => source.Z;
		}



		extension<T>(Size<T, T> source) where T : unmanaged, IBinaryInteger<T>
		{
			public T RowCount => source.Y;

			public T ColumnCount => source.X;

		}

		extension<T>(Size<T, T, T> source) where T : unmanaged, IBinaryInteger<T>
		{
			public T RowCount => source.Y;

			public T ColumnCount => source.X;

			public T ChannelCount => source.Z;
		}
	}
}