using Infokom.Numerics.Atomics;

using OpenCvSharp;

namespace Infokom.ML.Vision.Sensors
{
	/// <summary>
	/// Reads RGB frames from a video file
	/// </summary>
	internal sealed class VideoFilePlayer : VisualSensorBase
	{
		private readonly VideoCapture _capture;
		private readonly int _frameWidth;
		private readonly int _frameHeight;
		private readonly double _framesPerSecond;
		private bool _isEndOfStream;

		public VideoFilePlayer(string fileName)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

			var fullPath = Path.GetFullPath(fileName);
			if (!File.Exists(fullPath))
				throw new FileNotFoundException("The video file was not found.", fullPath);

			_capture = new VideoCapture(fullPath);
			if (!_capture.IsOpened())
			{
				_capture.Dispose();
				throw new InvalidOperationException($"OpenCV could not open the video file '{fullPath}'.");
			}

			_frameWidth = _capture.FrameWidth;
			_frameHeight = _capture.FrameHeight;
			_framesPerSecond = _capture.Fps;
			if (_frameWidth <= 0 || _frameHeight <= 0)
			{
				_capture.Dispose();
				throw new InvalidOperationException($"The video file '{fullPath}' reports invalid frame dimensions.");
			}
		}

		/// <summary>
		/// Gets the frame width reported by the video.
		/// </summary>
		public override int FrameWidth => _frameWidth;

		/// <summary>
		/// Gets the frame height reported by the video.
		/// </summary>
		public override int FrameHeight => _frameHeight;

		/// <summary>
		/// Gets the frame rate reported by the video, if available.
		/// </summary>
		public override double? FramesPerSecond => double.IsFinite(_framesPerSecond) && _framesPerSecond > 0 ? _framesPerSecond : null;

		protected override bool IsEndOfStream => _isEndOfStream;

		protected override bool TryReadFrame(out RGB[,] frame, int millisecondsWait)
		{
			frame = null;
			if (_isEndOfStream)
				return false;

			var nextFrame = new RGB[_frameHeight, _frameWidth];
			using (var frameMat = Mat.FromPixelData(_frameHeight, _frameWidth, MatType.CV_8UC3, nextFrame))
			{
				if (!_capture.Read(frameMat))
				{
					_isEndOfStream = true;
					return false;
				}
			}

			frame = nextFrame;
			return true;
		}

		protected override void DisposeSource() => _capture.Dispose();
	}
}
