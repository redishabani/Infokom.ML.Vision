using System.Collections;
using System.Numerics;

namespace Infokom.Numerics.Atomics
{
	public readonly struct Line<Tx, Ty> : IReadOnlyList<Point<Tx, Ty>> where Tx : unmanaged, INumber<Tx> where Ty : unmanaged, INumber<Ty>
	{
		private readonly Tx _x1;
		private readonly Ty _y1;
		private readonly Tx _x2;
		private readonly Ty _y2;

		private Line(Tx x0, Ty y0, Tx x1, Ty y1) => (_x1, _y1, _x2, _y2) = (x0, y0, x1, y1);

		public Point<Tx, Ty> this[int index] => index switch
		{
			0 => (_x1, _y1),
			1 => (_x2, _y2),
			_ => throw new IndexOutOfRangeException($"{index}")
		};
		public int Count => 2;
		public IEnumerator<Point<Tx, Ty>> GetEnumerator()
		{
			yield return (_x1, _y1);
			yield return (_x2, _y2);
		}
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public static Line<Tx, Ty> Create(Point<Tx, Ty> p1, Point<Tx, Ty> p2) => new(p1.X, p1.Y, p2.X, p2.Y);
	}

	public static class Line
	{
		public static Line<T, T> Between<T>(Point<T, T> p1, Point<T, T> p2) where T : unmanaged, INumber<T>
		{
			if (p1.CompareTo(p2) > 0) (p1, p2) = (p2, p1);

			return Line<T, T>.Create(p1, p2);
		}
	}
}
