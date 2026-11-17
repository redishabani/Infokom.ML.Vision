using System.Threading;

namespace Microsoft.Kinect
{
	/// <summary>
	/// AutoLock extensions for ReaderWriterLockSlim.
	/// </summary>
	internal static class ReaderWriterLockSlimExtensionMethods
	{
		/// <summary>
		/// Returns a read AutoLock on the given lock.
		/// </summary>
		/// <param name="readerWriterLock">Slim to act upon.</param>
		/// <returns>New AutoLock object.</returns>
		public static AutoLock EnterReadLockAuto(this ReaderWriterLockSlim readerWriterLock)
		{
			return new AutoLock(readerWriterLock, AutoLock.AutoLockMode.Read);
		}

		/// <summary>
		/// Returns an upgradable AutoLock for the given lock.
		/// </summary>
		/// <param name="readerWriterLock">Slim to act upon.</param>
		/// <returns>New AutoLock object.</returns>
		public static AutoLock EnterUpgradeableReadLockAuto(this ReaderWriterLockSlim readerWriterLock)
		{
			return new AutoLock(readerWriterLock, AutoLock.AutoLockMode.UpgradeableRead);
		}

		/// <summary>
		/// Returns an write AutoLock for the given lock.
		/// </summary>
		/// <param name="readerWriterLock">Slim to act upon.</param>
		/// <returns>New AutoLock object.</returns>
		public static AutoLock EnterWriteLockAuto(this ReaderWriterLockSlim readerWriterLock)
		{
			return new AutoLock(readerWriterLock, AutoLock.AutoLockMode.Write);
		}
	}
}
