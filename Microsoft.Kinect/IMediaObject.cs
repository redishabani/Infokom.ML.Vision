using System.Runtime.InteropServices;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This interface provides methods for manipulating a Microsoft DirectX Media
	/// Object (DMO).
	/// </summary>
	[ComImport]
	[ComVisible(true)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("d8ad0f58-5494-4102-97c5-ec798e59bcf4")]
	internal interface IMediaObject
	{
		/// <summary>
		/// This method retrieves the number of input and output streams.
		/// </summary>
		/// <param name="pcInputStreams">
		/// Receives the number of input streams.
		/// </param>
		/// <param name="pcOutputStreams">
		/// Receives the number of output streams.
		/// </param>
		void GetStreamCount(out int pcInputStreams, out int pcOutputStreams);

		/// <summary>
		/// This method retrieves information about an input stream, such as any
		/// restrictions on the number of samples per buffer, and whether the
		/// stream performs a lookahead on the input data. This information never
		/// changes.
		/// </summary>
		/// <param name="dwInputStreamIndex">
		/// Zero-based index of an input stream on the DMO. 
		/// </param>
		/// <param name="pdwFlags">
		/// Receives a bitwise combination of zero or more
		/// DMO_INPUT_STREAM_INFO_FLAGS flags. 
		/// </param>
		void GetInputStreamInfo(int dwInputStreamIndex, out int pdwFlags);

		/// <summary>
		/// This method retrieves information about an output stream; for example,
		/// whether the stream is discardable and whether it uses a fixed sample
		/// size. This information never changes.
		/// </summary>
		/// <param name="dwOutputStreamIndex">
		/// Zero-based index of an output stream on the DMO.
		/// </param>
		/// <param name="pdwFlags">
		/// Receives a bitwise combination of zero or more
		/// DMO_OUTPUT_STREAM_INFO_FLAGS flags. 
		/// </param>
		void GetOutputStreamInfo(int dwOutputStreamIndex, out int pdwFlags);

		/// <summary>
		/// This method retrieves a preferred media type for a specified input
		/// stream.
		/// </summary>
		/// <param name="dwInputStreamIndex">
		/// Zero-based index of an input stream on the DMO. 
		/// </param>
		/// <param name="dwTypeIndex">
		/// Zero-based index on the set of acceptable media types. 
		/// </param>
		/// <param name="pmt">
		/// Reference to a DMO_MEDIA_TYPE structure. The method fills the structure
		/// with the media type information. The format block might be null, in
		/// which case the format type GUID is GUID_NULL.
		/// </param>
		void GetInputType(int dwInputStreamIndex, int dwTypeIndex, out DMO_MEDIA_TYPE pmt);

		/// <summary>
		/// This method retrieves a preferred media type for a specified output
		/// stream.
		/// </summary>
		/// <param name="dwOutputStreamIndex">
		/// Zero-based index of an output stream on the DMO. 
		/// </param>
		/// <param name="dwTypeIndex">
		/// Zero-based index on the set of acceptable media types. 
		/// </param>
		/// <param name="pmt">
		/// Reference to a DMO_MEDIA_TYPE structure. The method fills the structure
		/// with the media type information. The format block might be null, in
		/// which case the format type GUID is GUID_NULL.
		/// </param>
		void GetOutputType(int dwOutputStreamIndex, int dwTypeIndex, out DMO_MEDIA_TYPE pmt);

		/// <summary>
		/// This method sets the media type on an input stream or tests whether a
		/// media type is acceptable.
		/// </summary>
		/// <param name="dwInputStreamIndex">
		/// Zero-based index of an input stream on the DMO. 
		/// </param>
		/// <param name="pmt">
		/// DMO_MEDIA_TYPE structure that specifies the media type.
		/// </param>
		/// <param name="dwFlags">
		/// Bitwise combination of zero or more flags from the DMO_SET_TYPE_FLAGS
		/// enumeration. 
		/// </param>
		void SetInputType(int dwInputStreamIndex, DMO_MEDIA_TYPE pmt, int dwFlags);

		/// <summary>
		/// This method sets the media type on an output stream or tests whether a
		/// media type is acceptable.
		/// </summary>
		/// <param name="dwOutputStreamIndex">
		/// Zero-based index of an output stream on the DMO. 
		/// </param>
		/// <param name="pmt">
		/// DMO_MEDIA_TYPE structure that specifies the media type.
		/// </param>
		/// <param name="dwFlags">
		/// Bitwise combination of zero or more flags from the DMO_SET_TYPE_FLAGS
		/// enumeration. 
		/// </param>
		void SetOutputType(int dwOutputStreamIndex, DMO_MEDIA_TYPE pmt, int dwFlags);

		/// <summary>
		/// This method retrieves the media type that was set for an input stream,
		/// if any.
		/// </summary>
		/// <param name="dwInputStreamIndex">
		/// Zero-based index of an input stream on the DMO. 
		/// </param>
		/// <param name="pmt">
		/// Reference to a DMO_MEDIA_TYPE structure. The method fills the structure
		/// with the media type information.
		/// </param>
		void GetInputCurrentType(int dwInputStreamIndex, out DMO_MEDIA_TYPE pmt);

		/// <summary>
		/// This method retrieves the media type that was set for an output stream,
		/// if any.
		/// </summary>
		/// <param name="dwOutputStreamIndex">
		/// Zero-based index of an output stream on the DMO. 
		/// </param>
		/// <param name="pmt">
		/// Reference to a DMO_MEDIA_TYPE structure. The method fills the structure
		/// with the media type information.
		/// </param>
		void GetOutputCurrentType(int dwOutputStreamIndex, out DMO_MEDIA_TYPE pmt);

		/// <summary>
		/// This method retrieves the buffer requirements for a specified input
		/// stream.
		/// </summary>
		/// <param name="dwInputStreamIndex">
		/// Zero-based index of an input stream on the DMO. 
		/// </param>
		/// <param name="pcbSize">
		/// Receives the minimum size of an input buffer for this stream, in bytes.
		/// </param>
		/// <param name="pcbMaxLookahead">
		/// Receives the maximum amount of data that the DMO will hold for a
		/// lookahead, in bytes. If the DMO does not perform a lookahead on the
		/// stream, the value is zero.
		/// </param>
		/// <param name="pcbAlignment">
		/// Receives the required buffer alignment, in bytes. If the input stream
		/// has no alignment requirement, the value is 1.
		/// </param>
		void GetInputSizeInfo(int dwInputStreamIndex, out int pcbSize, out int pcbMaxLookahead, out int pcbAlignment);

		/// <summary>
		/// This method retrieves the buffer requirements for a specified output
		/// stream.
		/// </summary>
		/// <param name="dwOutputStreamIndex">
		/// Zero-based index of an output stream on the DMO. 
		/// </param>
		/// <param name="pcbSize">
		/// Receives the minimum size of an output buffer for this stream, in bytes.
		/// </param>
		/// <param name="pcbAlignment">
		/// Receives the required buffer alignment, in bytes. If the output stream
		/// has no alignment requirement, the value is 1.
		/// </param>
		void GetOutputSizeInfo(int dwOutputStreamIndex, out int pcbSize, out int pcbAlignment);

		/// <summary>
		/// This method retrieves the maximum latency on a specified input stream.
		/// </summary>
		/// <param name="dwInputStreamIndex">
		/// Zero-based index of an input stream on the DMO.
		/// </param>
		/// <param name="prtMaxLatency">
		/// Receives the maximum latency.
		/// </param>
		void GetInputMaxLatency(int dwInputStreamIndex, out long prtMaxLatency);

		/// <summary>
		/// This method sets the maximum latency on a specified input stream. For
		/// the definition of maximum latency, see
		/// <see cref="M:Microsoft.Kinect.IMediaObject.GetInputMaxLatency(System.Int32,System.Int64@)" />.
		/// </summary>
		/// <param name="dwInputStreamIndex">
		/// Zero-based index of an input stream on the DMO.
		/// </param>
		/// <param name="rtMaxLatency">
		/// Maximum latency.
		/// </param>
		void SetInputMaxLatency(int dwInputStreamIndex, long rtMaxLatency);

		/// <summary>
		/// This method flushes all internally buffered data.
		/// </summary>
		void Flush();

		/// <summary>
		/// This method signals a discontinuity on the specified input stream.
		/// </summary>
		/// <param name="dwInputStreamIndex">
		/// Zero-based index of an input stream on the DMO.
		/// </param>
		void Discontinuity(int dwInputStreamIndex);

		/// <summary>
		/// This method allocates any resources needed by the DMO. Calling this
		/// method is always optional.
		/// </summary>
		void AllocateStreamingResources();

		/// <summary>
		/// This method frees resources allocated by the DMO. Calling this method
		/// is always optional.
		/// </summary>
		void FreeStreamingResources();

		/// <summary>
		/// This method queries whether an input stream can accept more input data.
		/// </summary>
		/// <param name="dwInputStreamIndex">
		/// Zero-based index of an input stream on the DMO. 
		/// </param>
		/// <param name="dwFlags">
		/// Receives either zero or DMO_INPUT_STATUSF_ACCEPT_DATA.
		/// </param>
		void GetInputStatus(int dwInputStreamIndex, out int dwFlags);

		/// <summary>
		/// This method delivers a buffer to the specified input stream.
		/// </summary>
		/// <param name="dwInputStreamIndex">
		/// Zero-based index of an input stream on the DMO. 
		/// </param>
		/// <param name="pBuffer">
		/// <see cref="T:Microsoft.Kinect.IMediaBuffer" /> interface for buffer.
		/// </param>
		/// <param name="dwFlags">
		/// Bitwise combination of zero or more flags from the
		/// DMO_INPUT_DATA_BUFFER_FLAGS enumeration.
		/// </param>
		/// <param name="rtTimestamp">
		/// Time stamp that specifies the start time of the data in the buffer. If
		///  the buffer has a valid time stamp, set the DMO_INPUT_DATA_BUFFERF_TIME
		/// flag in the dwFlags parameter. Otherwise, the DMO ignores this value. 
		/// </param>
		/// <param name="rtTimelength">
		/// Reference time specifying the duration of the data in the buffer. If
		/// this value is valid, set the DMO_INPUT_DATA_BUFFERF_TIMELENGTH flag in
		/// the dwFlags parameter. Otherwise, the DMO ignores this value.
		/// </param>
		void ProcessInput(int dwInputStreamIndex, IMediaBuffer pBuffer, int dwFlags, long rtTimestamp, long rtTimelength);

		/// <summary>
		/// This method generates output from the current input data.
		/// </summary>
		/// <param name="dwFlags">
		/// Bitwise combination of zero or more flags from the
		/// DMO_PROCESS_OUTPUT_FLAGS enumeration. 
		/// </param>
		/// <param name="cOutputBufferCount">
		/// Number of output buffers. 
		/// </param>
		/// <param name="pOutputBuffers">
		/// Array of DMO_OUTPUT_DATA_BUFFER structures containing the output buffers.
		/// Specify the size of the array in the cOutputBufferCount parameter.
		/// </param>
		/// <param name="pdwStatus">
		/// Receives a reserved value (zero). The application should ignore this
		/// value.
		/// </param>
		void ProcessOutput(int dwFlags, int cOutputBufferCount, [MarshalAs(UnmanagedType.LPArray)] DMO_OUTPUT_DATA_BUFFER[] pOutputBuffers, out int pdwStatus);

		/// <summary>
		/// This method acquires or releases a lock on the DMO. Call this method
		/// to keep the DMO serialized when performing multiple operations.
		/// </summary>
		/// <param name="bLock">
		/// Value that specifies whether to acquire or release the lock. If the
		/// value is nonzero, a lock is acquired. If the value is zero, the lock
		/// is released. 
		/// </param>
		void Lock(long bLock);
	}
}
