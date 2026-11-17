using Infokom.ML.Vision.Atomics;
using Infokom.Numerics.Atomics;
using Infokom.Numerics.Extensions;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace OpenCvSharp
#pragma warning restore IDE0130 // Namespace does not match folder structure
{	
	public static class MatExtensions
	{
		extension(Cv2)
		{
			public static void CenterFit(Mat source, Mat result, out float scale, out int padX, out int padY)
			{
				var w0 = source.Width; var h0 = source.Height;
				var w1 = result.Width; var h1 = result.Height;

				// Calculate the scaling ratio
				scale = Math.Min((float)w1 / w0, (float)h1 / h0);

				// Compute new unpadded dimensions
				int w = (int)Math.Round(source.Width * scale); int h = (int)Math.Round(source.Height * scale);

				// Calculate total padding needed and split evenly
				float dw = (w1 - w) / 2.0f; float dh = (h1 - h) / 2.0f;

				// Resize if dimensions changed
				using (var scaled = new Mat())
				{
					if (w0 != w || h0 != h)
					{
						InterpolationFlags interp = scale < 1.0f ? InterpolationFlags.Area : InterpolationFlags.Linear;
						Cv2.Resize(source, scaled, new Size(w, h), 0, 0, interp);
					}
					else
					{
						source.CopyTo(scaled);
					}

					// Calculate exact integer borders to avoid off-by-one rounding discrepancies
					int y0 = (int)Math.Round(dh - 0.1f);
					int y1 = (int)Math.Round(dh + 0.1f);
					int x0 = (int)Math.Round(dw - 0.1f);
					int x1 = (int)Math.Round(dw + 0.1f);

					// Store the top-left padding offset for downstream coordinate mapping
					(padX, padY) = (x0, y0);

					// Apply border padding				
					//Cv2.CopyMakeBorder(scaled, result, y0, y1, x0, x1, BorderTypes.Constant, Scalar.Black);
				}
			}
		}

		public static void Draw(this Mat image, ReadOnlySpan<Feature> features)
		{
			foreach (var feature in features)
			{
				var (r, g, b) = RGB.Wheel(feature.Group * 10.0);
				var color = new Scalar(b, g, r);
				DrawFeature(image, feature.BoundingBox, color, feature.ToString());
			}
		}
		

		public static void Draw<TTaxonomy>(this Mat image, List<Feature<TTaxonomy>> features) where TTaxonomy : unmanaged, ITaxonomy<TTaxonomy>
		{
			foreach (var feature in features)
			{
				var (r, g, b) = RGB.Wheel(feature.Group.Id * 10.0 / TTaxonomy.Count);
				var color = new Scalar(b, g, r);
				DrawFeature(image, feature.BoundingBox, color, feature.ToString());
			}
		}

		private static void DrawFeature(Mat image, Box<float, float> boundingBox, Scalar color, string label)
		{
			if (image.Empty())
				return;

			var ((x, y), (w, h)) = boundingBox;
			if (!float.IsFinite(x) || !float.IsFinite(y) || !float.IsFinite(w) || !float.IsFinite(h) || w <= 0 || h <= 0)
				return;

			float left = Math.Clamp(x - w / 2, 0, image.Cols);
			float top = Math.Clamp(y - h / 2, 0, image.Rows);
			float right = Math.Clamp(x + w / 2, 0, image.Cols);
			float bottom = Math.Clamp(y + h / 2, 0, image.Rows);
			if (right <= left || bottom <= top)
				return;

			int rectX = (int)Math.Floor(left);
			int rectY = (int)Math.Floor(top);
			int rectW = (int)Math.Ceiling(right) - rectX;
			int rectH = (int)Math.Ceiling(bottom) - rectY;
			var rectangle = new Rect(rectX, rectY, rectW, rectH);

			using (var fillRoi = new Mat(image, rectangle))
			using (var fillOverlay = new Mat(fillRoi.Size(), fillRoi.Type(), color))
				Cv2.AddWeighted(fillOverlay, 0.25, fillRoi, 0.75, 0, fillRoi);

			Cv2.Rectangle(image, rectangle, color, thickness: 2);

			var fontFace = HersheyFonts.HersheySimplex;
			const double fontScale = 0.5;
			const int thickness = 1;
			const int padding = 4;
			var textSize = Cv2.GetTextSize(label, fontFace, fontScale, thickness, out int baseline);

			int bgW = Math.Min(textSize.Width + padding * 2, image.Cols - rectX);
			int bgH = Math.Min(textSize.Height + baseline + padding * 2, image.Rows - rectY);
			if (bgW > 0 && bgH > 0)
			{
				var bgRect = new Rect(rectX, rectY, bgW, bgH);
				using var bgRoi = new Mat(image, bgRect);
				using var bgOverlay = new Mat(bgRoi.Size(), bgRoi.Type(), color);
				Cv2.AddWeighted(bgOverlay, 0.5, bgRoi, 0.5, 0, bgRoi);
			}

			var textOrigin = new Point(
				Math.Min(rectX + padding, image.Cols - 1),
				Math.Min(rectY + textSize.Height + padding, image.Rows - 1));
			Cv2.PutText(image, label, textOrigin, fontFace, fontScale, Scalar.White, thickness, LineTypes.AntiAlias);
		}

		public static void Draw<TTaxonomy>(this RGB[,] image, List<Feature<TTaxonomy>> features) where TTaxonomy : unmanaged, ITaxonomy<TTaxonomy>
		{
			var (w, h) = image.Size;
			using(var imageMat = Mat.FromPixelData(h, w, MatType.CV_8UC3, image))
			{
				imageMat.Draw(features);
			}
		}
	}

	public static class Cv2Extensions
	{
		extension(Cv2)
		{
			public static void ImShow(string winName, RGB[,] image)
			{
				var (w, h) = image.Size;
				using (var imageMat = Mat.FromPixelData(h, w, MatType.CV_8UC3, image))
				{
					Cv2.ImShow(winName, imageMat);
				}
			}
		}
	}
}
