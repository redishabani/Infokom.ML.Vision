using System;
using System.Runtime.InteropServices;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This interface provides methods for manipulating a data buffer. Any buffer passed to a
	/// Microsoft DirectX Media Object (DMO) must implement this interface.
	/// </summary>
	[ComImport]
	[ComVisible(true)]
	[Guid("59eff8b9-938c-4a26-82f2-95cb84cdc837")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMediaBuffer
	{
		/// <summary>
		/// This method specifies the length of the data currently in the buffer.
		/// </summary>
		/// <param name="ulLength">
		/// Size of the data, in bytes. The value must not exceed the buffer's maximum size.
		/// Call the <see cref="M:Microsoft.Kinect.IMediaBuffer.GetMaxLength(System.UInt32@)" /> method to obtain the maximum size. 
		/// </param>
		void SetLength(uint ulLength);

		/// <summary>
		/// This method retrieves the maximum number of bytes this buffer can hold.
		/// </summary>
		/// <param name="pcbMaxLength">
		/// Pointer to a variable that receives the buffer's maximum size, in bytes.
		/// </param>
		void GetMaxLength(out uint pcbMaxLength);

		/// <summary>
		/// This method retrieves the buffer and the size of the valid data in the buffer.
		/// </summary>
		/// <param name="pBuffer">
		/// <see cref="T:System.IntPtr" /> that receives the native memory address of the
		/// buffer array. The <paramref name="pBuffer" /> parameter can be IntPtr.Zero.
		/// </param>
		/// <param name="cbLength">
		/// Reference to a variable that receives the size of the valid data,
		/// in bytes.
		/// </param>
		void GetBufferAndLength([Out] IntPtr pBuffer, out uint cbLength);
	}
}
