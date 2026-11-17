using Infokom.Numerics.Atomics;

namespace Infokom.ML.Vision.Sensors
{
	/// <summary>
	/// Provides visual frames through direct polling or an observable stream.
	/// </summary>
	public interface IVisualSensor : IObservable<RGB[,]>, IDisposable
	{
		/// <summary>
		/// Gets the width, in pixels, of frames produced by this sensor.
		/// </summary>
		int FrameWidth { get; }

		/// <summary>
		/// Gets the height, in pixels, of frames produced by this sensor.
		/// </summary>
		int FrameHeight { get; }

		/// <summary>
		/// Gets the source frame rate in frames per second, or null when the source does not report it.
		/// </summary>
		double? FramesPerSecond { get; }

		/// <summary>
		/// Attempts to retrieve the next frame. Returns false when no frame is available before the timeout
		/// or when a finite source has ended.
		/// </summary>
		bool TryGrabFrame(out RGB[,] frame, int millisecondsWait = 0);
	}
}
