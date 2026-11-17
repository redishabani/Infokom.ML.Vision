using Infokom.Numerics.Atomics;

using System.Diagnostics;
using System.Reactive.Linq;

namespace Infokom.ML.Vision.Predictors
{
	/// <summary>
	/// Provides descriptive information about the loaded prediction model.
	/// </summary>
	public interface IVisualPredictorMetadata
	{
		string ModelInformation { get; }
	}

	/// <summary>
	/// Predicts features from RGB images and can process an observable stream of images.
	/// </summary>
	/// <typeparam name="TFeature">The feature type produced for each image.</typeparam>
	public interface IVisualPredictor<TFeature> : IDisposable
	{
		/// <summary>
		/// Predicts the features present in an image.
		/// </summary>
		IReadOnlyList<TFeature> Predict(RGB[,] image);

		/// <summary>
		/// Transforms an image stream into predictions while preserving each source image and its inference duration.
		/// </summary>
		IObservable<VisualPrediction<TFeature>> Observe(IObservable<RGB[,]> images)
		{
			ArgumentNullException.ThrowIfNull(images);
			return images.Select(image =>
			{
				var startedAt = Stopwatch.GetTimestamp();
				var features = this.Predict(image);
				return new VisualPrediction<TFeature>(image, features, Stopwatch.GetElapsedTime(startedAt));
			});
		}
	}

	/// <summary>
	/// The prediction produced for one source image.
	/// </summary>
	public readonly record struct VisualPrediction<TFeature>(RGB[,] Image, IReadOnlyList<TFeature> Features, TimeSpan InferenceTime);
}
