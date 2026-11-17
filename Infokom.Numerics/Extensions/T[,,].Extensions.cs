using System.Runtime.CompilerServices;

namespace Infokom.Numerics.Extensions
{
	public static class Array3D
	{
		extension<T>(T[,,] source)
		{
			public int Depth => source.GetLength(0);
			public int Height => source.GetLength(1);
			public int Width => source.GetLength(2);

		}
	}
}
