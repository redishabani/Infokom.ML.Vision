using System.Collections;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Numerics.Atomics
{
	public interface ITaxonomy<TTaxonomy> : IEquatable<TTaxonomy>, IComparable<TTaxonomy> where TTaxonomy : unmanaged, ITaxonomy<TTaxonomy>
	{
		public int Id { get; }

		public string Name { get; }


		bool IEquatable<TTaxonomy>.Equals(TTaxonomy other) => this.Id.Equals(other.Id);

		int IComparable<TTaxonomy>.CompareTo(TTaxonomy other) => this.Id.CompareTo(other.Id);



		public static abstract int Count { get; }

		public static abstract TTaxonomy FromId(int source);

		public static abstract TTaxonomy FromName(string source);
	}

	public interface ITaxonomic<TTaxonomy> where TTaxonomy : unmanaged, ITaxonomy<TTaxonomy>
	{
		public TTaxonomy Group { get; }
	}


	public interface IGeometry<TGeometry> : IEquatable<TGeometry>, IComparable<TGeometry> where TGeometry : unmanaged, IGeometry<TGeometry>
	{

	}

	public interface IGeometric<TGeometry>
	{
		public TGeometry Shape { get; }
	}


	public interface IGeometry<TGeometry, Tx, Ty> : IReadOnlyList<Point<Tx, Ty>> where TGeometry : unmanaged, IGeometry<TGeometry, Tx, Ty> where Tx : unmanaged, INumber<Tx> where Ty : unmanaged, INumber<Ty>
	{
	}

	public interface IGeometric<TGeometry, Tx, Ty> where TGeometry : unmanaged, IGeometry<TGeometry, Tx, Ty> where Tx : unmanaged, INumber<Tx> where Ty : unmanaged, INumber<Ty>
	{
		public TGeometry Shape { get; }
	}




	public interface IConfident<TConfidence> where TConfidence : IBinaryFloatingPointIeee754<TConfidence>
	{
		public TConfidence Score { get; }
	}

	

	[StructLayout(LayoutKind.Sequential)]
	public readonly partial record struct Box<Tx, Ty>(Point<Tx, Ty> Center, Size<Tx, Ty> Size) where Tx : unmanaged, INumber<Tx> where Ty : unmanaged, INumber<Ty>
	{

		/// <summary>
		/// Empty <see cref="Box{Tx, Ty}">box</see> with center in <see cref="Box{Tx, Ty}.Center">(0, 0)</see>
		/// </summary>
		public static readonly Box<Tx, Ty> Zero;



		public void Deconstruct(out Tx x, out Ty y, out Tx w, out Ty h) => ((x, y), (w, h)) = (this.Center, this.Size);

		/// <summary>
		/// Create empty box at a given center
		/// </summary>
		/// <param name="x">X coordinate of the center</param>
		/// <param name="y">Y coordinate of the center</param>
		/// <returns>Empty <see cref="Box{Tx, Ty}">box</see> with center in <see cref="Box{Tx, Ty}.Center">(<paramref name="x"/>, <paramref name="y"/>)</see> </returns>
		public static Box<Tx, Ty> Empty(Tx x, Ty y) => new((x, y), default);


		public static implicit operator Box<Tx, Ty>(((Tx, Ty), (Tx, Ty)) source)
		{
			var (c, s) = source;

			return new(c, s);
		}

		public static implicit operator Box<Tx, Ty>((Tx, Ty, Tx, Ty) source)
		{
			var (x, y, w, h) = source;

			return new((x, y), (w, h));
		}
	}


	public readonly partial record struct Box<Tx, Ty> : IGeometry<Box<Tx, Ty>, Tx, Ty>
	{
		public Point<Tx, Ty> this[int index] => index switch
		{
			0 => (this.Center.X + this.Size.X / Tx.CreateChecked(2), this.Center.Y + this.Size.Y / Ty.CreateChecked(2)),
			1 => (this.Center.X - this.Size.X / Tx.CreateChecked(2), this.Center.Y + this.Size.Y / Ty.CreateChecked(2)),
			2 => (this.Center.X - this.Size.X / Tx.CreateChecked(2), this.Center.Y - this.Size.Y / Ty.CreateChecked(2)),
			3 => (this.Center.X + this.Size.X / Tx.CreateChecked(2), this.Center.Y - this.Size.Y / Ty.CreateChecked(2)),
			_ => throw new IndexOutOfRangeException()
		};

		public int Count => 4;

		public struct Enumerator : IEnumerator<Point<Tx, Ty>>
		{
			private readonly Box<Tx, Ty> _owner;
			private int _offset;
			public Enumerator(Box<Tx, Ty> owner) => _owner = owner;
			public readonly Point<Tx, Ty> Current => _owner[_offset];
			readonly object IEnumerator.Current => this.Current;
			public bool MoveNext() => ++_offset < 4;
			public void Reset() => _offset = -1;
			readonly void IDisposable.Dispose() { }
		}
		public IEnumerator<Point<Tx, Ty>> GetEnumerator() => throw new NotImplementedException();
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}


	[StructLayout(LayoutKind.Sequential)]
	public readonly record struct Box<Tx, Ty, Tz>(Point<Tx, Ty, Tz> Center, Size<Tx, Ty, Tz> Size) where Tx : unmanaged, INumber<Tx> where Ty : unmanaged, INumber<Ty> where Tz : unmanaged, INumber<Tz>
	{

		/// <summary>
		/// Empty <see cref="Box{Tx, Ty, Tz}">box</see> with center in <see cref="Box{Tx, Ty, Tz}.Center">(0, 0)</see>
		/// </summary>
		public static readonly Box<Tx, Ty, Tz> Zero;



		public void Deconstruct(out Point<Tx, Ty, Tz> center, out Size<Tx, Ty, Tz> size) => (center, size) = (this.Center, this.Size);

		public void Deconstruct(out Tx x, out Ty y, out Tz z, out Tx w, out Ty h, out Tz d) => ((x, y, z), (w, h, d)) = (this.Center, this.Size);

		/// <summary>
		/// Create empty box at a given center
		/// </summary>
		/// <param name="x">X coordinate of the center</param>
		/// <param name="y">Y coordinate of the center</param>
		/// <param name="z">Z coordinate of the center</param>
		/// <returns>Empty <see cref="Box{Tx, Ty, Tz}">box</see> with center in <see cref="Box{Tx, Ty, Tz}.Center">(<paramref name="x"/>, <paramref name="y"/>, <paramref name="z"/>)</see> </returns>
		public static Box<Tx, Ty, Tz> Empty(Tx x, Ty y, Tz z) => new((x, y, z), default);


		public static implicit operator Box<Tx, Ty, Tz>(((Tx, Ty, Tz), (Tx, Ty, Tz)) source)
		{
			var (c, s) = source;

			return new(c, s);
		}

		public static implicit operator Box<Tx, Ty, Tz>((Tx, Ty, Tz, Tx, Ty, Tz) source)
		{
			var (x, y, z, w, h, d) = source;

			return new((x, y, z), (w, h, d));
		}
	}


	public static class Box
	{
		public static Box<T, T> Empty<T>(T x, T y) where T : unmanaged, INumber<T> => Box<T, T>.Empty(x, y);



		public static bool Overlaps<Tx, Ty>(Box<Tx, Ty> a, Box<Tx, Ty> b) where Tx : unmanaged, INumber<Tx> where Ty : unmanaged, INumber<Ty>
		{
			var (ax, ay, aw, ah) = a;
			var (bx, by, bw, bh) = b;
			return ax - aw / Tx.One < bx + bw / Tx.One && ax + aw / Tx.One > bx - bw / Tx.One && ay - ah / Ty.One < by + bh / Ty.One && ay + ah / Ty.One > by - bh / Ty.One;
		}

		/// <summary>
		/// Calculates the intersection-over-union ratio of two boxes.
		/// </summary>
		/// <typeparam name="T">The numeric type used for the box coordinates, dimensions, and result.</typeparam>
		/// <param name="a">The first box, specified by its center coordinates and dimensions.</param>
		/// <param name="b">The second box, specified by its center coordinates and dimensions.</param>
		/// <returns>The intersection area divided by the union area, or zero if the union area is not positive.</returns>
		/// <remarks>
		/// Intersection over Union (IoU) is an evaluation metric used  to measure how well a 
		/// source bounding box overlaps with a target bounding box. It produces a value between 0 and 1, where 0 indicates no overlap and 1 indicates perfect overlap.
		/// </remarks>
		public static float IoU<T>(Box<T, T> a, Box<T, T> b) where T : unmanaged, INumber<T>
		{
			var ((ax, ay), (aw, ah)) = a;
			var ((bx, by), (bw, bh)) = b;

			var w1 = float.CreateChecked(aw);
			var h1 = float.CreateChecked(ah);
			var x1 = float.CreateChecked(ax) - w1 / 2.0f; // Top-left X of box a
			var y1 = float.CreateChecked(ay) - h1 / 2.0f; // Top-left Y of box a

			var w2 = float.CreateChecked(bw);
			var h2 = float.CreateChecked(bh);
			var x2 = float.CreateChecked(bx) - w2 / 2.0f; // Top-left X of box b
			var y2 = float.CreateChecked(by) - h2 / 2.0f	; // Top-left Y of box b

			// 1. Find the top-left and bottom-right corners of the intersection rectangle
			var capX1 = Math.Max(x1, x2);
			var capY1 = Math.Max(y1, y2);
			var capX2 = Math.Min(x1 + w1, x2 + w2);
			var capY2 = Math.Min(y1 + h1, y2 + h2);

			// 2. Calculate intersection dimensions (clamped to 0 if there is no overlap)
			var capW = Math.Max(0, capX2 - capX1);
			var capH = Math.Max(0, capY2 - capY1);

			// 3. Compute intersection and union areas
			var cap = capW * capH;
			if (cap == 0) return 0;

			var cup = (w1 * h1) + (w2 * h2) - cap;

			// 4. Return the IoU ratio
			return cup > 0 ? cap / cup : 0;
		}



	}
}
