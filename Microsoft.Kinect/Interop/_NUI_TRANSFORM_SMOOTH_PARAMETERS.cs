using System.Runtime.InteropServices;

namespace Microsoft.Kinect.Interop
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct _NUI_TRANSFORM_SMOOTH_PARAMETERS
	{
		public float fSmoothing;

		public float fCorrection;

		public float fPrediction;

		public float fJitterRadius;

		public float fMaxDeviationRadius;
	}
}
