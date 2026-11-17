using Infokom.Numerics.Atomics;

using Microsoft.Kinect;

namespace Infokom.ML.Vision.Sensors
{
	/// <summary>
	/// Reads color frames from a connected Kinect sensor and exposes them as RGB images.
	/// </summary>
	internal sealed class KinectColorCamera : VisualSensorBase
	{
		private const ColorImageFormat COLOR_FORMAT = ColorImageFormat.RgbResolution640x480Fps30;
		private readonly KinectSensor _sensor;
		private bool _ownsColorStream;
		private bool _ownsSensorStart;

		/// <summary>
		/// Creates a camera for the first connected Kinect sensor.
		/// </summary>
		public KinectColorCamera() : this(null)
		{
		}

		/// <summary>
		/// Creates a camera for a connected Kinect sensor selected by its device or unique ID.
		/// </summary>
		public KinectColorCamera(string sensorId)
		{
			var sensors = KinectSensor.KinectSensors;
			_sensor = string.IsNullOrWhiteSpace(sensorId)
				? sensors.FirstOrDefault(sensor => sensor.Status == KinectStatus.Connected)
				: sensors[sensorId];

			if (_sensor is null || _sensor.Status != KinectStatus.Connected)
				throw new InvalidOperationException("No connected Kinect sensor was found.");
		}

		public override int FrameWidth => 640;

		public override int FrameHeight => 480;

		public override double? FramesPerSecond => 30;

		protected override bool TryReadFrame(out RGB[,] frame, int millisecondsWait)
		{
			frame = null;
			EnsureColorStreamStarted();

			using var colorFrame = _sensor.ColorStream.OpenNextFrame(millisecondsWait);
			if (colorFrame is null)
				return false;

			if (colorFrame.Format != COLOR_FORMAT || colorFrame.BytesPerPixel != 4)
				throw new InvalidOperationException($"Unsupported Kinect color frame format: {colorFrame.Format}.");

			var sourcePixels = colorFrame.Data;
			var expectedLength = checked(colorFrame.Width * colorFrame.Height * colorFrame.BytesPerPixel);
			if (sourcePixels is null || sourcePixels.Length != expectedLength)
				throw new InvalidOperationException("The Kinect returned an invalid color frame buffer.");

			var rgbFrame = new RGB[colorFrame.Height, colorFrame.Width];
			for (int y = 0; y < colorFrame.Height; y++)
			{
				for (int x = 0; x < colorFrame.Width; x++)
				{
					int offset = (y * colorFrame.Width + x) * colorFrame.BytesPerPixel;
					rgbFrame[y, x] = (sourcePixels[offset + 2], sourcePixels[offset + 1], sourcePixels[offset]);
				}
			}

			frame = rgbFrame;
			return true;
		}

		protected override void DisposeSource()
		{
			if (_ownsColorStream && _sensor.ColorStream.IsEnabled)
				_sensor.ColorStream.Disable();

			if (_ownsSensorStart && _sensor.IsRunning)
				_sensor.Stop();
		}

		private void EnsureColorStreamStarted()
		{
			if (_sensor.Status != KinectStatus.Connected)
				throw new InvalidOperationException("The Kinect sensor is not connected.");

			if (_sensor.IsRunning)
			{
				if (!_sensor.ColorStream.IsEnabled)
				{
					_sensor.ColorStream.Enable(COLOR_FORMAT);
					_ownsColorStream = true;
				}
				else if (_sensor.ColorStream.Format != COLOR_FORMAT)
				{
					throw new InvalidOperationException($"The Kinect color stream is already enabled with format {_sensor.ColorStream.Format}.");
				}

				return;
			}

			if (!_sensor.ColorStream.IsEnabled)
			{
				_sensor.ColorStream.Enable(COLOR_FORMAT);
				_ownsColorStream = true;
			}
			else if (_sensor.ColorStream.Format != COLOR_FORMAT)
			{
				throw new InvalidOperationException($"The Kinect color stream is already enabled with format {_sensor.ColorStream.Format}.");
			}

			_sensor.Start();
			_ownsSensorStart = true;
		}
	}
}
