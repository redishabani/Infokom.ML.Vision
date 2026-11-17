using System;

namespace Microsoft.Kinect
{
	/// <summary>
	/// These flags are used to indicate of a skeleton is passing outside of the frame.
	/// </summary>
	[Flags]
	public enum FrameEdges
	{
		/// <summary>
		/// All of the player's body is in frame.
		/// </summary>
		None = 0,
		/// <summary>
		/// Part of the player's body is out of frame to the camera's right.
		/// </summary>
		Right = 1,
		/// <summary>
		/// Part of the player's body is out of frame to the camera's left.
		/// </summary>
		Left = 2,
		/// <summary>
		/// Part of the player's body is out of frame above the camera's field of view.
		/// </summary>
		Top = 4,
		/// <summary>
		/// Part of the player's body is out of frame below the camera's field of view.
		/// </summary>
		Bottom = 8
	}
}
