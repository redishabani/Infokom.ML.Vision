using System;

namespace Microsoft.Kinect
{
	/// <summary>
	/// The generic DataPool class for a key with four values.
	/// </summary>
	/// <typeparam name="K">The key type.</typeparam>
	/// <typeparam name="V1">The first value type.</typeparam>
	/// <typeparam name="V2">The second value type.</typeparam>
	/// <typeparam name="V3">The third value type.</typeparam>
	/// <typeparam name="V4">The fourth value type.</typeparam>
	internal class DataPool<K, V1, V2, V3, V4>
	{
		/// <summary>
		/// A basic Key/Value pair.
		/// </summary>
		public abstract class Entry
		{
			/// <summary>
			/// Gets or sets the key of the entry.
			/// </summary>
			public K Key { get; set; }

			/// <summary>
			/// Gets or sets the first value.
			/// </summary>
			public V1 Value1 { get; set; }

			/// <summary>
			/// Gets or sets the second value.
			/// </summary>
			public V2 Value2 { get; set; }

			/// <summary>
			/// Gets or sets the third value.
			/// </summary>
			public V3 Value3 { get; set; }

			/// <summary>
			/// Gets or sets the fourth value.
			/// </summary>
			public V4 Value4 { get; set; }
		}

		/// <summary>
		/// Private subclass of Entry, used for additional bookkeping.
		/// </summary>
		private class EntryPrivate : Entry
		{
			/// <summary>
			/// The current reference count.
			/// </summary>
			private int _refCount;

			/// <summary>
			/// Gets a value indicating whether the object is locked or not.
			/// </summary>
			public bool IsLocked => _refCount > 0;

			/// <summary>
			/// Gets or sets a value indicating whether the object is in a pool.
			/// </summary>
			public bool IsInPool { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the object is valid.
			/// </summary>
			public bool IsValid { get; set; }

			/// <summary>
			/// Adds a reference count to the entry.
			/// </summary>
			public void AddRef()
			{
				_refCount++;
			}

			/// <summary>
			/// Releases a reference count from the entry.
			/// </summary>
			public void Release()
			{
				_refCount--;
			}
		}

		/// <summary>
		/// The array of private entries.
		/// </summary>
		private readonly EntryPrivate[] _dataPool;

		/// <summary>
		/// The next available entry.
		/// </summary>
		private int _nextPoolEntry;

		/// <summary>
		/// The lock object used to synchronize access.
		/// </summary>
		private readonly object _dataPoolLock = new object();

		/// <summary>
		/// Initializes a new instance of the DataPool class.
		/// </summary>
		/// <param name="length">The number of pool entries to manage.</param>
		public DataPool(int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			_dataPool = new EntryPrivate[length];
			for (int i = 0; i < _dataPool.Length; i++)
			{
				_dataPool[i] = new EntryPrivate();
				_dataPool[i].IsInPool = true;
				_dataPool[i].IsValid = true;
			}
		}

		/// <summary>
		/// Returns a free entry, allocating if necessary.
		/// Must be checked in via CheckInEntryForUpdate.
		/// </summary>
		/// <returns>A new Entry to be updated and checked in via CheckInEntryForUpdate.</returns>
		public Entry CheckOutFreeEntryForUpdate()
		{
			lock (_dataPoolLock)
			{
				int nextPoolEntry = _nextPoolEntry;
				_nextPoolEntry = (_nextPoolEntry + 1) % _dataPool.Length;
				EntryPrivate entryPrivate = _dataPool[nextPoolEntry];
				if (!entryPrivate.IsValid || entryPrivate.IsLocked)
				{
					entryPrivate.IsInPool = false;
					entryPrivate = new EntryPrivate();
					entryPrivate.IsInPool = true;
					entryPrivate.IsValid = true;
					_dataPool[nextPoolEntry] = entryPrivate;
				}
				entryPrivate.IsValid = false;
				return entryPrivate;
			}
		}

		/// <summary>
		/// Marks an entry as fully updated, ready to be retrieved via TryLockEntry.
		/// </summary>
		/// <param name="entry">
		///   The entry that is done being updated, as retrived from CHeckOutFreeEntryForUpdate.
		///   The Key may not be null.
		/// </param>
		public void CheckInEntryForUpdate(Entry entry)
		{
			lock (_dataPoolLock)
			{
				EntryPrivate entryPrivate = (EntryPrivate)entry;
				if (entryPrivate.IsInPool && !entryPrivate.IsLocked && !entryPrivate.IsValid)
				{
					entryPrivate.IsValid = true;
				}
			}
		}

		/// <summary>
		/// Attempt to find and lock an entry given the provided Key.  If successful, must be unlocked
		/// via UnlockEntry.
		/// </summary>
		/// <param name="key">The Key to use to look up an entry.  Cannot be null.</param>
		/// <param name="entry">The entry if found, null otherwise.</param>
		/// <returns>True if a matching entry is found, false otherwise.</returns>
		public bool TryLockEntry(K key, out Entry entry)
		{
			entry = null;
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			lock (_dataPoolLock)
			{
				for (int i = 0; i < _dataPool.Length; i++)
				{
					if (_dataPool[i].IsValid && _dataPool[i].Key.Equals(key))
					{
						EntryPrivate entryPrivate = _dataPool[i];
						entryPrivate.AddRef();
						entry = entryPrivate;
						break;
					}
				}
			}
			return entry != null;
		}

		/// <summary>
		/// Unlocks an entry retrieved via TryLockEntry.  Must be an Entry retrieved from this DataPool instance.
		/// </summary>
		/// <param name="entry">The entry to unlock.</param>
		public void UnlockEntry(Entry entry)
		{
			lock (_dataPoolLock)
			{
				EntryPrivate entryPrivate = (EntryPrivate)entry;
				entryPrivate.Release();
			}
		}
	}
	/// <summary>
	/// The generic DataPool class for a key with three values.
	/// </summary>
	/// <typeparam name="K">The key type.</typeparam>
	/// <typeparam name="V1">The first value type.</typeparam>
	/// <typeparam name="V2">The second value type.</typeparam>
	/// <typeparam name="V3">The third value type.</typeparam>
	internal class DataPool<K, V1, V2, V3> : DataPool<K, V1, V2, V3, bool?>
	{
		/// <summary>
		/// Initializes a new instance of the DataPool class.
		/// </summary>
		/// <param name="length">The number of pool entries to manage.</param>
		public DataPool(int length)
			: base(length)
		{
		}
	}
}
