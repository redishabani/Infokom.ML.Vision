using System.Runtime.CompilerServices;

namespace Infokom.Numerics
{
	public interface IMatrix<T>
	{
		public T this[int index0, int index1] { get; }

		public T this[InlineArray2<int> index] { get; }

		public InlineArray2<int> Size { get; }
	}


	public class Matrix
	{
		
	}
}
