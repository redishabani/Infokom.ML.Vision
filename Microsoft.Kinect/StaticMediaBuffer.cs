using System;
using System.Runtime.InteropServices;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This class represents a pre-allocated buffer, managed by the StaticMediaBuffer client.
	/// Its main purpose is to present somebody else's buffer as a <see cref="T:Microsoft.Kinect.IMediaObject" />.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	[ComVisible(true)]
	internal class StaticMediaBuffer : IMediaBuffer
	{
		/// <summary>
		/// Native pointer to buffer data.
		/// </summary>
		private readonly IntPtr _pData;

		/// <summary>
		/// Maximum number of bytes that can be stored in buffer.
		/// </summary>
		private uint _ulSize;

		/// <summary>
		/// Number of bytes currently stored in buffer that are ready to be consumed.
		/// </summary>
		private uint _ulData;

		/// <summary>
		/// Prevents a default instance of the StaticMediaBuffer class from being created.
		/// </summary>
		private StaticMediaBuffer()
		{
		}

		/// <summary>
		/// Invalidates all buffer data, getting buffer ready to be written-to
		/// again from the beginning.
		/// </summary>
		/// <param name="size">
		/// New maximum size of buffer.
		/// </param>
		public void Reset(uint size)
		{
			_ulSize = size;
			_ulData = 0u;
		}

		/// <summary>
		/// Initializes a new instance of the StaticMediaBuffer class from the
		/// specified buffer data and size.
		/// </summary>
		/// <param name="pData">
		/// Native pointer to buffer data.
		/// </param>
		/// <param name="size">
		/// Number of bytes allocated for <paramref name="pData" /> pointer.
		/// </param>
		public StaticMediaBuffer(IntPtr pData, uint size)
		{
			_pData = pData;
			Reset(size);
		}

		/// <summary>
		/// This method specifies the length of the data currently in the buffer.
		/// </summary>
		/// <param name="ulLength">
		/// Size of the data, in bytes. The value must not exceed the buffer's maximum size.
		/// Call the <see cref="M:Microsoft.Kinect.IMediaBuffer.GetMaxLength(System.UInt32@)" /> method to obtain the maximum size. 
		/// </param>
		public void SetLength(uint ulLength)
		{
			_ulData = ulLength;
		}

		/// <summary>
		/// This method retrieves the maximum number of bytes this buffer can hold.
		/// </summary>
		/// <param name="pcbMaxLength">
		/// Pointer to a variable that receives the buffer's maximum size, in bytes.
		/// </param>
		public void GetMaxLength(out uint pcbMaxLength)
		{
			pcbMaxLength = _ulSize;
		}

		/// <summary>
		/// This method retrieves the buffer and the size of the valid data in the buffer.
		/// </summary>
		/// <param name="ppBuffer">
		/// <see cref="T:System.IntPtr" /> that receives the native memory address of the
		/// buffer array. The <paramref name="ppBuffer" /> parameter can be IntPtr.Zero.
		/// </param>
		/// <param name="cbLength">
		/// Reference to a variable that receives the size of the valid data,
		/// in bytes.
		/// </param>
		public void GetBufferAndLength(IntPtr ppBuffer, out uint cbLength)
		{
			if (ppBuffer != IntPtr.Zero)
			{
				Marshal.WriteIntPtr(ppBuffer, _pData);
			}
			cbLength = _ulData;
		}
	}
}
