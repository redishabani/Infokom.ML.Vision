using Infokom.Numerics.Atomics;

using System.Collections;
using System.Runtime.CompilerServices;

namespace Infokom.Numerics.Extensions
{

	public static class InlineArray2Extensions
	{
		extension<TValue>(InlineArray2<TValue> source)
		{
			public void Deconstruct(out TValue value0, out TValue value1)
			{
				value0 = source[0];
				value1 = source[1];
			}
		}
	}

	public static class InlineArray3Extensions
	{
		extension<TValue>(InlineArray3<TValue> source)
		{
			public void Deconstruct(out TValue value0, out TValue value1, out TValue value2)
			{
				value0 = source[0];
				value1 = source[1];
				value2 = source[2];
			}
		}

		
	}

	public static class InlineArray
	{
		public static InlineArray5<T> Create<T>(T element0, T element1, T element2, T element3, T element4)
		{
			var result = new InlineArray5<T>();

			result[0] = element0;
			result[1] = element1;
			result[2] = element2;
			result[3] = element3;
			result[4] = element4;

			return result;
		}

		public static InlineArray4<T> Create<T>(T element0, T element1, T element2, T element3)
		{
			var result = new InlineArray4<T>();

			result[0] = element0;
			result[1] = element1;
			result[2] = element2;
			result[3] = element3;

			return result;
		}

		public static InlineArray3<T> Create<T>(T element0, T element1, T element2)
		{
			var result = new InlineArray3<T>();

			result[0] = element0;
			result[1] = element1;
			result[2] = element2;

			return result;
		}

		public static InlineArray2<T> Create<T>(T element0, T element1)
		{
			var result = new InlineArray2<T>();

			result[0] = element0;
			result[1] = element1;
			
			return result;
		}


		public static InlineArray2<T> ToInlineArray<T>(this ValueTuple<T, T> source) => Create(source.Item1, source.Item2);
		public static InlineArray3<T> ToInlineArray<T>(this ValueTuple<T, T, T> source) => Create(source.Item1, source.Item2, source.Item3);
		public static InlineArray4<T> ToInlineArray<T>(this ValueTuple<T, T, T, T> source) => Create(source.Item1, source.Item2, source.Item3, source.Item4);
		public static InlineArray5<T> ToInlineArray<T>(this ValueTuple<T, T, T, T, T> source) => Create(source.Item1, source.Item2, source.Item3, source.Item4, source.Item5);

	}

	public static partial class Array2D
	{
		[InlineArray(2)]
		public struct Size
		{
			private int _0;
		}

		extension(Array)
		{
			public static T[,] Create<T>(int length0, int length1) => new T[length0, length1];

			public static T[,] Create<T>(int length0, int length1, T value) 
			{
				var array = Create<T>(length0, length1);

				if (!EqualityComparer<T>.Default.Equals(value, default(T)))
				{
					for (int index0 = 0; index0 < length0; index0++)
					{
						for (int index1 = 0; index1 < length1; index1++)
						{
							array[index0, index1] = value;
						}
					}
				}

				return array;
			}

			public static T[,] Create<T>(int length0, int length1, Func<int, int, T> source)
			{
				var array = Array.Create<T>(length0, length1);
				
				if (source != null)
				{
					for (int index0 = 0; index0 < length0; index0++)
					{
						for (int index1 = 0; index1 < length1; index1++)
						{
							array[index0, index1] = source(index0, index1);
						}
					}
				}

				return array;
			}
		}

		extension<T>(T[,] source)
		{
			public Size<int, int> Size => (source.GetLength(1), source.GetLength(0));

			public int Rows => source.GetLength(0);

			public int Cols => source.GetLength(1);

			public bool IsEmpty => source.Length == 0;
		}
	}
}
