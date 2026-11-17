using System;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Specifies the FMTID/PID identifier that programmatically identifies a
	/// property.
	/// </summary>
	internal struct PROPERTYKEY
	{
		/// <summary>
		/// A unique GUID for the property.
		/// </summary>
		public Guid fmtid;

		/// <summary>
		/// A property identifier (PID). It is recommended that you set this value
		/// to PID_FIRST_USABLE. Any value greater than or equal to 2 is
		/// acceptable.
		/// </summary>
		public int pid;
	}
}
