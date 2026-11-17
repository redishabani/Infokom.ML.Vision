using Infokom.Numerics.Atomics;

using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Infokom.ML.Vision.Sensors
{
	/// <summary>
	/// Shares polling, observable-stream, and disposal behavior for visual sensors.
	/// Polling and observable consumption are mutually exclusive while a stream subscription is active.
	/// </summary>
	internal abstract class VisualSensorBase : IVisualSensor
	{
		private readonly object _gate = new();
		private readonly IObservable<RGB[,]> _frames;
		private CancellationTokenSource _streamCancellation;
		private bool _isStreaming;
		private bool _isDisposed;

		protected VisualSensorBase() => _frames = Observable.Create<RGB[,]>(this.StartStreaming).Publish().RefCount();

		public abstract int FrameWidth { get; }

		public abstract int FrameHeight { get; }

		public abstract double? FramesPerSecond { get; }

		/// <summary>
		/// Returns the expected interval between observable frames, or zero when the source controls its own rate.
		/// </summary>
		protected virtual TimeSpan ObservableFrameInterval =>
			FramesPerSecond is double fps && double.IsFinite(fps) && fps > 0
				? TimeSpan.FromSeconds(1.0 / fps)
				: TimeSpan.Zero;

		/// <summary>
		/// Indicates that a failed read means the source has permanently ended.
		/// </summary>
		protected virtual bool IsEndOfStream => false;

		/// <summary>
		/// Reads one frame from the source. Called under the sensor's synchronization gate.
		/// </summary>
		protected abstract bool TryReadFrame(out RGB[,] frame, int millisecondsWait);

		/// <summary>
		/// Releases resources owned by the concrete sensor. Called under the sensor's synchronization gate.
		/// </summary>
		protected abstract void DisposeSource();

		public bool TryGrabFrame(out RGB[,] frame, int millisecondsWait = 0)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(millisecondsWait);

			lock (_gate)
			{
				ObjectDisposedException.ThrowIf(_isDisposed, this);
				if (_isStreaming)
					throw new InvalidOperationException("Stop observing frames before grabbing frames directly.");

				return this.TryReadFrame(out frame, millisecondsWait);
			}
		}

		public IDisposable Subscribe(IObserver<RGB[,]> observer)
		{
			ArgumentNullException.ThrowIfNull(observer);
			return _frames.Subscribe(observer);
		}

		public void Dispose()
		{
			lock (_gate)
			{
				if (_isDisposed)
					return;

				_isDisposed = true;
				_streamCancellation?.Cancel();
				this.DisposeSource();
			}
		}

		private IDisposable StartStreaming(IObserver<RGB[,]> observer)
		{
			var cancellation = new CancellationTokenSource();
			lock (_gate)
			{
				ObjectDisposedException.ThrowIf(this._isDisposed, this);
				_isStreaming = true;
				_streamCancellation = cancellation;
			}

			var worker = Task.Run(() => this.PublishFrames(observer, cancellation.Token));
			return Disposable.Create(() =>
			{
				lock (_gate)
					cancellation.Cancel();

				if (worker.IsCompleted)
					this.CompleteStreaming(cancellation);
				else
					_ = worker.ContinueWith(_ => this.CompleteStreaming(cancellation), TaskScheduler.Default);
			});
		}

		private async Task PublishFrames(IObserver<RGB[,]> observer, CancellationToken cancellationToken)
		{
			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					var startedAt = Stopwatch.GetTimestamp();
					RGB[,] frame;
					bool hasFrame;
					bool isEndOfStream;
					lock (_gate)
					{
						if (_isDisposed || cancellationToken.IsCancellationRequested)
							return;

						hasFrame = this.TryReadFrame(out frame, 100);
						isEndOfStream = !hasFrame && this.IsEndOfStream;
					}

					if (!hasFrame)
					{
						if (isEndOfStream)
						{
							observer.OnCompleted();
							return;
						}

						await Task.Delay(1, cancellationToken).ConfigureAwait(false);
						continue;
					}

					observer.OnNext(frame);
					var remainingInterval = this.ObservableFrameInterval - Stopwatch.GetElapsedTime(startedAt);
					if (remainingInterval > TimeSpan.Zero)
						await Task.Delay(remainingInterval, cancellationToken).ConfigureAwait(false);
				}
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
			}
			catch (Exception error)
			{
				observer.OnError(error);
			}
		}

		private void CompleteStreaming(CancellationTokenSource cancellation)
		{
			lock (_gate)
			{
				if (ReferenceEquals(this._streamCancellation, cancellation))
				{
					_streamCancellation = null;
					_isStreaming = false;
				}

				cancellation.Dispose();
			}
		}
	}
}
