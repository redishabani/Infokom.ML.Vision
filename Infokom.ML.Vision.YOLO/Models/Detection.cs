using CommunityToolkit.HighPerformance;

using Infokom.ML.Vision.Atomics;
using Infokom.ML.Vision.Extensions;
using Infokom.Numerics.Atomics;
using Infokom.Numerics.Extensions;

using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

using OpenCvSharp;

using System.Collections;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Infokom.ML.Vision.YOLO.Models.Detection;
using static System.Net.Mime.MediaTypeNames;

namespace Infokom.ML.Vision.YOLO.Models
{
	internal class Detection
	{

		private Detection(ImmutableArray<Feature<Coco8.Taxon>> feaures) => this.Feaures = feaures;

		public ImmutableArray<Feature<Coco8.Taxon>> Feaures { get; }





		public class Frame
		{
			public Frame()
			{

			}
		}


		public class Source : DenseTensor<float>, IReadOnlyCollection<NamedOnnxValue>
		{
			private readonly NamedOnnxValue _onnxValue;

			public Source() : base([1, 3, 640, 640])
			{
				_onnxValue = NamedOnnxValue.CreateFromTensor("images", this);
			}
			
			int IReadOnlyCollection<NamedOnnxValue>.Count => 1;
			IEnumerator<NamedOnnxValue> IEnumerable<NamedOnnxValue>.GetEnumerator() { yield return _onnxValue; }
			IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<NamedOnnxValue>)this).GetEnumerator();
		}

		public class Result : DenseTensor<float>, IReadOnlyCollection<NamedOnnxValue>
		{
			private readonly NamedOnnxValue _onnxValue;

			public Result() : base([1,300,6])
			{
				_onnxValue = NamedOnnxValue.CreateFromTensor("output0", this);
			}
			
			int IReadOnlyCollection<NamedOnnxValue>.Count => 1;
			IEnumerator<NamedOnnxValue> IEnumerable<NamedOnnxValue>.GetEnumerator() { yield return _onnxValue; }
			IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<NamedOnnxValue>)this).GetEnumerator();
		}





		

		public class Inference
		{
			private RGB[,] _image;

			/// <summary>
			/// Injected image width
			/// </summary>
			private int _sizeX;

			/// <summary>
			/// Injected image height
			/// </summary>
			private int _sizeY;

			/// <summary>
			/// Scale applied to the injected image in order to fit in 640x640
			/// </summary>
			private float _scale;

			/// <summary>
			/// Padding along x resulted after the injected imagewas scaled in order to fit in 640x640
			/// </summary>
			private int _paddX;

			/// <summary>
			/// Padding along y resulted after the injected imagewas scaled in order to fit in 640x640
			/// </summary>
			private int _paddY;





			private readonly Source _source;

			private readonly Result _result;

			private Inference()
			{
				_source = new();
				_result = new();
			}

			public void Inject(RGB[,] image)
			{
				ArgumentNullException.ThrowIfNull(image);
				if (image.Rows == 0 || image.Cols == 0)
					throw new ArgumentException("Image dimensions must be greater than zero.", nameof(image));

				_image = image;
				_sizeX = image.Cols;
				_sizeY = image.Rows;

				var pixels = image.CenterFit(640, 640, out _scale, out _paddX, out _paddY);

				for (int y = 0; y < 640; y++)
				{
					for (int x = 0; x < 640; x++)
					{
						var (r, g, b) = pixels[y, x].Color;
						_source[0, 0, y, x] = r;
						_source[0, 1, y, x] = g;
						_source[0, 2, y, x] = b;
					}
				}
			}

			public List<Feature<Coco8.Taxon>> Extract()
			{
				var results = new List<Feature<Coco8.Taxon>>(300);

				for (int i = 0; i < 300; i++)
				{
					float score = _result[0, i, 4];
					if (!float.IsFinite(score) || score < 0.3f)
						continue;

					float classValue = _result[0, i, 5];
					if (!float.IsFinite(classValue))
						continue;

					int classId = (int)classValue;
					if (classId < 0 || classId >= Coco8.Taxon.Count)
						continue;

					float x1 = (_result[0, i, 0] - _paddX) / _scale;
					float y1 = (_result[0, i, 1] - _paddY) / _scale;
					float x2 = (_result[0, i, 2] - _paddX) / _scale;
					float y2 = (_result[0, i, 3] - _paddY) / _scale;
					if (!float.IsFinite(x1) || !float.IsFinite(y1) || !float.IsFinite(x2) || !float.IsFinite(y2) || x2 <= x1 || y2 <= y1)
						continue;

					x1 = Math.Clamp(x1, 0, _sizeX);
					y1 = Math.Clamp(y1, 0, _sizeY);
					x2 = Math.Clamp(x2, 0, _sizeX);
					y2 = Math.Clamp(y2, 0, _sizeY);
					if (x2 <= x1 || y2 <= y1)
						continue;

					var group = Coco8.Taxon.FromId(classId);
					var bbox = new Box<float, float>(new((x2 + x1) / 2, (y2 + y1) / 2), new(x2 - x1, y2 - y1));

					results.Add(new(group, bbox, score));
				}

				return results;
			}



			public class Session : IDisposable
			{
				const string SOURCE_NAME = "images", RESULT_NAME = "output0";

				private readonly Inference _inference;
				private readonly InferenceSession _session;

				public Session(FileInfo modelFile)
				{
					ArgumentNullException.ThrowIfNull(modelFile);

					_session = new InferenceSession(modelFile.FullName);
					_inference = new();
					ModelInformation = CreateModelInformation(modelFile.Name);
				}

				public Session(string modelFileName)
				{
					ArgumentException.ThrowIfNullOrWhiteSpace(modelFileName);

					_session = new InferenceSession(modelFileName);
					_inference = new();
					ModelInformation = CreateModelInformation(Path.GetFileName(modelFileName));
				}

				public string ModelInformation { get; }

				private string CreateModelInformation(string modelName)
				{
					var metadata = _session.ModelMetadata;
					var inputs = string.Join(Environment.NewLine, _session.InputMetadata.Select(item =>
						$"  {item.Key}: {item.Value.ElementType.Name}[{string.Join(", ", item.Value.Dimensions)}]"));
					var outputs = string.Join(Environment.NewLine, _session.OutputMetadata.Select(item =>
						$"  {item.Key}: {item.Value.ElementType.Name}[{string.Join(", ", item.Value.Dimensions)}]"));
					var lines = new List<string>
					{
						$"File: {modelName}",
						$"Graph: {metadata.GraphName}",
						$"Producer: {metadata.ProducerName}",
						$"Domain: {metadata.Domain}",
						$"Version: {metadata.Version}",
						$"Description: {metadata.Description}",
						$"Inputs:{Environment.NewLine}{inputs}",
						$"Outputs:{Environment.NewLine}{outputs}"
					};
					if (metadata.CustomMetadataMap.Count > 0)
						lines.Add($"Custom metadata: {string.Join(", ", metadata.CustomMetadataMap.Select(item => $"{item.Key}={item.Value}"))}");

					return string.Join(Environment.NewLine, lines);
				}

				public void Dispose() => _session.Dispose();


				public List<Feature<Coco8.Taxon>> Inject(RGB[,] image)
				{
					_inference.Inject(image);

					_session.Run(_inference._source, _inference._result);

					var features = _inference.Extract();

					return features;
				}
			}
		}
	}
}
