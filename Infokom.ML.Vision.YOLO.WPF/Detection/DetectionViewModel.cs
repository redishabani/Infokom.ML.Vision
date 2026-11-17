using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Infokom.ML.Vision.Atomics;
using Infokom.ML.Vision.Predictors;
using Infokom.ML.Vision.Sensors;
using Infokom.ML.Vision.YOLO;
using Infokom.Numerics.Atomics;

using OpenCvSharp;

using System.Diagnostics;
using System.Threading.Channels;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Infokom.ML.Vision.YOLO.WPF.Detection
{
	public partial class DetectionViewModel : ObservableObject, IAsyncDisposable
	{
		private readonly Dispatcher _dispatcher;
		private readonly DispatcherTimer _diagnosticsTimer;
		private IVisualPredictor<Feature<Coco8.Taxon>> _predictor;
		private IVisualSensor _videoSensor;
		private CancellationTokenSource _playbackCancellation;
		private Task _playbackTask;
		private bool _videoEnded;
		private long _framePosition;
		private TimeSpan _previousCpuTime;
		private long _previousSampleTimestamp;
		private long _capturedFrames;
		private long _processedFrames;
		private double _lastInferenceMilliseconds;
		private int _lastDetectionCount;
		private bool _isDisposed;

		[ObservableProperty]
		private string _modelPath = string.Empty;

		[ObservableProperty]
		private string _videoPath = string.Empty;

		[ObservableProperty]
		private string _status = "Choose an ONNX detection model and a video file to begin.";

		[ObservableProperty]
		private string _diagnostics = "Detection FPS: 0.0 | inference: -- ms | CPU: 0% | working set: -- MB";

		[ObservableProperty]
		private string _modelInformation = "No model loaded.";

		[ObservableProperty]
		private ImageSource _originalFrame;

		[ObservableProperty]
		private ImageSource _detectedFrame;

		[ObservableProperty]
		private string _sourceFrameLabel = "Source frame: --";

		[ObservableProperty]
		private string _detectedFrameLabel = "Detection frame: --";

		public DetectionViewModel(Dispatcher dispatcher)
		{
			ArgumentNullException.ThrowIfNull(dispatcher);
			_dispatcher = dispatcher;
			using (var process = Process.GetCurrentProcess())
				_previousCpuTime = process.TotalProcessorTime;
			_previousSampleTimestamp = Stopwatch.GetTimestamp();

			_diagnosticsTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
			_diagnosticsTimer.Tick += UpdateDiagnostics;
			_diagnosticsTimer.Start();
		}

		[RelayCommand]
		private async Task LoadModelAsync()
		{
			if (string.IsNullOrWhiteSpace(ModelPath))
			{
				Status = "Choose an ONNX model before loading.";
				return;
			}

			await PausePlaybackAsync();
			Status = "Loading ONNX model...";
			IVisualPredictor<Feature<Coco8.Taxon>> loadedPredictor;
			try
			{
				loadedPredictor = await Task.Run(() => Yolo.CreateDetector(ModelPath));
			}
			catch (Exception error)
			{
				Status = $"Model load failed: {error.Message}";
				return;
			}

			if (_isDisposed)
			{
				loadedPredictor.Dispose();
				return;
			}

			var previousPredictor = _predictor;
			_predictor = loadedPredictor;
			previousPredictor?.Dispose();
			ModelInformation = (loadedPredictor as IVisualPredictorMetadata)?.ModelInformation ?? "The loaded predictor does not provide model metadata.";
			Status = "Model loaded. Choose a video and press Play.";
		}

		[RelayCommand]
		private void Play()
		{
			if (_predictor is null)
			{
				Status = "Load an ONNX model before starting detection.";
				return;
			}

			if (string.IsNullOrWhiteSpace(VideoPath))
			{
				Status = "Choose a video file before starting detection.";
				return;
			}

			if (_playbackTask is { IsCompleted: false })
				return;

			if (_videoEnded)
			{
				Status = "Video ended. Press Stop / replay to start it again.";
				return;
			}

			try
			{
				_videoSensor ??= VisualSensor.FromVideoFile(VideoPath);
				var fps = _videoSensor.FramesPerSecond;
				if (fps is not double frameRate || !double.IsFinite(frameRate) || frameRate <= 0)
					throw new InvalidOperationException($"The video reports an invalid frame rate: {fps}.");
				if (_videoSensor.FrameWidth <= 0 || _videoSensor.FrameHeight <= 0)
					throw new InvalidOperationException("The video does not report valid frame dimensions.");

				_playbackCancellation?.Dispose();
				_playbackCancellation = new CancellationTokenSource();
				var cancellationToken = _playbackCancellation.Token;
				var sensor = _videoSensor;
				var predictor = _predictor;
				_playbackTask = Task.Run(() => RunPlaybackAsync(sensor, predictor, frameRate, cancellationToken));
				Status = "Playing video and running detection.";
			}
			catch (Exception error)
			{
				Status = $"Could not start video playback: {error.Message}";
				_videoSensor?.Dispose();
				_videoSensor = null;
			}
		}

		[RelayCommand]
		private async Task PauseAsync()
		{
			await PausePlaybackAsync();
			Status = "Playback paused.";
		}

		[RelayCommand]
		private async Task StopAsync()
		{
			await PausePlaybackAsync();
			_videoSensor?.Dispose();
			_videoSensor = null;
			_videoEnded = false;
			_framePosition = 0;
			OriginalFrame = null;
			DetectedFrame = null;
			SourceFrameLabel = "Source frame: --";
			DetectedFrameLabel = "Detection frame: --";
			Status = "Playback stopped. Press Play to replay from the beginning.";
		}

		public async Task SetVideoPathAsync(string fileName)
		{
			await PausePlaybackAsync();
			_videoSensor?.Dispose();
			_videoSensor = null;
			_videoEnded = false;
			_framePosition = 0;
			VideoPath = fileName;
			OriginalFrame = null;
			DetectedFrame = null;
			SourceFrameLabel = "Source frame: --";
			DetectedFrameLabel = "Detection frame: --";
			Status = "Video selected. Load a model if needed, then press Play.";
		}

		public async ValueTask DisposeAsync()
		{
			if (_isDisposed)
				return;

			_isDisposed = true;
			_diagnosticsTimer.Stop();
			await PausePlaybackAsync();
			_videoSensor?.Dispose();
			_videoSensor = null;
			_predictor?.Dispose();
			_predictor = null;
		}

		private async Task PausePlaybackAsync()
		{
			var cancellation = _playbackCancellation;
			cancellation?.Cancel();
			var playbackTask = _playbackTask;
			if (playbackTask is not null)
			{
				try
				{
					await playbackTask;
				}
				catch (OperationCanceledException)
				{
				}
			}

			if (ReferenceEquals(_playbackTask, playbackTask))
				_playbackTask = null;
			if (ReferenceEquals(_playbackCancellation, cancellation))
			{
				_playbackCancellation = null;
				cancellation?.Dispose();
			}
		}

		private async Task RunPlaybackAsync(IVisualSensor sensor, IVisualPredictor<Feature<Coco8.Taxon>> predictor, double framesPerSecond, CancellationToken cancellationToken)
		{
			var channel = Channel.CreateBounded<CapturedFrame>(new BoundedChannelOptions(1)
			{
				FullMode = BoundedChannelFullMode.DropOldest,
				SingleReader = true,
				SingleWriter = true
			});
			using var pipelineCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			var pipelineToken = pipelineCancellation.Token;
			var captureTask = CaptureFramesAsync(sensor, channel.Writer, framesPerSecond, pipelineToken);
			var detectionTask = DetectFramesAsync(channel.Reader, predictor, pipelineToken);

			try
			{
				await Task.WhenAny(captureTask, detectionTask);
				if (captureTask.IsFaulted || detectionTask.IsFaulted)
					pipelineCancellation.Cancel();
				await Task.WhenAll(captureTask, detectionTask);
				if (!cancellationToken.IsCancellationRequested)
					_dispatcher.InvokeAsync(() => Status = "Video ended. Press Stop / replay to start it again.");
			}
			catch (OperationCanceledException) when (pipelineToken.IsCancellationRequested)
			{
			}
			catch (Exception error)
			{
				pipelineCancellation.Cancel();
				_dispatcher.InvokeAsync(() => Status = $"Video detection failed: {error.GetBaseException().Message}");
			}
		}

		private async Task CaptureFramesAsync(IVisualSensor sensor, ChannelWriter<CapturedFrame> writer, double framesPerSecond, CancellationToken cancellationToken)
		{
			Exception completionError = null;
			try
			{
				var frameInterval = TimeSpan.FromSeconds(1.0 / framesPerSecond);
				var startedAt = Stopwatch.GetTimestamp();
				var nextFrameAt = TimeSpan.Zero;
				while (!cancellationToken.IsCancellationRequested)
				{
					if (!sensor.TryGrabFrame(out var frame))
					{
						_videoEnded = true;
						return;
					}

					var framePosition = Interlocked.Increment(ref _framePosition);
					Interlocked.Increment(ref _capturedFrames);
					var captured = new CapturedFrame(framePosition, frame);
					ShowSourceFrame(captured);
					if (!writer.TryWrite(captured) && !cancellationToken.IsCancellationRequested)
						throw new InvalidOperationException("The inference frame queue is closed unexpectedly.");

					nextFrameAt += frameInterval;
					var delay = nextFrameAt - Stopwatch.GetElapsedTime(startedAt);
					if (delay > TimeSpan.Zero)
						await Task.Delay(delay, cancellationToken);
				}
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
			}
			catch (Exception error)
			{
				completionError = error;
				throw;
			}
			finally
			{
				writer.TryComplete(completionError);
			}
		}

		private async Task DetectFramesAsync(ChannelReader<CapturedFrame> reader, IVisualPredictor<Feature<Coco8.Taxon>> predictor, CancellationToken cancellationToken)
		{
			await foreach (var captured in reader.ReadAllAsync(cancellationToken))
			{
				cancellationToken.ThrowIfCancellationRequested();
				var inferenceTimer = Stopwatch.StartNew();
				var features = predictor.Predict(captured.Image);
				inferenceTimer.Stop();
				ShowPrediction(captured, features, inferenceTimer.Elapsed);
			}
		}

		private void ShowSourceFrame(CapturedFrame captured)
		{
			var bitmap = CreateBitmap(captured.Image);
			_dispatcher.InvokeAsync(() =>
			{
				OriginalFrame = bitmap;
				SourceFrameLabel = $"Source frame: {captured.Position} (video clock)";
			}, DispatcherPriority.Render);
		}

		private void ShowPrediction(CapturedFrame captured, IReadOnlyList<Feature<Coco8.Taxon>> features, TimeSpan inferenceTime)
		{
			var detectedImage = (RGB[,])captured.Image.Clone();
			detectedImage.Draw(features.ToList());
			var detected = CreateBitmap(detectedImage);

			Interlocked.Increment(ref _processedFrames);
			Interlocked.Exchange(ref _lastInferenceMilliseconds, inferenceTime.TotalMilliseconds);
			Interlocked.Exchange(ref _lastDetectionCount, features.Count);
			_dispatcher.InvokeAsync(() =>
			{
				DetectedFrame = detected;
				DetectedFrameLabel = $"Detection frame: {captured.Position} (same source frame)";
			}, DispatcherPriority.Render);
		}

		private readonly record struct CapturedFrame(long Position, RGB[,] Image);

		private static BitmapSource CreateBitmap(RGB[,] image)
		{
			var height = image.GetLength(0);
			var width = image.GetLength(1);
			var stride = checked(width * 3);
			var pixels = new byte[checked(stride * height)];
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					var (red, green, blue) = image[y, x];
					var offset = y * stride + x * 3;
					pixels[offset] = red;
					pixels[offset + 1] = green;
					pixels[offset + 2] = blue;
				}
			}

			var bitmap = BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb24, null, pixels, stride);
			bitmap.Freeze();
			return bitmap;
		}

		private void UpdateDiagnostics(object sender, EventArgs e)
		{
			using var process = Process.GetCurrentProcess();
			process.Refresh();
			var now = Stopwatch.GetTimestamp();
			var elapsed = Stopwatch.GetElapsedTime(_previousSampleTimestamp, now);
			var cpu = process.TotalProcessorTime - _previousCpuTime;
			_previousSampleTimestamp = now;
			_previousCpuTime = process.TotalProcessorTime;
			var capturedFrames = Interlocked.Exchange(ref _capturedFrames, 0);
			var processedFrames = Interlocked.Exchange(ref _processedFrames, 0);
			var sourceFps = elapsed > TimeSpan.Zero ? capturedFrames / elapsed.TotalSeconds : 0;
			var detectionFps = elapsed > TimeSpan.Zero ? processedFrames / elapsed.TotalSeconds : 0;
			var cpuPercent = elapsed > TimeSpan.Zero
				? 100 * cpu.TotalSeconds / (elapsed.TotalSeconds * Environment.ProcessorCount)
				: 0;
			var inferenceMilliseconds = Volatile.Read(ref _lastInferenceMilliseconds);
			var detectionCount = Volatile.Read(ref _lastDetectionCount);
			var memoryMegabytes = process.WorkingSet64 / (1024.0 * 1024.0);

			Diagnostics = $"Source FPS: {sourceFps:F1} | detection FPS: {detectionFps:F1} | inference: {inferenceMilliseconds:F1} ms | detections: {detectionCount} | CPU: {cpuPercent:F0}% | working set: {memoryMegabytes:F0} MB";
		}
	}
}
