namespace System.Drawing.Extensions
{
	public static class ColorExtensions
	{
		private const float TWO_PI = 2 * MathF.PI; // 120 degrees in radians
		private const float TWO_PI_THIRDS = TWO_PI / 3; // 120 degrees in radians

		extension(Color)
		{
			/// <summary>
			/// Gets an RGB color from an angle in radians where R, G, and B are 120° apart.
			/// </summary>
			/// <param name="radians">Angle in radians (any positive or negative value).</param>
			/// <param name="maxBrightness">Optional brightness cap (0 to 255). Lowering this makes text easier to read on light backgrounds.</param>
			public static Color FromRgbWheel(float radians, int maxBrightness = 255)
			{
				maxBrightness = Math.Clamp(maxBrightness, 0, 255);

				// Shift each channel by 120 degrees (2π/3 radians)
				// Mapping cos(x) from [-1, 1] to [0, 1] via (cos(x) + 1) / 2
				var rNorm = (MathF.Cos(radians) + 1) / 2 ;
				var gNorm = (MathF.Cos(radians - TWO_PI_THIRDS) + 1) / 2;
				var bNorm = (MathF.Cos(radians - 2 * TWO_PI_THIRDS) + 1) / 2;
				
				int r = (int)Math.Round(rNorm * maxBrightness);
				int g = (int)Math.Round(gNorm * maxBrightness);
				int b = (int)Math.Round(bNorm * maxBrightness);

				return Color.FromArgb(r, g, b);
			}



			public static Color FromRgbWheel(float min, float max, float offset, int maxBrightness = 255)
			{
				var rad = (offset - min) / (max - min) * TWO_PI;

				return FromRgbWheel(rad, maxBrightness);
			}
		}
	}
}