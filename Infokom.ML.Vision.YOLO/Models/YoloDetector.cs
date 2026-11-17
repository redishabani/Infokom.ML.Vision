using Infokom.ML.Vision.Atomics;
using Infokom.ML.Vision.Predictors;
using Infokom.Numerics.Atomics;

namespace Infokom.ML.Vision.YOLO.Models
{
	internal sealed class YoloDetector : IVisualPredictor<Feature<Coco8.Taxon>>, IVisualPredictorMetadata
	{
		private readonly object _gate = new();
		private readonly Detection.Inference.Session _session;
		private bool _isDisposed;

		public YoloDetector(string modelFileName)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(modelFileName);
			var fullPath = Path.GetFullPath(modelFileName);
			if (!File.Exists(fullPath))
				throw new FileNotFoundException("The ONNX model file was not found.", fullPath);

			_session = new Detection.Inference.Session(fullPath);
		}

		public string ModelInformation => _session.ModelInformation;

		public IReadOnlyList<Feature<Coco8.Taxon>> Predict(RGB[,] image)
		{
			ArgumentNullException.ThrowIfNull(image);
			lock (_gate)
			{
				ObjectDisposedException.ThrowIf(_isDisposed, this);
				return _session.Inject(image).AsReadOnly();
			}
		}

		public void Dispose()
		{
			lock (_gate)
			{
				if (_isDisposed)
					return;

				_isDisposed = true;
				_session.Dispose();
			}
		}
	}
}
