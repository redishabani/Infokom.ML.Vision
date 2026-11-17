using System;
using System.Runtime.InteropServices;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This structure describes an output buffer used by a DirectX DMO.
	/// </summary>
	[ComVisible(true)]
	internal struct DMO_OUTPUT_DATA_BUFFER
	{
		/// <summary>
		/// Value of pinned native pointer to the <see cref="T:Microsoft.Kinect.IMediaBuffer" />
		/// interface of a buffer allocated by the application.
		/// </summary>
		public IntPtr Buffer;

		/// <summary>
		/// Status flags. After processing output, the DMO sets this member to a
		/// bitwise combination of zero or more DMO_OUTPUT_DATA_BUFFER_FLAGS flags.
		/// </summary>
		public uint Status;

		/// <summary>
		/// Time stamp that specifies the start time of the data in the buffer. If
		/// the buffer has a valid time stamp, the DMO sets this member and also
		/// sets the DMO_OUTPUT_DATA_BUFFERF_TIME flag in the dwStatus member.
		/// Otherwise, ignore this member.
		/// </summary>
		public long Timestamp;

		/// <summary>
		/// Reference time specifying the length of the data in the buffer. If the
		/// DMO sets this member to a valid value, it also sets the
		/// DMO_OUTPUT_DATA_BUFFERF_TIMELENGTH flag in the dwStatus member.
		/// Otherwise, ignore this member. 
		/// </summary>
		public long Timelength;
	}
}
