using Infokom.Numerics.Atomics;

namespace Infokom.Numerics.Extensions
{
	public static class ReadOnlySpanExtensions
	{
		public static string Stringify<T>(this ReadOnlySpan<T> source, string elementSeparator = ", ", string elementFormat = null, IFormatProvider elementFormatProvider = null) where T : IFormattable
		{
			Span<string> tokens = new string[source.Length];

			int i = 0;
			foreach (var element in source)
				tokens[i++] = element.ToString(elementFormat, elementFormatProvider);

			return string.Join(elementSeparator, tokens);
		}
	}

	
}
