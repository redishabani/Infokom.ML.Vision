using System.ComponentModel;
using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This class contains all of the available color camera settings.
	/// </summary>
	public sealed class ColorCameraSettings : INotifyPropertyChanged
	{
		/// <summary>
		/// This holds a reference to the associated sensor.
		/// </summary>
		private readonly NuiColorCameraSettings _colorCameraSettings;

		/// <summary>
		/// Gets or sets a value indicating whether the auto exposure is enabled.
		/// </summary>
		public bool AutoExposure
		{
			get
			{
				return _colorCameraSettings.GetAutoExposure();
			}
			set
			{
				if (value != AutoExposure)
				{
					_colorCameraSettings.SetAutoExposure(value);
					NotifyPropertyChanged("AutoExposure");
				}
			}
		}

		/// <summary>
		/// Gets or sets a value for the exposure time.
		/// </summary>
		/// <remarks>This property takes effect only when AutoExposure is false.</remarks>
		public double ExposureTime
		{
			get
			{
				return _colorCameraSettings.GetExposureTime();
			}
			set
			{
				if (!value.Equals(ExposureTime))
				{
					_colorCameraSettings.SetExposureTime(value);
					NotifyPropertyChanged("ExposureTime");
				}
			}
		}

		/// <summary>
		/// Gets the minimum exposure time.
		/// </summary>
		public double MinExposureTime => _colorCameraSettings.GetMinExposureTime();

		/// <summary>
		/// Gets the maximum exposure time.
		/// </summary>
		public double MaxExposureTime => _colorCameraSettings.GetMaxExposureTime();

		/// <summary>
		/// Gets or sets a value for the frame interval.
		/// </summary>
		/// <remarks>This property takes effect only when AutoExposure is false.</remarks>
		public double FrameInterval
		{
			get
			{
				return _colorCameraSettings.GetFrameInterval();
			}
			set
			{
				if (!value.Equals(FrameInterval))
				{
					_colorCameraSettings.SetFrameInterval(value);
					NotifyPropertyChanged("FrameInterval");
				}
			}
		}

		/// <summary>
		/// Gets the minimum frame interval.
		/// </summary>
		public double MinFrameInterval => _colorCameraSettings.GetMinFrameInterval();

		/// <summary>
		/// Gets the maximum frame interval.
		/// </summary>
		public double MaxFrameInterval => _colorCameraSettings.GetMaxFrameInterval();

		/// <summary>
		/// Gets or sets a value for the brightness.
		/// </summary>
		/// <remarks>This property takes effect only when AutoExposure is true.</remarks>
		public double Brightness
		{
			get
			{
				return _colorCameraSettings.GetBrightness();
			}
			set
			{
				if (!value.Equals(Brightness))
				{
					_colorCameraSettings.SetBrightness(value);
					NotifyPropertyChanged("Brightness");
				}
			}
		}

		/// <summary>
		/// Gets the minimum brightness.
		/// </summary>
		public double MinBrightness => _colorCameraSettings.GetMinBrightness();

		/// <summary>
		/// Gets the maximum brightness.
		/// </summary>
		public double MaxBrightness => _colorCameraSettings.GetMaxBrightness();

		/// <summary>
		/// Gets or sets the desired backlight compensation mode.
		/// </summary>
		/// <remarks>This property takes effect only when AutoExposure is true.</remarks>
		public BacklightCompensationMode BacklightCompensationMode
		{
			get
			{
				return (BacklightCompensationMode)_colorCameraSettings.GetBacklightCompensationMode();
			}
			set
			{
				if (value != BacklightCompensationMode)
				{
					_colorCameraSettings.SetBacklightCompensationMode((_NUI_BACKLIGHT_COMPENSATION_MODE)value);
					NotifyPropertyChanged("BacklightCompensationMode");
				}
			}
		}

		/// <summary>
		/// Gets or sets the desired power line frequency.
		/// </summary>
		/// <remarks>This property takes effect only when AutoExposure is true.</remarks>
		public PowerLineFrequency PowerLineFrequency
		{
			get
			{
				return (PowerLineFrequency)_colorCameraSettings.GetPowerLineFrequency();
			}
			set
			{
				if (value != PowerLineFrequency)
				{
					_colorCameraSettings.SetPowerLineFrequency((_NUI_POWER_LINE_FREQUENCY)value);
					NotifyPropertyChanged("PowerLineFrequency");
				}
			}
		}

		/// <summary>
		/// Gets or sets a value for the gain.
		/// </summary>
		/// <remarks>This property takes effect only when AutoExposure is false.</remarks>
		public double Gain
		{
			get
			{
				return _colorCameraSettings.GetGain();
			}
			set
			{
				if (!value.Equals(Gain))
				{
					_colorCameraSettings.SetGain(value);
					NotifyPropertyChanged("Gain");
				}
			}
		}

		/// <summary>
		/// Gets the minimum gain.
		/// </summary>
		public double MinGain => _colorCameraSettings.GetMinGain();

		/// <summary>
		/// Gets the maximum gain.
		/// </summary>
		public double MaxGain => _colorCameraSettings.GetMaxGain();

		/// <summary>
		/// Gets or sets a value indicating whether the white balance is automatic or not.
		/// </summary>
		public bool AutoWhiteBalance
		{
			get
			{
				return _colorCameraSettings.GetAutoWhiteBalance();
			}
			set
			{
				if (value != AutoWhiteBalance)
				{
					_colorCameraSettings.SetAutoWhiteBalance(value);
					NotifyPropertyChanged("AutoWhiteBalance");
				}
			}
		}

		/// <summary>
		/// Gets or sets a value for the white balance.
		/// </summary>
		/// <remarks>This property takes effect only when AutoWhiteBalance is false.</remarks>
		public int WhiteBalance
		{
			get
			{
				return _colorCameraSettings.GetWhiteBalance();
			}
			set
			{
				if (value != WhiteBalance)
				{
					_colorCameraSettings.SetWhiteBalance(value);
					NotifyPropertyChanged("WhiteBalance");
				}
			}
		}

		/// <summary>
		/// Gets the minimum white balance.
		/// </summary>
		public int MinWhiteBalance => _colorCameraSettings.GetMinWhiteBalance();

		/// <summary>
		/// Gets the maximum white balance.
		/// </summary>
		public int MaxWhiteBalance => _colorCameraSettings.GetMaxWhiteBalance();

		/// <summary>
		/// Gets or sets a value for the white balance.
		/// </summary>
		public double Contrast
		{
			get
			{
				return _colorCameraSettings.GetContrast();
			}
			set
			{
				if (!value.Equals(Contrast))
				{
					_colorCameraSettings.SetContrast(value);
					NotifyPropertyChanged("Contrast");
				}
			}
		}

		/// <summary>
		/// Gets the minimum white balance.
		/// </summary>
		public double MinContrast => _colorCameraSettings.GetMinContrast();

		/// <summary>
		/// Gets the maximum white balance.
		/// </summary>
		public double MaxContrast => _colorCameraSettings.GetMaxContrast();

		/// <summary>
		/// Gets or sets a value for the white balance.
		/// </summary>
		public double Hue
		{
			get
			{
				return _colorCameraSettings.GetHue();
			}
			set
			{
				if (!value.Equals(Hue))
				{
					_colorCameraSettings.SetHue(value);
					NotifyPropertyChanged("Hue");
				}
			}
		}

		/// <summary>
		/// Gets the minimum hue.
		/// </summary>
		public double MinHue => _colorCameraSettings.GetMinHue();

		/// <summary>
		/// Gets the maximum hue.
		/// </summary>
		public double MaxHue => _colorCameraSettings.GetMaxHue();

		/// <summary>
		/// Gets or sets a value for the saturation.
		/// </summary>
		public double Saturation
		{
			get
			{
				return _colorCameraSettings.GetSaturation();
			}
			set
			{
				if (!value.Equals(Saturation))
				{
					_colorCameraSettings.SetSaturation(value);
					NotifyPropertyChanged("Saturation");
				}
			}
		}

		/// <summary>
		/// Gets the minimum saturation.
		/// </summary>
		public double MinSaturation => _colorCameraSettings.GetMinSaturation();

		/// <summary>
		/// Gets the maximum saturation.
		/// </summary>
		public double MaxSaturation => _colorCameraSettings.GetMaxSaturation();

		/// <summary>
		/// Gets or sets a value for the gamma.
		/// </summary>
		public double Gamma
		{
			get
			{
				return _colorCameraSettings.GetGamma();
			}
			set
			{
				if (!value.Equals(Gamma))
				{
					_colorCameraSettings.SetGamma(value);
					NotifyPropertyChanged("Gamma");
				}
			}
		}

		/// <summary>
		/// Gets the minimum gamma.
		/// </summary>
		public double MinGamma => _colorCameraSettings.GetMinGamma();

		/// <summary>
		/// Gets the maximum gamma.
		/// </summary>
		public double MaxGamma => _colorCameraSettings.GetMaxGamma();

		/// <summary>
		/// Gets or sets a value for the sharpness.
		/// </summary>
		public double Sharpness
		{
			get
			{
				return _colorCameraSettings.GetSharpness();
			}
			set
			{
				if (!value.Equals(Sharpness))
				{
					_colorCameraSettings.SetSharpness(value);
					NotifyPropertyChanged("Sharpness");
				}
			}
		}

		/// <summary>
		/// Gets the minimum sharpness.
		/// </summary>
		public double MinSharpness => _colorCameraSettings.GetMinSharpness();

		/// <summary>
		/// Gets the maximum sharpness.
		/// </summary>
		public double MaxSharpness => _colorCameraSettings.GetMaxSharpness();

		/// <summary>
		/// This event triggers when a related property changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Initializes a new instance of the ColorCameraSettings class.
		/// </summary>
		/// <param name="mainNui">The sensor these settings relate to.</param>
		internal ColorCameraSettings(KinectSensor mainNui)
		{
			_colorCameraSettings = mainNui.NuiSensor.NuiGetColorCameraSettings();
		}

		/// <summary>
		/// Resets all color camera settings to their defaults.
		/// </summary>
		public void ResetToDefault()
		{
			bool autoExposure = AutoExposure;
			bool autoWhiteBalance = AutoWhiteBalance;
			BacklightCompensationMode backlightCompensationMode = BacklightCompensationMode;
			double brightness = Brightness;
			double contrast = Contrast;
			double exposureTime = ExposureTime;
			double frameInterval = FrameInterval;
			double gain = Gain;
			double gamma = Gamma;
			double hue = Hue;
			PowerLineFrequency powerLineFrequency = PowerLineFrequency;
			double saturation = Saturation;
			double sharpness = Sharpness;
			int whiteBalance = WhiteBalance;
			_colorCameraSettings.ResetCameraSettingsToDefault();
			CheckAndTriggerIfChanged(autoExposure, AutoExposure, "AutoExposure");
			CheckAndTriggerIfChanged(autoWhiteBalance, AutoWhiteBalance, "AutoWhiteBalance");
			CheckAndTriggerIfChanged(backlightCompensationMode, BacklightCompensationMode, "BacklightCompensationMode");
			CheckAndTriggerIfChanged(brightness, Brightness, "Brightness");
			CheckAndTriggerIfChanged(contrast, Contrast, "Contrast");
			CheckAndTriggerIfChanged(exposureTime, ExposureTime, "ExposureTime");
			CheckAndTriggerIfChanged(frameInterval, FrameInterval, "FrameInterval");
			CheckAndTriggerIfChanged(gain, Gain, "Gain");
			CheckAndTriggerIfChanged(gamma, Gamma, "Gamma");
			CheckAndTriggerIfChanged(hue, Hue, "Hue");
			CheckAndTriggerIfChanged(powerLineFrequency, PowerLineFrequency, "PowerLineFrequency");
			CheckAndTriggerIfChanged(saturation, Saturation, "Saturation");
			CheckAndTriggerIfChanged(sharpness, Sharpness, "Sharpness");
			CheckAndTriggerIfChanged(whiteBalance, WhiteBalance, "WhiteBalance");
		}

		/// <summary>
		/// This triggers the NotifyPropertyChanged if the value is different.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		/// <param name="propertyName">The property name.</param>
		private void CheckAndTriggerIfChanged(double oldValue, double newValue, string propertyName)
		{
			if (!oldValue.Equals(newValue))
			{
				NotifyPropertyChanged(propertyName);
			}
		}

		/// <summary>
		/// This triggers the NotifyPropertyChanged if the value is different.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		/// <param name="propertyName">The property name.</param>
		private void CheckAndTriggerIfChanged(bool oldValue, bool newValue, string propertyName)
		{
			if (oldValue != newValue)
			{
				NotifyPropertyChanged(propertyName);
			}
		}

		/// <summary>
		/// This triggers the NotifyPropertyChanged if the value is different.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		/// <param name="propertyName">The property name.</param>
		private void CheckAndTriggerIfChanged(PowerLineFrequency oldValue, PowerLineFrequency newValue, string propertyName)
		{
			if (oldValue != newValue)
			{
				NotifyPropertyChanged(propertyName);
			}
		}

		/// <summary>
		/// This triggers the NotifyPropertyChanged if the value is different.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		/// <param name="propertyName">The property name.</param>
		private void CheckAndTriggerIfChanged(BacklightCompensationMode oldValue, BacklightCompensationMode newValue, string propertyName)
		{
			if (oldValue != newValue)
			{
				NotifyPropertyChanged(propertyName);
			}
		}

		/// <summary>
		/// Notifies the client of property changes.
		/// </summary>
		/// <param name="propertyName">The name of the property that changed.</param>
		private void NotifyPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
