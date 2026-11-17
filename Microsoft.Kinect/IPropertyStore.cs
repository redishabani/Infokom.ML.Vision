using System.Runtime.InteropServices;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Exposes methods for enumerating, getting, and setting property values.
	/// </summary>
	[ComImport]
	[Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IPropertyStore
	{
		/// <summary>
		/// Gets the number of properties attached to the file.
		/// </summary>
		/// <param name="cProps">
		/// Receives the number of properties in store.
		/// </param>
		void GetCount(out uint cProps);

		/// <summary>
		/// Gets a property key from the store's array of properties.
		/// </summary>
		/// <param name="iProp">
		/// The index of the property key in the array of <see cref="T:Microsoft.Kinect.PROPERTYKEY" />
		/// structures. This is a zero-based index.
		/// </param>
		/// <param name="pkey">
		/// Receives a <see cref="T:Microsoft.Kinect.PROPERTYKEY" /> structure that contains the unique
		/// identifier for a property.
		/// </param>
		void GetAt([In] uint iProp, out PROPERTYKEY pkey);

		/// <summary>
		/// Gets data for a specific property.
		/// </summary>
		/// <param name="key">
		/// <see cref="T:Microsoft.Kinect.PROPERTYKEY" /> structure that contains a unique identifier
		/// for the property in question.
		/// </param>
		/// <param name="pv">
		/// Receives an object that contains the property data.
		/// </param>
		void GetValue([In] ref PROPERTYKEY key, out object pv);

		/// <summary>
		/// Sets a new property value, or replaces or removes an existing value.
		/// </summary>
		/// <param name="key">
		/// <see cref="T:Microsoft.Kinect.PROPERTYKEY" /> structure that contains a unique identifier
		/// for the property in question.
		/// </param>
		/// <param name="pv">
		/// Reference to an object that contains the new property data.
		/// </param>
		void SetValue([In] ref PROPERTYKEY key, [In] ref object pv);

		/// <summary>
		/// Saves a property change.
		/// </summary>
		void Commit();
	}
}
