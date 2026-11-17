using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Represents the collection of all Kinect™ sensors on the host.
	/// </summary>
	[ComVisible(false)]
	public sealed class KinectSensorCollection : ReadOnlyCollection<KinectSensor>, IDisposable
	{
		/// <summary>
		/// Lock for the sensor collection.
		/// </summary>
		private static readonly object s_sensorsLock = new object();

		/// <summary>
		/// List of acitve sensors.
		/// <remarks>
		/// This must be a ThreadSafeList because the base class (ReadOnlyCollection) may want to access it while s_sensorsLock is locked.
		/// </remarks>
		/// </summary>
		private static readonly ThreadSafeList<KinectSensor> s_activeSensors = new ThreadSafeList<KinectSensor>(s_sensorsLock);

		/// <summary>
		/// Static instance of this collection.
		/// </summary>
		private static readonly KinectSensorCollection s_instance = Initialize();

		/// <summary>
		/// List of all sensors.  Not all are active.
		/// </summary>
		private readonly List<KinectSensor> _sensors = new List<KinectSensor>();

		/// <summary>
		/// Caches native callback delegate to ensure it does not get garbage collected.
		/// </summary>
		private readonly NativeMethods.NuiStatusCallback _nuiCallBackDelegate;

		/// <summary>
		/// Called whenever a kinect changes status.
		/// </summary>
		private readonly ContextEventHandler<StatusChangedEventArgs> _statusChangedContextHandler;

		/// <summary>
		/// Gets the static instance of this collection.
		/// </summary>
		internal static KinectSensorCollection Instance => s_instance;

		/// <summary>
		/// Returns a sensor based on the DeviceConnectionId or UniqueKinectId.
		/// </summary>
		/// <param name="instanceId">Can pass in either the value from KinectSensor.DeviceConnectionId or KinectSensor.UniqueKinectId.</param>
		/// <returns>The sensor associated with the given id.</returns>
		public KinectSensor this[string instanceId]
		{
			get
			{
				lock (s_sensorsLock)
				{
					if (string.IsNullOrEmpty(instanceId))
					{
						throw new ArgumentNullException("instanceId");
					}
					KinectSensor kinectSensor = _sensors.FirstOrDefault((KinectSensor r) => r.GetDeviceConnectionId() == instanceId || r.GetUniqueKinectId() == instanceId);
					if (kinectSensor == null)
					{
						INuiSensor pNuiSensor = null;
						KinectExceptionHelper.CheckHr(NativeMethods.NuiCreateSensorById(instanceId, out pNuiSensor));
						kinectSensor = new KinectSensor(pNuiSensor);
						_sensors.Add(kinectSensor);
						this.UpdateActiveSensors();
					}
					return kinectSensor;
				}
			}
		}

		/// <summary>
		/// Called whenever a kinect changes status.  Sent from KinectSensorCollection.
		/// </summary>
		public event EventHandler<StatusChangedEventArgs> StatusChanged
		{
			add
			{
				_statusChangedContextHandler.AddHandler(value);
			}
			remove
			{
				_statusChangedContextHandler.RemoveHandler(value);
			}
		}

		/// <summary>
		/// Prevents a default instance of the KinectSensorCollection class from being created.
		/// Initializes a new instance of this collection.  
		/// </summary>
		private KinectSensorCollection()
			: base((IList<KinectSensor>)s_activeSensors)
		{
			_nuiCallBackDelegate = OnNuiStatusCallback;
			NativeMethods.NuiSetDeviceStatusCallback(_nuiCallBackDelegate, IntPtr.Zero);
			_statusChangedContextHandler = new ContextEventHandler<StatusChangedEventArgs>();
		}

		/// <summary>
		/// Singleton accessor or KinectSensorCollection.  Creates KinectSensor objects for all native INuiSensors.
		/// </summary>
		/// <returns>The singleton instance of this class.</returns>
		private static KinectSensorCollection Initialize()
		{
			KinectSensorCollection kinectSensorCollection = new KinectSensorCollection();
			try
			{
				KinectExceptionHelper.CheckHr(NativeMethods.NuiGetSensorCount(out var pCount));
				for (int i = 0; i < pCount; i++)
				{
					INuiSensor pNuiSensor = null;
					if (NativeMethods.NuiCreateSensorByIndex(i, out pNuiSensor) == 0)
					{
						KinectSensor item = new KinectSensor(pNuiSensor);
						kinectSensorCollection._sensors.Add(item);
					}
				}
				kinectSensorCollection.UpdateActiveSensors();
				return kinectSensorCollection;
			}
			catch (Exception)
			{
				kinectSensorCollection.Dispose();
				throw;
			}
		}

		/// <summary>
		/// Callback function for getting status notifications from native.
		/// </summary>
		/// <param name="status">HRESULT status from the sensor.</param>
		/// <param name="instanceName">Sensor hub device instance id.</param>
		/// <param name="uniqueDeviceName">Sensor camera device instance id.  May be null.</param>
		/// <param name="pUserData">User data for the callback.  Always null for this callback.</param>
		internal void OnNuiStatusCallback(int status, string instanceName, string uniqueDeviceName, IntPtr pUserData)
		{
			InternalStatusChangedEventArgs e = new InternalStatusChangedEventArgs();
			e.KinectStatus = KinectSensor.MapStatus((uint)status);
			e.DeviceConnectionId = instanceName;
			e.UniqueKinectId = uniqueDeviceName;
			InternalStatusChangedEventArgs state = e;
			NotifyUserKinectsStatusChanged(state);
		}

		/// <summary>
		/// Fires the StatusChanged event.
		/// </summary>
		/// <param name="state">InternalStatusChangedEventArgs object.</param>
		internal void NotifyUserKinectsStatusChanged(object state)
		{
			KinectSensor kinectSensor = null;
			InternalStatusChangedEventArgs internalArgs = (InternalStatusChangedEventArgs)state;
			lock (s_sensorsLock)
			{
				kinectSensor = _sensors.FirstOrDefault((KinectSensor r) => r.GetDeviceConnectionId() == internalArgs.DeviceConnectionId);
				if (internalArgs.KinectStatus == KinectStatus.Disconnected)
				{
					if (kinectSensor != null)
					{
						RemakeSensor(kinectSensor, internalArgs.DeviceConnectionId);
					}
				}
				else if (kinectSensor == null)
				{
					INuiSensor pNuiSensor = null;
					KinectExceptionHelper.CheckHr(NativeMethods.NuiCreateSensorById(internalArgs.DeviceConnectionId, out pNuiSensor));
					kinectSensor = new KinectSensor(pNuiSensor);
					_sensors.Add(kinectSensor);
				}
				if (kinectSensor != null)
				{
					kinectSensor.LastStatus = internalArgs.KinectStatus;
				}
				UpdateActiveSensors();
			}
			if (_statusChangedContextHandler.HasHandlers)
			{
				StatusChangedEventArgs e = new StatusChangedEventArgs();
				e.Status = internalArgs.KinectStatus;
				StatusChangedEventArgs e2 = e;
				e2.Sensor = kinectSensor;
				_statusChangedContextHandler.Invoke(this, e2);
			}
		}

		/// <summary>
		/// Updates the list of active sensors to contain all sensors that are connected.
		/// </summary>
		private void UpdateActiveSensors()
		{
			lock (s_sensorsLock)
			{
				s_activeSensors.Clear();
				s_activeSensors.AddRange(from r in _sensors
					where r.GetStatus() != KinectStatus.Disconnected
					orderby r.InstanceIndex
					select r);
			}
		}

		/// <summary>
		/// Removes a sensor from the list, allowing the sensor object to be recreated.
		/// </summary>
		/// <param name="sensor">KinectSensor to be removed.</param>
		/// <param name="name">Name of sensor to recreate.</param>
		internal void RemakeSensor(KinectSensor sensor, string name)
		{
			lock (s_sensorsLock)
			{
				_sensors.Remove(sensor);
			}
			if (sensor.LastStatus == KinectStatus.Connected && name != null)
			{
				try
				{
					sensor = Instance[name];
				}
				catch (InvalidOperationException)
				{
				}
			}
		}

		/// <summary>
		/// IDisposable.Dispose implementation.
		/// </summary>
		public void Dispose()
		{
			_statusChangedContextHandler.Dispose();
		}
	}
}
