using Infokom.Numerics.Atomics;
using Infokom.Numerics.Extensions;

using OpenCvSharp;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Infokom.ML.Vision
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
	public static class Image
	{
		public static void Show(string windowName, RGB[,] image, int delay = 0)
		{
			if (image is null || image.IsEmpty)
				throw new ArgumentNullException(nameof(image), "Must not be null or empty.");

			Cv2.ImShow(windowName, image);
			Cv2.WaitKey(delay);
		}
	}
}
