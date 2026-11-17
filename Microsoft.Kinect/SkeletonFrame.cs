using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This class contains the skeleton frame returned by the Kinect API.
	/// </summary>
	public sealed class SkeletonFrame : IDisposable
	{
		/// <summary>
		/// The associated skeleton stream.
		/// </summary>
		private readonly SkeletonStream _skeletonStream;

		/// <summary>
		/// The associated skeleton data.
		/// </summary>
		private DataPool<int, Tuple<float, float, float, float>, object, Skeleton[], bool?>.Entry _skeletonData;

		/// <summary>
		/// The data access lock.
		/// </summary>
		private readonly object _dataAccessLock = new object();

		/// <summary>
		/// Tracks whether this instance is disposed.  0 == false, 1 == true.
		/// We need to use int (vs. bool) for Interlocked.Exchange.
		/// </summary>
		private int _isDisposed;

		/// <summary>
		/// Gets or sets the timestamp for the frame.
		/// </summary>
		public long Timestamp { get; set; }

		/// <summary>
		/// Gets or sets the frame number for the frame.
		/// </summary>
		public int FrameNumber { get; set; }

		/// <summary>
		/// Gets the tracking mode in which this frame was captured.
		/// </summary>
		[Obsolete("SkeletonFrame.TrackingMode property is reserved for future use.  Do not use this property; this value may change in a future release.", false)]
		public SkeletonTrackingMode TrackingMode { get; private set; }

		/// <summary>
		/// Gets or sets the floor's clip plane.
		/// The floats should be in this order from the native type: x, y, z, w.
		/// </summary>
		public Tuple<float, float, float, float> FloorClipPlane { get; set; }

		/// <summary>
		/// Gets the total length of the skeleton data buffer of this SkeletonFrame.
		/// </summary>
		public int SkeletonArrayLength => 6;

		/// <summary>
		/// Gets the depth filter that was applied to the frame.
		/// </summary>
		public IDepthFilter DepthFilter => _skeletonStream.Sensor.NuiSensor.GetDepthFilterForTimestamp(Timestamp);

		/// <summary>
		/// Initializes a new instance of the SkeletonFrame class.
		/// </summary>
		/// <param name="skeletonStream">The associated SkeletonStream object.</param>
		/// <param name="frameNumber">The current frames number.</param>
		/// <param name="timestamp">The timestamp on the frame.</param>
		/// <param name="trackingMode">The tracking mode in which the frame was captured.</param>
		/// <param name="skeletonData">The related SkeletonData.</param>
		private SkeletonFrame(SkeletonStream skeletonStream, int frameNumber, long timestamp, SkeletonTrackingMode trackingMode, DataPool<int, Tuple<float, float, float, float>, object, Skeleton[], bool?>.Entry skeletonData)
		{
			_skeletonStream = skeletonStream;
			_skeletonData = skeletonData;
			FrameNumber = frameNumber;
			Timestamp = timestamp;
			TrackingMode = trackingMode;
			FloorClipPlane = skeletonData.Value1;
			_skeletonData = skeletonData;
		}

		/// <summary>
		/// Creates a new skeleton frame object.
		/// </summary>
		/// <param name="skeletonStream">The associated SkeletonStream.</param>
		/// <param name="frameNumber">The new frame number.</param>
		/// <param name="timestamp">The frames timestamp.</param>
		/// <param name="trackingMode">The tracking mode in which the frame was captured.</param>
		/// <returns>The new SkeletonFrame object.</returns>
		internal static SkeletonFrame Create(SkeletonStream skeletonStream, int frameNumber, long timestamp, SkeletonTrackingMode trackingMode)
		{
			SkeletonFrame result = null;
			DataPool<int, Tuple<float, float, float, float>, object, Skeleton[], bool?>.Entry entry = skeletonStream.LockSkeletonData(frameNumber);
			if (entry != null)
			{
				result = new SkeletonFrame(skeletonStream, frameNumber, timestamp, trackingMode, entry);
			}
			return result;
		}

		/// <summary>
		/// This method copies the skeleton data to an array of skeleton data.
		/// </summary>
		/// <param name="skeletonData">The target array to receive the data.</param>
		public void CopySkeletonDataTo(Skeleton[] skeletonData)
		{
			if (skeletonData == null)
			{
				throw new ArgumentNullException("skeletonData");
			}
			if (skeletonData.Length != _skeletonData.Value3.Length)
			{
				throw new ArgumentException(Resources.SkeletonBufferIncorrectLength, "skeletonData");
			}
			lock (_dataAccessLock)
			{
				if (_skeletonData == null)
				{
					throw new ObjectDisposedException("SkeletonFrame");
				}
				for (int i = 0; i < _skeletonData.Value3.Length; i++)
				{
					if (skeletonData[i] == null)
					{
						skeletonData[i] = new Skeleton(_skeletonData.Value3[i]);
					}
					else
					{
						skeletonData[i].CopyFrom(_skeletonData.Value3[i]);
					}
				}
			}
		}

		/// <summary>
		/// This method disposes the skeleton frame.
		/// </summary>
		public void Dispose()
		{
			if (Interlocked.Exchange(ref _isDisposed, 1) == 0)
			{
				Dispose(disposing: true);
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Finalizes an instance of the SkeletonFrame class.
		/// This destructor is used to notify the developer if they have forgotten to
		/// dispose of the frame correctly.
		/// </summary>
		~SkeletonFrame()
		{
			Debugger.Log(0, "Performance", string.Format(CultureInfo.InvariantCulture, "{0}\n", new object[1] { Resources.SkeletonFrameNotDisposed }));
			Dispose(disposing: false);
		}

		/// <summary>
		/// To be used to return data to free lists.  If SkeletonFrame is ever unsealed, 
		/// this must change to protected virtual.
		/// </summary>
		/// <param name="disposing">Specify true to indicate that the class should dispose clean up all resources.</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (_dataAccessLock)
				{
					_skeletonStream.UnlockSkeletonData(_skeletonData);
					_skeletonData = null;
				}
			}
		}
	}
}
