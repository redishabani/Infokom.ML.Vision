using System;
using System.Threading;

namespace Microsoft.Kinect
{
	/// <summary>
	/// A Disposable wrapper for a ReaderWriterLockSlim, which provides for easy 
	/// scoped locking via a using block.  Also provides Write Upgrade functionality.
	/// </summary>
	internal struct AutoLock : IDisposable
	{
		/// <summary>
		/// Kind of lock you need.
		/// </summary>
		internal enum AutoLockMode
		{
			/// <summary>
			/// No lock is desired.
			/// </summary>
			None,
			/// <summary>
			/// A Read lock is desired.
			/// </summary>
			Read,
			/// <summary>
			/// A Read lock that may be upgraded to Write lock is desired.
			/// </summary>
			UpgradeableRead,
			/// <summary>
			/// A Write lock is desired.
			/// </summary>
			Write
		}

		/// <summary>
		/// Lockable object to lock and unlock.
		/// </summary>
		private readonly ReaderWriterLockSlim _lock;

		/// <summary>
		/// Desired lock mode.
		/// </summary>
		private AutoLockMode _lockMode;

		/// <summary>
		/// Initializes a new instance of the AutoLock struct.  Enters the desired lock mode on the given lock.
		/// </summary>
		/// <param name="readerWriterLock">Lock the given lock.</param>
		/// <param name="initialAutoLockMode">Lock mode to start with.  If mode is upgradable, this may change.</param>
		public AutoLock(ReaderWriterLockSlim readerWriterLock, AutoLockMode initialAutoLockMode)
		{
			_lock = readerWriterLock;
			_lockMode = initialAutoLockMode;
			switch (_lockMode)
			{
			case AutoLockMode.Read:
				_lock.EnterReadLock();
				break;
			case AutoLockMode.UpgradeableRead:
				_lock.EnterUpgradeableReadLock();
				break;
			case AutoLockMode.Write:
				_lock.EnterWriteLock();
				break;
			default:
				throw new ArgumentOutOfRangeException("initialAutoLockMode");
			}
		}

		/// <summary>
		/// Upgrades the lock from UpgradableRead to Write.
		/// </summary>
		/// <returns>New AutoLock object with write lock.</returns>
		public AutoLock UpgradeToWrite()
		{
			if (_lockMode != AutoLockMode.UpgradeableRead)
			{
				throw new InvalidOperationException();
			}
			return new AutoLock(_lock, AutoLockMode.Write);
		}

		/// <summary>
		/// Unlocks the given lock and sets the LockMode to none.
		/// </summary>
		public void Dispose()
		{
			switch (_lockMode)
			{
			case AutoLockMode.Read:
				_lock.ExitReadLock();
				break;
			case AutoLockMode.UpgradeableRead:
				_lock.ExitUpgradeableReadLock();
				break;
			case AutoLockMode.Write:
				_lock.ExitWriteLock();
				break;
			default:
				throw new InvalidOperationException();
			case AutoLockMode.None:
				break;
			}
			_lockMode = AutoLockMode.None;
		}
	}
}
