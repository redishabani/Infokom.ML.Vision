//using static Infokom.Numerics.Atomics.FloatingPointConstant;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Infokom.Numerics.Atomics
{
	[StructLayout(LayoutKind.Explicit)]
	public struct BitMatrix4x8
	{
		[FieldOffset(0)] private readonly uint _data;
		[FieldOffset(0)] private ByteVector4 _bytes;
		[FieldOffset(0)] private BitVector32 _bits;
		[FieldOffset(0)] public InlineArray4<BitVector8> Rows;




		/// <summary>
		/// Access a bit in this matrix by row and column index.
		/// </summary>
		/// <param name="i">The row index (0-3).</param>
		/// <param name="j">The column index (0-7).</param>
		/// <returns>The bit at the specified row and column.</returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public Bit this[int i, int j]
		{
			readonly get => i switch
			{
				0 => Rows[0][j],
				1 => Rows[1][j],
				2 => Rows[2][j],
				3 => Rows[3][j],
				_ => throw new ArgumentOutOfRangeException($"(R:{i}, C:{j})")
			};

			set
			{
				switch (i)
				{
					case 0: Rows[0][j] = value; break;
					case 1: Rows[1][j] = value; break;
					case 2: Rows[2][j] = value; break;
					case 3: Rows[3][j] = value; break;
					default: throw new ArgumentOutOfRangeException($"(R:{i}, C:{j})");
				}
			}
		}
	}
}