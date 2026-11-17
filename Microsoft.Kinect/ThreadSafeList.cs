using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Kinect
{
	/// <summary>
	/// IList implementation with locking on all operations.
	/// </summary>
	/// <typeparam name="T">Type of generic IList to implement.</typeparam>
	internal class ThreadSafeList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		/// <summary>
		/// Provides a SNAPSHOT enumerator of the list.  Keep this in mind when using this enumerator.
		/// </summary>
		private class ThreadSafeEnumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			/// <summary>
			/// Snapshot to enumerate.
			/// </summary>
			private readonly ThreadSafeList<T> _list;

			/// <summary>
			/// Internal enumerator of the snapshot.
			/// </summary>
			private readonly IEnumerator<T> _enum;

			/// <summary>
			/// Gets the element in the collection at the current position of the enumerator.
			/// </summary>
			/// <returns>The element in the collection at the current position of the enumerator.</returns>
			public T Current
			{
				get
				{
					lock (_list._lock)
					{
						return _enum.Current;
					}
				}
			}

			/// <summary>
			/// Gets the element in the collection at the current position of the enumerator.
			/// </summary>
			/// <returns>The element in the collection at the current position of the enumerator.</returns>
			object IEnumerator.Current
			{
				get
				{
					lock (_list._lock)
					{
						return _enum.Current;
					}
				}
			}

			/// <summary>
			/// Initializes a new instance of the ThreadSafeEnumerator class, creating a snapshot of the given list.
			/// </summary>
			/// <param name="list">List to snapshot.</param>
			public ThreadSafeEnumerator(ThreadSafeList<T> list)
			{
				lock (list._lock)
				{
					_list = new ThreadSafeList<T>();
					_list.AddRange(list);
					_enum = _list._list.GetEnumerator();
				}
			}

			/// <summary>
			/// Disposes the underlying enumerator.  Does not set _list or _enum to null so calls will still
			/// proxy to the disposed instance (and throw the proper exception).
			/// </summary>
			public void Dispose()
			{
				lock (_list._lock)
				{
					_enum.Dispose();
				}
			}

			/// <summary>
			/// Advances the enumerator to the next element of the collection.
			/// </summary>
			/// <returns>
			/// true if the enumerator was successfully advanced to the next element; false
			/// if the enumerator has passed the end of the collection.
			/// </returns>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
			public bool MoveNext()
			{
				lock (_list._lock)
				{
					return _enum.MoveNext();
				}
			}

			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first element
			/// in the collection.
			/// </summary>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
			public void Reset()
			{
				lock (_list._lock)
				{
					_enum.Reset();
				}
			}
		}

		/// <summary>
		/// Lock object to use for all operations.
		/// </summary>
		private readonly object _lock;

		/// <summary>
		/// Wrapped list object.
		/// </summary>
		private readonly List<T> _list = new List<T>();

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public T this[int index]
		{
			get
			{
				lock (_lock)
				{
					return _list[index];
				}
			}
			set
			{
				lock (_lock)
				{
					_list[index] = value;
				}
			}
		}

		/// <summary>
		/// Gets the number of elements actually contained in the ThreadSafeList&lt;T&gt;.
		/// </summary>
		public int Count
		{
			get
			{
				lock (_lock)
				{
					return _list.Count;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the list is read only.  Returns true.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				lock (_lock)
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the ThreadSafeList class with a new lock.
		/// </summary>
		public ThreadSafeList()
			: this(new object())
		{
		}

		/// <summary>
		/// Initializes a new instance of the ThreadSafeList class with an existing new lock.
		/// </summary>
		/// <param name="critSec">Existing lock to use for this list.</param>
		public ThreadSafeList(object critSec)
		{
			_lock = critSec;
		}

		/// <summary>
		/// Adds the elements of the specified collection to the end of the ThreadSafeList&lt;T&gt;.
		/// </summary>
		/// <param name="collection">
		/// The collection whose elements should be added to the end of the ThreadSafeList&lt;T&gt;.
		/// The collection itself cannot be null, but it can contain elements that are
		/// null, if type T is a reference type.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">Collection is null.</exception>
		public void AddRange(IEnumerable<T> collection)
		{
			lock (_lock)
			{
				_list.AddRange(collection);
			}
		}

		/// <summary>
		/// Searches for the specified object and returns the zero-based index of the
		/// first occurrence within the entire ThreadSafeList&lt;T&gt;.
		/// </summary>
		/// <param name="item">
		/// The object to locate in the ThreadSafeList&lt;T&gt;. The value
		/// can be null for reference types.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of item within the entire ThreadSafeList&lt;T&gt;,
		/// if found; otherwise, –1.
		/// </returns>
		public int IndexOf(T item)
		{
			lock (_lock)
			{
				return _list.IndexOf(item);
			}
		}

		/// <summary>
		/// Inserts an element into the ThreadSafeList&lt;T&gt; at the specified
		/// index.
		/// </summary>
		/// <param name="index">
		/// The zero-based index at which item should be inserted.
		/// </param>
		/// <param name="item">
		/// The object to insert. The value can be null for reference types.
		/// </param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">Index is less than 0.-or-index is greater than ThreadSafeList&lt;T&gt;.Count.</exception>
		public void Insert(int index, T item)
		{
			lock (_lock)
			{
				_list.Insert(index, item);
			}
		}

		/// <summary>
		/// Removes the element at the specified index of the ThreadSafeList&lt;T&gt;.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">Index is less than 0.-or-index is equal to or greater than ThreadSafeList&lt;T&gt;.Count.</exception>
		public void RemoveAt(int index)
		{
			lock (_lock)
			{
				_list.RemoveAt(index);
			}
		}

		/// <summary>
		/// Adds an object to the end of the ThreadSafeList&lt;T&gt;.
		/// </summary>
		/// <param name="item">
		/// The object to be added to the end of the ThreadSafeList&lt;T&gt;.
		/// The value can be null for reference types.
		/// </param>
		public void Add(T item)
		{
			lock (_lock)
			{
				_list.Add(item);
			}
		}

		/// <summary>
		/// Removes all elements from the ThreadSafeList&lt;T&gt;.
		/// </summary>
		public void Clear()
		{
			lock (_lock)
			{
				_list.Clear();
			}
		}

		/// <summary>
		/// Determines whether an element is in the ThreadSafeList&lt;T&gt;.
		/// </summary>
		/// <param name="item">
		/// The object to locate in the ThreadSafeList&lt;T&gt;. The value
		/// can be null for reference types.
		/// </param>
		/// <returns>
		/// true if item is found in the ThreadSafeList&lt;T&gt;; otherwise,
		/// false.
		/// </returns>
		public bool Contains(T item)
		{
			lock (_lock)
			{
				return _list.Contains(item);
			}
		}

		/// <summary>
		/// Copies the entire ThreadSafeList&lt;T&gt; to a compatible one-dimensional
		/// array, starting at the beginning of the target array.
		/// </summary>
		/// <param name="array">
		/// The one-dimensional System.Array that is the destination of the elements
		/// copied from ThreadSafeList&lt;T&gt;. The System.Array must have
		/// zero-based indexing.
		/// </param>
		/// <param name="arrayIndex">
		/// The zero-based index in array at which copying begins.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">Array is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">ArrayIndex is less than 0.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// The number of elements in the source ThreadSafeList&lt;T&gt; is
		/// greater than the available space from arrayIndex to the end of the destination
		/// array.
		/// </exception>
		public void CopyTo(T[] array, int arrayIndex)
		{
			lock (_lock)
			{
				_list.CopyTo(array, arrayIndex);
			}
		}

		/// <summary>
		/// Copies the entire ThreadSafeList&lt;Tgt; to a compatible one-dimensional
		/// array, starting at the beginning of the target array.
		/// </summary>
		/// <param name="array">
		/// The one-dimensional System.Array that is the destination of the elements
		/// copied from System.Collections.Generic.List&lt;Tgt;. The System.Array must have
		/// zero-based indexing.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">Array is null.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// The number of elements in the source ThreadSafeList&lt;Tgt; is
		/// greater than the number of elements that the destination array can contain.
		/// </exception>
		public void CopyTo(T[] array)
		{
			lock (_lock)
			{
				_list.CopyTo(array);
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the ThreadSafeList&lt;T&gt;.
		/// </summary>
		/// <param name="item">
		/// The object to remove from the ThreadSafeList&lt;T&gt;. The value
		/// can be null for reference types.
		/// </param>
		/// <returns>
		/// true if item is successfully removed; otherwise, false. This method also
		/// returns false if item was not found in the ThreadSafeList&lt;T&gt;.
		/// </returns>
		public bool Remove(T item)
		{
			lock (_lock)
			{
				return _list.Remove(item);
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through the ThreadSafeList&lt;T&gt;.
		/// </summary>
		/// <remarks>This support function exists to satisfy code quality warning CA2000.  Otherwise, it would be in-line.</remarks>
		/// <returns>A ThreadSafeList&lt;T&gt;.Enumerator for the ThreadSafeList&lt;T&gt;.</returns>
		private IEnumerator<T> NewEnumerator()
		{
			return new ThreadSafeEnumerator(this);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the ThreadSafeList&lt;T&gt;.
		/// </summary>
		/// <remarks>This enumerator is a SNAPSHOT of the list.  Keep this in mind when using this enumerator.</remarks>
		/// <returns>A ThreadSafeList&lt;T&gt;.Enumerator for the ThreadSafeList&lt;T&gt;.</returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			lock (_lock)
			{
				return NewEnumerator();
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through the ThreadSafeList&lt;T&gt;.
		/// </summary>
		/// <remarks>This enumerator is a SNAPSHOT of the list.  Keep this in mind when using this enumerator.</remarks>
		/// <returns>A ThreadSafeList&lt;T&gt;.Enumerator for the ThreadSafeList&lt;T&gt;.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			lock (_lock)
			{
				return NewEnumerator();
			}
		}
	}
}
