namespace Microsoft.Kinect
{
	/// <summary>
	/// This structure defines the format of waveform-audio data.
	/// </summary>
	internal struct WAVEFORMATEX
	{
		/// <summary>
		/// Waveform-audio format type.
		/// </summary>
		public ushort wFormatTag;

		/// <summary>
		/// Number of channels in the waveform-audio data. Monaural data uses one
		/// channel and stereo data uses two channels.
		/// </summary>
		public ushort nChannels;

		/// <summary>
		/// Sample rate, in samples per second (Hertz), that each channel should be
		/// played or recorded. If wFormatTag is WAVE_FORMAT_PCM, then common values
		/// for nSamplesPerSecare 8.0 kHz, 11.025 kHz, 22.05 kHz, and 44.1 kHz. For
		/// non-PCM formats, this member must be computed according to the
		/// manufacturer's specification of the format tag.
		/// </summary>
		public uint nSamplesPerSec;

		/// <summary>
		/// Required average data-transfer rate, in bytes per second, for the format
		/// tag. If wFormatTag is WAVE_FORMAT_PCM, nAvgBytesPerSec should be equal to
		/// the product of nSamplesPerSec and nBlockAlign. For non-PCM formats, this
		/// member must be computed according to the manufacturer's specification of
		/// the format tag.
		/// Playback and record software can estimate buffer sizes by using the
		/// nAvgBytesPerSec member. 
		/// </summary>
		public uint nAvgBytesPerSec;

		/// <summary>
		/// Block alignment, in bytes. The block alignment is the minimum atomic unit
		/// of data for the wFormatTag format type. If wFormatTag is WAVE_FORMAT_PCM,
		/// nBlockAlign should be equal to the product of nChannels and wBitsPerSample
		/// divided by 8 (bits per byte). For non-PCM formats, this member must be
		/// computed according to the manufacturer's specification of the format tag.
		/// Playback and record software must process a multiple of nBlockAlign bytes
		/// of data at a time. Data written and read from a device must always start
		/// at the beginning of a block. For example, it is illegal to start playback
		/// of PCM data in the middle of a sample (that is, on a non-block-aligned
		/// boundary). 
		/// </summary>
		public ushort nBlockAlign;

		/// <summary>
		/// Bits per sample for the wFormatTag format type. If wFormatTag is
		/// WAVE_FORMAT_PCM, then wBitsPerSample should be equal to 8 or 16. For
		/// non-PCM formats, this member must be set according to the manufacturer's
		/// specification of the format tag. Some compression schemes cannot define a
		/// value for wBitsPerSample, so this member can be zero.
		/// </summary>
		public ushort wBitsPerSample;

		/// <summary>
		/// Size, in bytes, of extra format information appended to the end of the
		/// WAVEFORMATEX structure. This information can be used by non-PCM formats to
		/// store extra attributes for the wFormatTag. If no extra information is
		/// required by the wFormatTag, this member must be set to zero.
		/// For WAVE_FORMAT_PCM formats only, this member is ignored.
		/// </summary>
		public ushort cbSize;
	}
}
