using Infokom.ML.Vision.Extensions;
using Infokom.ML.Vision.Sensors;
using Infokom.ML.Vision.YOLO.Models;
using Infokom.Numerics.Atomics;
using Infokom.Numerics.Extensions;

using OpenCvSharp;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Infokom.ML.Vision.YOLO.Models.Detection.Inference;
using static System.Net.Mime.MediaTypeNames;

namespace Infokom.ML.Vision.YOLO.Benchmarks
{
	internal class Program
	{
		private const string TIRANA_MP4 = @".\.data\tirana.mp4";
		private const string YOLO_NANO = @".\.data\yolo26n-1.640.640-1.300.6.onnx";


		[STAThread]
		static void Main()
		{
			Console.WriteLine(Unsafe.SizeOf<RGB>());
			Console.WriteLine(Marshal.SizeOf<RGB>());

			using (var video = VisualSensor.FromVideoFile(TIRANA_MP4))
			using (var model = Yolo.Detect(YOLO_NANO))
			{
				//CenterFitDemo(video);
				DetectDemo(video, model);
			}
		}

		private static void DetectDemo(IVisualSensor sensor, Detection.Inference.Session session)
		{
			var originalFps = sensor.FramesPerSecond;
			if (originalFps is not double fps || !double.IsFinite(fps) || fps <= 0)
				throw new InvalidOperationException($"The sensor reports an invalid frame rate: {originalFps}.");
			if (sensor.FrameWidth <= 0 || sensor.FrameHeight <= 0)
				throw new InvalidOperationException("The sensor does not report valid frame dimensions.");

			var frameIntervalMs = 1000.0 / fps;
			var frameStep = 1L;
			var nextFrameModulo = 0L;
			var framePosition = 0L;

			while (true)
			{
				var frameTimer = Stopwatch.StartNew();
				if (!sensor.TryGrabFrame(out var frame))
					break;

				framePosition++;
				if (framePosition % frameStep != nextFrameModulo)
					continue;

				
					
				Image.Show("Source", frame, 1);

				var features = session.Inject(frame);

				frame.Draw(features);

				Image.Show("Result", frame, 1);


				frameTimer.Stop();

				var frameProcessingMs = frameTimer.Elapsed.TotalMilliseconds;
				frameStep = Math.Max(1L, (long)Math.Ceiling(frameProcessingMs / frameIntervalMs));
				nextFrameModulo = framePosition % frameStep;

				var remainingDelayMs = frameStep * frameIntervalMs - frameProcessingMs;
				var waitMs = (int)Math.Clamp(Math.Ceiling(remainingDelayMs), 1, int.MaxValue);
				var key = Cv2.WaitKey(waitMs);
				if (key == 'q' || key == 'Q')
					break;
			}

			Cv2.DestroyAllWindows();
		}


		private static void DetectDemo(RGB[,] image, Detection.Inference.Session session)
		{
			
		}
	}


}
