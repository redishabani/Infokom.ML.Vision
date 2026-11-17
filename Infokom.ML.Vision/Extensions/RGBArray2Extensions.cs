using Infokom.Numerics.Atomics;
using Infokom.Numerics.Extensions;

using OpenCvSharp;

using CV = OpenCvSharp;

namespace Infokom.ML.Vision.Extensions
{	
	public static class RGBArray2Extensions
	{
		public static void CenterFit(this RGB[,] source, RGB[,] result, out float scale, out int padX, out int padY, bool cleanup = false)
		{
			int sourceHeight = source.GetLength(0);
			int sourceWidth = source.GetLength(1);

			int resultHeight = result.GetLength(0);
			int resultWidth = result.GetLength(1);

			if (sourceWidth == 0 || sourceHeight == 0 ||
			    resultWidth == 0 || resultHeight == 0)
				throw new ArgumentException("Image dimensions must be greater than zero.");

			if (ReferenceEquals(source, result))
				throw new ArgumentException("Source and result cannot reference the same array.");

			// Calculate aspect-preserving scale.
			scale = Math.Min(
			    (float)resultWidth / sourceWidth,
			    (float)resultHeight / sourceHeight);

			// Calculate resized dimensions.
			int width = Math.Min(resultWidth, (int)Math.Round(sourceWidth * scale));
			int height = Math.Min(resultHeight, (int)Math.Round(sourceHeight * scale));

			// Center offsets.
			padX = (resultWidth - width) / 2;
			padY = (resultHeight - height) / 2;


			// Initialize destination with black pixels.
			if(cleanup)
				Array.Clear(result);

			// Nearest-neighbor resize.
			for (int y = 0; y < height; y++)
			{
				int sy = (int)(((2L * y + 1) * sourceHeight) / (2L * height));

				for (int x = 0; x < width; x++)
				{
					int sx = (int)(((2L * x + 1) * sourceWidth) / (2L * width));

					result[y + padY, x + padX] = source[sy, sx];
				}
			}
		}

		public static RGB[,] CenterFit(this RGB[,] source, int sizeX, int sizeY, out float scale, out int resultPadX, out int resultPadY)
		{
			var result = new RGB[sizeY, sizeX];

			CenterFit(source, result, out scale, out resultPadX, out resultPadY);

			return result;
		}



		public static void Scale(this RGB[,] source, RGB[,] result)
		{
			var (sourceRows, sourceCols) = source.Size;
			var (resultRows, resultCols) = result.Size;

			using (var sourceMat = CV.Mat.FromPixelData(sourceRows, sourceCols, CV.MatType.CV_8UC3, source))
			{
				using (var resultMat = CV.Mat.FromPixelData(resultRows, resultCols, CV.MatType.CV_8UC3, result))
				{
					CV.Cv2.Resize(sourceMat, resultMat, new(resultCols, resultRows));
				}
			}
		}

		public static RGB[,] Scale(this RGB[,] source, int resultRows, int resultCols)
		{
			var result = new RGB[resultRows, resultCols];

			Scale(source, result);

			return result;
		}


		public static RGB[,] Scale(this RGB[,] source, float factor)
		{
			var (sourceRows, sourceCols) = source.Size;
			var (resultRows, resultCols) = ((int)(sourceRows * factor), (int)(sourceCols * factor));

			return Scale(source, resultRows, resultCols);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="resultSize">Greatest size between result rows and cols</param>
		/// <returns></returns>
		public static RGB[,] Scale(this RGB[,] source, int resultSize)
		{
			var (sourceRows, sourceCols) = source.Size;

			var factor = Math.Max(sourceRows, sourceCols) / resultSize;
			
			return source.Scale(factor);
		}




	}
}
