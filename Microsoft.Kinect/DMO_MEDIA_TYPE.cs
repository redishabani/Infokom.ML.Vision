using System;
using System.Runtime.InteropServices;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This structure describes the format of the data used by a stream in a DirectX DMO.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	[ComVisible(true)]
	internal sealed class DMO_MEDIA_TYPE : IDisposable
	{
		/// <summary>
		/// Media major type GUID for audio types.
		/// </summary>
		public static readonly Guid MEDIATYPE_Audio = new Guid("73647561-0000-0010-8000-00AA00389B71");

		/// <summary>
		/// Media subtype GUID for PCM audio signals.
		/// </summary>
		public static readonly Guid MEDIASUBTYPE_PCM = new Guid("00000001-0000-0010-8000-00AA00389B71");

		/// <summary>
		/// Media subtype GUID for wave audio signals.
		/// </summary>
		public static readonly Guid MEDIASUBTYPE_WAVE = new Guid("e436eb8b-524f-11ce-9f53-0020af0ba770");

		/// <summary>
		/// Media format type GUID for WAVEFORMATEX formats.
		/// </summary>
		public static readonly Guid FORMAT_WaveFormatEx = new Guid("05589f81-c356-11ce-bf01-00aa0055595a");

		/// <summary>
		/// A GUID identifying the stream's major media type.
		/// This must be one of the DMO Media Types.
		/// </summary>
		public Guid majortype;

		/// <summary>
		/// Subtype GUID of the stream. 
		/// </summary>
		public Guid subtype;

		/// <summary>
		/// If TRUE, samples are of a fixed size. This field is informational only.
		/// For audio, it is generally set to TRUE. For video, it is usually TRUE
		/// for uncompressed video and FALSE for compressed video. 
		/// </summary>
		public bool bFixedSizeSamples;

		/// <summary>
		/// If TRUE, samples are compressed using temporal (interframe) compression.
		/// A value of TRUE indicates that not all frames are key frames.
		/// This field is informational only. 
		/// </summary>
		public bool bTemporalCompression;

		/// <summary>
		/// Size of the sample, in bytes. For compressed data, the value can be zero. 
		/// </summary>
		public int lSampleSize;

		/// <summary>
		/// GUID specifying the format type. The pbFormat member points to the
		/// corresponding format structure.
		/// </summary>
		public Guid formattype;

		/// <summary>
		/// This field is not used.
		/// </summary>
		public object pUnk;

		/// <summary>
		/// Size of the format block of the media type. 
		/// </summary>
		public int cbFormat;

		/// <summary>
		/// Pointer to the format structure. The structure type is specified by the
		/// formattype member. The format structure must be present,
		/// unless formattype is GUID_NULL or FORMAT_None. 
		/// </summary>
		public IntPtr pbFormat;

		/// <summary>
		/// Initializes a new instance of the DMO_MEDIA_TYPE class.
		/// </summary>
		public DMO_MEDIA_TYPE()
		{
			pbFormat = IntPtr.Zero;
		}

		/// <summary>
		/// Associates this instance with the specified audio format, allocating
		/// native memory as appropriate.
		/// </summary>
		/// <param name="fmt">
		/// Audio format to associate with this instance.
		/// </param>
		public void SetFormat(WAVEFORMATEX fmt)
		{
			pbFormat = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(WAVEFORMATEX)));
			if (pbFormat == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}
			Marshal.StructureToPtr((object)fmt, pbFormat, true);
		}

		/// <summary>
		/// Finalizes an instance of the DMO_MEDIA_TYPE class.
		/// </summary>
		~DMO_MEDIA_TYPE()
		{
			Dispose(disposing: false);
		}

		/// <summary>
		/// Frees native resources associated with this object, such as the memory
		/// used to hold the format structure.
		/// </summary>
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(disposing: true);
		}

		/// <summary>
		/// Frees native resources associated with this object, such as the memory
		/// used to hold the format structure.
		/// </summary>
		/// <param name="disposing">
		/// If <code>false</code>, the method has been called by the runtime from
		/// inside the finalizer and you should not reference other objects.
		/// Only unmanaged resources can be disposed.
		/// </param>
		private void Dispose(bool disposing)
		{
			if (pbFormat != IntPtr.Zero)
			{
				Marshal.FreeCoTaskMem(pbFormat);
			}
			pbFormat = IntPtr.Zero;
		}
	}
}
