namespace Infokom.Numerics
{
	public interface IMap<TSource, TResult>
	{
		public TResult this[TSource source] { get; }
	}


	public interface IMap<TSource, TTarget, TResult> : IMap<TSource, IMap<TTarget, TResult>>
	{
		public TResult this[TSource source, TTarget target] { get; }
	}

	public static class Map
	{
		public static void Foo(IMap<uint, uint, string> map)
		{
			var source = 1u;
			var target = 2u;
			var result = map[source][target];
		}
	}
}
