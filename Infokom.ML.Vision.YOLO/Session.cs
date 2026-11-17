using Infokom.ML.Vision.Atomics;
using Infokom.ML.Vision.Predictors;
using Infokom.ML.Vision.YOLO.Models;
using Infokom.Numerics.Atomics;

namespace Infokom.ML.Vision.YOLO
{
	public static class Yolo
	{
		internal static Detection.Inference.Session Detect(FileInfo modelFile) => new(modelFile);
		internal static Detection.Inference.Session Detect(string modelFileName) => new(modelFileName);

		/// <summary>
		/// Creates a detector for a compatible YOLO ONNX detection model.
		/// </summary>
		public static IVisualPredictor<Feature<Coco8.Taxon>> CreateDetector(string modelFileName) => new YoloDetector(modelFileName);

		/// <summary>
		/// Creates a detector for a compatible YOLO ONNX detection model.
		/// </summary>
		public static IVisualPredictor<Feature<Coco8.Taxon>> CreateDetector(FileInfo modelFile)
		{
			ArgumentNullException.ThrowIfNull(modelFile);
			return new YoloDetector(modelFile.FullName);
		}
	}

	public abstract class Model
	{
	}
}
