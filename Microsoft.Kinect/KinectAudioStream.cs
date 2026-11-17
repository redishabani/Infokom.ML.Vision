using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Kinect
{
	/// <summary>
	/// A Stream implementation to read data from the DMO output in a .NET friendly way.
	/// </summary>
	internal class KinectAudioStream : Stream
	{
		/// <summary>
		/// Helper class that represents a data buffer used for capturing audio data
		/// and making it available for clients to read.
		/// </summary>
		internal class Buffer
		{
			/// <summary>
			/// Gets or sets number of bytes of valid data contained in buffer.
			/// </summary>
			public uint DataLength { get; set; }

			/// <summary>
			/// Gets byte array containing buffer data.
			/// </summary>
			public byte[] Data { get; private set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="T:Microsoft.Kinect.KinectAudioStream.Buffer" /> class.
			/// </summary>
			/// <param name="size">
			/// Maximum number of bytes buffer can hold.
			/// </param>
			public Buffer(uint size)
			{
				Data = new byte[size];
			}
		}

		/// <summary>
		/// Helper class to keep track of audio beam and sound source angle changes during audio capture.
		/// </summary>
		internal class AudioAngleTracker
		{
			/// <summary>
			/// Last observed value for beam angle.
			/// </summary>
			private double _lastAngle = double.MinValue;

			/// <summary>
			/// Last observed value for sound source angle.
			/// </summary>
			private double _lastSoundSourceAngle = double.MinValue;

			/// <summary>
			/// Last observed value for sound source confidence.
			/// </summary>
			private double _lastSoundSourceConfidence = double.MinValue;

			/// <summary>
			/// <see cref="T:Microsoft.Kinect.KinectAudioStream" /> that owns this <see cref="T:Microsoft.Kinect.KinectAudioSource" />.
			/// </summary>
			private readonly KinectAudioSource _audioSource;

			/// <summary>
			/// <see cref="T:Microsoft.Kinect.INuiAudioBeam" /> object used to get and set the audio beam angle.
			/// </summary>
			private readonly INuiAudioBeam _audioBeam;

			/// <summary>
			/// Initializes a new instance of the AudioAngleTracker class.
			/// </summary>
			/// <param name="audioSource">
			///  <see cref="T:Microsoft.Kinect.KinectAudioStream" /> that owns the new angle tracker.
			/// </param>
			/// <param name="audioBeam">
			/// <see cref="T:Microsoft.Kinect.INuiAudioBeam" /> object used to get and set the audio
			/// beam angle.
			/// </param>
			internal AudioAngleTracker(KinectAudioSource audioSource, INuiAudioBeam audioBeam)
			{
				_audioSource = audioSource;
				_audioBeam = audioBeam;
			}

			/// <summary>
			/// Track changes in beam angle and sound source position, triggering events for client as appropriate.
			/// </summary>
			internal void TrackAngles()
			{
				bool flag = false;
				if (_audioSource.BeamAngleMode == BeamAngleMode.Manual)
				{
					double num = _audioSource.FetchNextBeamAngle();
					if (num > double.MinValue)
					{
						_audioBeam.SetBeam(num);
						flag = true;
					}
				}
				if (_audioSource.BeamAngleMode == BeamAngleMode.Automatic || _audioSource.BeamAngleMode == BeamAngleMode.Adaptive || flag)
				{
					double angle = 0.0;
					_audioBeam.GetBeam(out angle);
					if (Math.Abs(angle - _lastAngle) > double.Epsilon)
					{
						_lastAngle = angle;
						_audioSource.SetBeamAngle(angle);
					}
				}
				_audioBeam.GetPosition(out var angle2, out var confidence);
				if (Math.Abs(angle2 - _lastSoundSourceAngle) > double.Epsilon || Math.Abs(confidence - _lastSoundSourceConfidence) > double.Epsilon)
				{
					_audioSource.SetSoundSourceAngleAndConfidence(angle2, confidence);
					_lastSoundSourceAngle = angle2;
					_lastSoundSourceConfidence = confidence;
				}
			}
		}

		/// <summary>
		/// Helper class to manage writing to the appropriate write buffer and queuing up
		/// data into the read queue when it's ready to be read by stream clients.
		/// </summary>
		internal class BufferManager
		{
			/// <summary>
			/// Managed buffer that will receive data from native layer.
			/// </summary>
			private Buffer _writeBuf;

			/// <summary>
			/// <see cref="T:Microsoft.Kinect.KinectAudioStream" /> that owns this <see cref="T:Microsoft.Kinect.KinectAudioStream.BufferManager" />.
			/// </summary>
			private readonly KinectAudioStream _stream;

			/// <summary>
			/// Initializes a new instance of the BufferManager class.
			/// </summary>
			/// <param name="stream">
			/// <see cref="T:Microsoft.Kinect.KinectAudioStream" /> that owns the new buffer manager.
			/// </param>
			internal BufferManager(KinectAudioStream stream)
			{
				_stream = stream;
			}

			/// <summary>
			/// Write the specified data into the current write buffer, queuing
			/// write buffer into read queue if it gets full.
			/// </summary>
			/// <param name="data">
			/// Data to write into write buffer.
			/// </param>
			/// <param name="length">
			/// Number of bytes to write.
			/// </param>
			internal void Write(byte[] data, uint length)
			{
				WriteHelper(length, delegate(uint destIndex)
				{
					Array.Copy(data, 0L, _writeBuf.Data, destIndex, length);
				});
			}

			/// <summary>
			/// Write empty data (zeroes) into the current write buffer, queuing
			/// write buffer into read queue if it gets full.
			/// </summary>
			/// <param name="length">
			/// Number of bytes to write.
			/// </param>
			internal void WriteEmpty(uint length)
			{
				WriteHelper(length, delegate(uint destIndex)
				{
					Array.Clear(_writeBuf.Data, (int)destIndex, (int)length);
				});
			}

			/// <summary>
			/// Performs the specified write action, queuing write buffers into
			/// read queue as they get full.
			/// </summary>
			/// <param name="length">
			/// Number of bytes to write into current write buffer.
			/// </param>
			/// <param name="writeAction">
			/// Action to perform on current write buffer.
			/// </param>
			private void WriteHelper(uint length, Action<uint> writeAction)
			{
				if (length != 0)
				{
					if (_writeBuf == null)
					{
						_writeBuf = _stream.GetWriteBuffer();
					}
					if (_writeBuf.DataLength + length < _writeBuf.Data.Length)
					{
						writeAction(_writeBuf.DataLength);
						_writeBuf.DataLength += length;
						return;
					}
					_stream.QueueCapturedBuffer(_writeBuf);
					_writeBuf = _stream.GetWriteBuffer();
					writeAction(0u);
					_writeBuf.DataLength = length;
				}
			}

			/// <summary>
			/// Queues any data lingering in current write buffer into the read buffer
			/// queue, ready for client to read it.
			/// </summary>
			internal void Flush()
			{
				if (_writeBuf != null)
				{
					_stream.QueueCapturedBuffer(_writeBuf);
					_writeBuf = null;
				}
			}
		}

		/// <summary>
		/// Status flag returned by IMediaObject::ProcessOutput.
		/// </summary>
		internal const uint DMO_OUTPUT_DATA_BUFFERF_INCOMPLETE = 16777216u;

		/// <summary>
		/// WASAPI error indicating that the specified capture device has become invalid.
		/// </summary>
		private const uint AUDCLNT_E_DEVICE_INVALIDATED = 2290679812u;

		/// <summary>
		/// Number of milliseconds' worth of data to allocate to each buffer in buffer pool.
		/// </summary>
		internal const uint BufferSizeMilliSeconds = 50u;

		/// <summary>
		/// Number of bytes to allocate to each buffer in buffer pool.
		/// </summary>
		internal const uint BufferSizeBytes = 1600u;

		/// <summary>
		/// Minimum number of milliseconds' worth of captured audio to keep in buffer pool.
		/// </summary>
		private const uint MinTotalBufferMilliseconds = 1000u;

		/// <summary>
		/// Maximum number of milliseconds' worth of captured audio to keep in buffer pool.
		/// </summary>
		private const uint MaxTotalBufferMilliseconds = 4000u;

		/// <summary>
		/// Number of milliseconds' worth of additional data to be discarded when we hit the
		/// threshold for stale read data.
		/// </summary>
		/// <remarks>
		/// The DMO buffers 0.1 seconds of mic data on the input, but some of it may already
		/// be making its way down the pipeline. Experimentally 250ms works best.
		/// </remarks>
		private const int AdditionalStaleDataToDiscard = 250;

		/// <summary>
		/// Event used to let capture thread know when it should stop capturing audio data.
		/// </summary>
		private readonly ManualResetEvent _stopEvent;

		/// <summary>
		/// <see cref="T:Microsoft.Kinect.KinectAudioSource" /> that owns this stream.
		/// </summary>
		private readonly KinectAudioSource _audioSource;

		/// <summary>
		/// Pool of data buffers waiting to be used for audio capture.
		/// </summary>
		private readonly Stack<Buffer> _writeBufferStack = new Stack<Buffer>();

		/// <summary>
		/// Queue of buffers with data pending to be read by stream client.
		/// </summary>
		private readonly Queue<Buffer> _readBufferQueue = new Queue<Buffer>();

		/// <summary>
		/// Object used to synchronize multithreaded access to data capture/read buffers.
		/// </summary>
		private readonly object _syncRoot = new object();

		/// <summary>
		/// Event used to wake up threads waiting to read from this stream.
		/// </summary>
		private readonly AutoResetEvent _dataReady = new AutoResetEvent(initialState: false);

		/// <summary>
		/// <code>true</code> if we've ever started to capture audio data through
		/// this stream instance.
		/// <code>false</code> otherwise.
		/// </summary>
		private bool _started;

		/// <summary>
		/// Audio capture thread.
		/// </summary>
		private Thread _capturingThread;

		/// <summary>
		/// Current audio data buffer from which client will read data.
		/// </summary>
		private Buffer _currentBuffer;

		/// <summary>
		/// Next data index within current audio data buffer from which
		/// client will read data.
		/// </summary>
		private int _currentBufferIndex;

		/// <summary>
		/// Number of bytes that have been read from this stream.
		/// </summary>
		private long _bytesRead;

		/// <summary>
		/// <code>true</code> if we're actively capturing audio data.
		/// <code>false</code> otherwise.
		/// </summary>
		private bool _capturing;

		/// <summary>
		/// Timestamp of last time that client stopped reading data.
		/// </summary>
		/// <remarks>
		/// Used in conjunction with _readStaleThreshold to
		/// detect if the reader stopped reading data. If so, we want to discard old data on
		/// the next read.
		/// </remarks>
		private DateTime _lastRead = DateTime.MinValue;

		/// <summary>
		/// If nothing is read for this amount of time, we assume that the client has
		/// purposefully stopped reading from stream, so we start with fresh data next time
		/// they request to read data.
		/// </summary>
		private readonly TimeSpan _readStaleThreshold;

		/// <summary>
		/// Exception, if any, encountered during capture thread execution.
		/// </summary>
		private Exception _captureException;

		/// <summary>
		/// Gets the read buffer queue.
		/// </summary>
		internal Queue<Buffer> ReadBufferQueue => _readBufferQueue;

		/// <summary>
		/// Gets a value indicating whether the current stream supports reading.
		/// </summary>
		public override bool CanRead => _started;

		/// <summary>
		/// Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		public override bool CanSeek => false;

		/// <summary>
		/// Gets a value indicating whether the current stream supports writing.
		/// </summary>
		public override bool CanWrite => false;

		/// <summary>
		/// Gets the length in bytes of the stream.
		/// </summary>
		/// <remarks>
		/// Always returns -1 in this implementation.
		/// </remarks>
		public override long Length => -1L;

		/// <summary>
		/// Gets or sets the position within the current stream.
		/// </summary>
		/// <remarks>
		/// Set is unsupported in this implementation.
		/// </remarks>
		public override long Position
		{
			get
			{
				return _bytesRead;
			}
			set
			{
				throw new InvalidOperationException(Resources.NotSupported);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Microsoft.Kinect.KinectAudioStream" /> class.
		/// </summary>
		/// <param name="audioSource">
		/// <see cref="T:Microsoft.Kinect.KinectAudioSource" /> that owns the stream being created
		/// </param>
		/// <param name="readStaleThreshold">
		/// If there are no reads to the stream for longer than this threshold the DMO discards
		/// any buffered audio. This prevents stale data from being returned in scenarios such as speech recognition and dialog
		/// systems, when the consumption of audio samples may stop for a while. Pass TimeSpan.MaxValue to avoid hitting this threshold.
		/// </param>
		internal KinectAudioStream(KinectAudioSource audioSource, TimeSpan readStaleThreshold)
		{
			_audioSource = audioSource;
			_readStaleThreshold = readStaleThreshold;
			_stopEvent = audioSource.NativeWrapper.CreateStreamStopEvent();
		}

		/// <summary>
		/// Stops capture thread and truncates pending stream reads.
		/// </summary>
		internal void Stop()
		{
			if (!_started)
			{
				throw new InvalidOperationException(Resources.CaptureNotStarted);
			}
			_stopEvent.Set();
			_capturingThread.Join();
		}

		/// <summary>
		/// Starts capture thread and prepares stream for reading.
		/// </summary>
		public void Start()
		{
			if (_started)
			{
				throw new InvalidOperationException(Resources.CaptureAlreadyStarted);
			}
			ResetStaleTracking();
			_capturingThread = new Thread(RunCapture);
			_capturingThread.IsBackground = true;
			_capturingThread.SetApartmentState(ApartmentState.MTA);
			_capturingThread.Name = "AudioCaptureThread";
			_capturingThread.Start(null);
			_capturing = true;
			_started = true;
		}

		/// <summary>
		/// Resets tracking of stale data as if data had just been read, so that no data
		/// will be discarded until {now + readStaleThreshold}.
		/// </summary>
		internal void ResetStaleTracking()
		{
			_lastRead = DateTime.UtcNow;
		}

		/// <summary>
		/// Configure media type in media object and allocate streaming resources.
		/// </summary>
		/// <param name="mediaObject">
		/// <see cref="T:Microsoft.Kinect.IMediaObject" /> to initialize.
		/// </param>
		private static void InitializeMediaObject(IMediaObject mediaObject)
		{
			WAVEFORMATEX format = new WAVEFORMATEX
			{
				wFormatTag = 1,
				nChannels = 1,
				nSamplesPerSec = 16000u,
				nAvgBytesPerSec = 32000u,
				nBlockAlign = 2,
				wBitsPerSample = 16,
				cbSize = 0
			};
			using (DMO_MEDIA_TYPE dMO_MEDIA_TYPE = new DMO_MEDIA_TYPE())
			{
				dMO_MEDIA_TYPE.majortype = DMO_MEDIA_TYPE.MEDIATYPE_Audio;
				dMO_MEDIA_TYPE.subtype = DMO_MEDIA_TYPE.MEDIASUBTYPE_PCM;
				dMO_MEDIA_TYPE.lSampleSize = 0;
				dMO_MEDIA_TYPE.bFixedSizeSamples = true;
				dMO_MEDIA_TYPE.bTemporalCompression = false;
				dMO_MEDIA_TYPE.formattype = DMO_MEDIA_TYPE.FORMAT_WaveFormatEx;
				dMO_MEDIA_TYPE.SetFormat(format);
				dMO_MEDIA_TYPE.cbFormat = Marshal.SizeOf(typeof(WAVEFORMATEX));
				mediaObject.SetOutputType(0, dMO_MEDIA_TYPE, 0);
			}
			mediaObject.AllocateStreamingResources();
		}

		/// <summary>
		/// Get number of buffers appropriate to use in a write buffer pool that
		/// keeps track of the specified number of milliseconds' worth of buffer.
		/// </summary>
		/// <param name="readStaleThresholdMilliseconds">
		/// Number of milliseconds in readStaleThreshold. See
		/// <see cref="M:Microsoft.Kinect.KinectAudioSource.#ctor(Microsoft.Kinect.KinectSensor)" />.
		/// </param>
		/// <returns>
		/// Number of buffers in write buffer pool.
		/// </returns>
		internal static uint GetNumBuffersInPool(uint readStaleThresholdMilliseconds)
		{
			uint num = Math.Min(4000u, Math.Max(1000u, readStaleThresholdMilliseconds));
			return (uint)Math.Ceiling((double)num / 50.0);
		}

		/// <summary>
		/// Thread procedure for audio capture thread.
		/// </summary>
		/// <param name="notused">
		/// UNUSED - Thread Procedure default parameter.
		/// </param>
		/// <remarks>
		/// This thread creates a DMO and calls it in a loop to get captured audio.
		/// Properties set on KinectAudioSource are read during the capture loop and applied
		/// to DMO.
		/// Thread handles audio device invalidation errors and generates a stream of silent
		/// audio until device becomes valid again.
		/// </remarks>
		private void RunCapture(object notused)
		{
			byte[] array = new byte[1600];
			object obj = _audioSource.NativeWrapper.CreateDmoInstance();
			IMediaObject mediaObject;
			IPropertyStore propertyStore;
			if (obj is NuiAudioBeam nuiAudioBeam)
			{
				mediaObject = (IMediaObject)nuiAudioBeam.Wrapped;
				propertyStore = (IPropertyStore)nuiAudioBeam.Wrapped;
			}
			else
			{
				mediaObject = (IMediaObject)obj;
				propertyStore = (IPropertyStore)obj;
			}
			int taskIndex = 0;
			int num = NativeMethods.AvSetMmThreadCharacteristics("Audio", ref taskIndex);
			uint numBuffersInPool = GetNumBuffersInPool((uint)_readStaleThreshold.TotalMilliseconds);
			for (int i = 0; i < numBuffersInPool; i++)
			{
				_writeBufferStack.Push(new Buffer(1600u));
			}
			GCHandle handle = default(GCHandle);
			GCHandle handle2 = default(GCHandle);
			IntPtr intPtr = IntPtr.Zero;
			bool flag = false;
			ushort? num2 = null;
			try
			{
				handle = GCHandle.Alloc(array, GCHandleType.Pinned);
				IntPtr pData = handle.AddrOfPinnedObject();
				StaticMediaBuffer staticMediaBuffer = new StaticMediaBuffer(pData, (uint)array.Length);
				intPtr = Marshal.GetComInterfaceForObject(staticMediaBuffer, typeof(IMediaBuffer));
				DMO_OUTPUT_DATA_BUFFER dMO_OUTPUT_DATA_BUFFER = new DMO_OUTPUT_DATA_BUFFER
				{
					Buffer = intPtr,
					Status = 0u
				};
				handle2 = GCHandle.Alloc(dMO_OUTPUT_DATA_BUFFER, GCHandleType.Pinned);
				DMO_OUTPUT_DATA_BUFFER[] pOutputBuffers = new DMO_OUTPUT_DATA_BUFFER[1] { dMO_OUTPUT_DATA_BUFFER };
				AudioAngleTracker audioAngleTracker = new AudioAngleTracker(_audioSource, obj as INuiAudioBeam);
				BufferManager bufferManager = new BufferManager(this);
				bool flag2 = false;
				while (!flag2)
				{
					do
					{
						dMO_OUTPUT_DATA_BUFFER.Status = 0u;
						staticMediaBuffer.Reset((uint)array.Length);
						while (!_stopEvent.WaitOne(0, exitContext: false) && !((int?)num2).HasValue)
						{
							num2 = _audioSource.FindMicrophoneIndex();
							if (((int?)num2).HasValue)
							{
								_audioSource.MicrophoneIndex = num2.Value;
								continue;
							}
							bufferManager.WriteEmpty(1600u);
							Thread.Sleep(50);
						}
						if (_stopEvent.WaitOne(10, exitContext: false))
						{
							flag2 = true;
							break;
						}
						_audioSource.ApplyPendingProperties(propertyStore);
						try
						{
							if (!flag)
							{
								InitializeMediaObject(mediaObject);
								flag = true;
							}
							mediaObject.ProcessOutput(0, 1, pOutputBuffers, out var _);
						}
						catch (COMException ex)
						{
							if (ex.ErrorCode == -2004287484)
							{
								num2 = null;
								continue;
							}
							throw;
						}
						catch (ArgumentException)
						{
							int count = 0;
							_audioSource.NativeWrapper.GetSpeakerDevices(null, ref count);
							if (_audioSource.EchoCancellationSpeakerIndex >= count)
							{
								IPropertyStore propertyStore2 = propertyStore;
								PROPERTYKEY key = KinectAudioSource.PropertyKeyFromId(2);
								object pv = 2;
								propertyStore2.SetValue(ref key, ref pv);
								IPropertyStore propertyStore3 = propertyStore;
								PROPERTYKEY key2 = KinectAudioSource.PropertyKeyFromId(10);
								object pv2 = 0;
								propertyStore3.SetValue(ref key2, ref pv2);
								continue;
							}
							throw;
						}
						staticMediaBuffer.GetBufferAndLength(IntPtr.Zero, out var cbLength);
						bufferManager.Write(array, cbLength);
						audioAngleTracker.TrackAngles();
					}
					while ((dMO_OUTPUT_DATA_BUFFER.Status & 0x1000000) != 0);
				}
				bufferManager.Flush();
			}
			catch (COMException captureException)
			{
				_captureException = captureException;
			}
			catch (UnauthorizedAccessException captureException2)
			{
				_captureException = captureException2;
			}
			catch (Exception)
			{
				_captureException = null;
			}
			finally
			{
				_capturing = false;
				_dataReady.Set();
				SafeFree(handle2);
				SafeFree(handle);
				if (intPtr != IntPtr.Zero)
				{
					Marshal.Release(intPtr);
				}
				_audioSource.NativeWrapper.DestroyDmoInstance(obj);
				if (num != 0)
				{
					NativeMethods.AvRevertMmThreadCharacteristics(num);
				}
			}
		}

		/// <summary>
		/// Enqueues the captured stream buffer into the data queue from which client will read stream.
		/// </summary>
		/// <param name="capturedBuffer">
		/// Buffer that was produced by audio capture operation.
		/// </param>
		private void QueueCapturedBuffer(Buffer capturedBuffer)
		{
			lock (_syncRoot)
			{
				_readBufferQueue.Enqueue(capturedBuffer);
				_dataReady.Set();
			}
		}

		/// <summary>
		/// Gets a buffer from the write buffer pool, if possible. Otherwise it
		/// gets the oldest buffer from the read buffer queue.
		/// </summary>
		/// <returns>
		/// A buffer ready to be written-to.
		/// </returns>
		private Buffer GetWriteBuffer()
		{
			lock (_syncRoot)
			{
				if (_writeBufferStack.Count > 0)
				{
					return _writeBufferStack.Pop();
				}
				if (_readBufferQueue.Count > 0)
				{
					Buffer buffer = _readBufferQueue.Dequeue();
					buffer.DataLength = 0u;
					return buffer;
				}
				return null;
			}
		}

		/// <summary>
		/// Frees a <see cref="T:System.Runtime.InteropServices.GCHandle" /> if it has been initialized.
		/// </summary>
		/// <param name="handle">
		/// Handle to free.
		/// </param>
		private static void SafeFree(GCHandle handle)
		{
			if (handle != default(GCHandle))
			{
				handle.Free();
			}
		}

		/// <summary>
		/// When overridden in a derived class, clears all buffers for this stream
		/// and causes any buffered data to be written to the underlying device.
		/// </summary>
		/// <remarks>
		/// Unsupported in this implementation.
		/// </remarks>
		public override void Flush()
		{
			throw new InvalidOperationException(Resources.NotSupported);
		}

		/// <summary>
		/// Reads a sequence of bytes from the current stream and advances the position
		/// within the stream by the number of bytes read.
		/// </summary>
		/// <param name="buffer">
		/// An array of bytes. When this method returns, the buffer contains the specified
		/// byte array with the values between <paramref name="offset" /> and
		/// (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by
		/// the bytes read from the current source.
		/// </param>
		/// <param name="offset">
		/// The zero-based byte offset in <paramref name="buffer" /> at which to begin
		/// storing the data read from the current stream. 
		/// </param>
		/// <param name="count">
		/// The maximum number of bytes to be read from the current stream. 
		/// </param>
		/// <returns>
		/// The total number of bytes read into the buffer. This can be less than the number
		/// of bytes requested if that many bytes are not currently available, or zero (0)
		/// if the end of the stream has been reached.
		/// </returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_captureException != null)
			{
				throw _captureException;
			}
			if (DateTime.UtcNow - _lastRead > _readStaleThreshold)
			{
				Thread.Sleep(250);
				ReleaseAllBuffers();
			}
			ResetStaleTracking();
			int num = 0;
			while (count > 0 && _capturing)
			{
				lock (_syncRoot)
				{
					if (_currentBuffer == null && _readBufferQueue.Count != 0)
					{
						_currentBuffer = _readBufferQueue.Dequeue();
						_currentBufferIndex = 0;
					}
					if (_currentBuffer != null)
					{
						int num2 = (int)Math.Min(_currentBuffer.DataLength - _currentBufferIndex, count);
						Array.Copy(_currentBuffer.Data, _currentBufferIndex, buffer, offset, num2);
						count -= num2;
						offset += num2;
						num += num2;
						_currentBufferIndex += num2;
						if (_currentBufferIndex >= _currentBuffer.DataLength)
						{
							ReleaseBuffer(_currentBuffer);
							_currentBuffer = ((_readBufferQueue.Count != 0) ? _readBufferQueue.Dequeue() : null);
							_currentBufferIndex = 0;
						}
					}
				}
				if (_currentBuffer == null && count > 0)
				{
					_dataReady.WaitOne(-1, exitContext: false);
				}
			}
			_bytesRead += num;
			return num;
		}

		/// <summary>
		/// Release an individual read buffer, returning it to the write buffer pool.
		/// </summary>
		/// <param name="buffer">
		/// Buffer to release.
		/// </param>
		private void ReleaseBuffer(Buffer buffer)
		{
			buffer.DataLength = 0u;
			_writeBufferStack.Push(buffer);
		}

		/// <summary>
		/// Release all read buffers, returning them to the write buffer pool.
		/// </summary>
		private void ReleaseAllBuffers()
		{
			lock (_syncRoot)
			{
				while (_readBufferQueue.Count > 0)
				{
					ReleaseBuffer(_readBufferQueue.Dequeue());
				}
				if (_currentBuffer != null)
				{
					ReleaseBuffer(_currentBuffer);
				}
				_currentBufferIndex = 0;
				_currentBuffer = null;
			}
		}

		/// <summary>
		/// Sets the position within the current stream.
		/// This implementation represents a live stream, so this method just returns
		/// the byte position relative to SeekOrigin.Current, without adjusting the
		/// actual stream position.
		/// </summary>
		/// <param name="offset">
		/// Byte offset relative to SeekOrigin.Current.
		/// </param>
		/// <param name="origin">
		/// A value of type SeekOrigin indicating the reference point used to obtain the new
		/// position.
		/// Parameter value is ignored in this implementation.
		/// </param>
		/// <returns>
		/// The new position within the current stream.
		/// </returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			return _bytesRead + offset;
		}

		/// <summary>
		/// When overridden in a derived class, sets the length of the current stream.
		/// Not supported in this implementation.
		/// </summary>
		/// <param name="value">
		/// The desired length of the current stream in bytes.
		/// </param>
		public override void SetLength(long value)
		{
			throw new InvalidOperationException(Resources.NotSupported);
		}

		/// <summary>
		/// When overridden in a derived class, writes a sequence of bytes to the current stream
		/// and advances the current position within this stream by the number of bytes written.
		/// Not supported in this implementation.
		/// </summary>
		/// <param name="buffer">
		/// An array of bytes. This method copies <paramref name="count" /> bytes from
		/// <paramref name="buffer" /> to the current stream.
		/// </param>
		/// <param name="offset">
		/// The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes
		/// to the current stream.
		/// </param>
		/// <param name="count">
		/// The number of bytes to be written to the current stream.
		/// </param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new InvalidOperationException(Resources.NotSupported);
		}

		/// <summary>
		/// Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream.
		/// </summary>
		public override void Close()
		{
			base.Close();
			Stop();
		}
	}
}
