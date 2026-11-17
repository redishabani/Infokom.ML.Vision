using Microsoft.Kinect.Interop;

namespace Microsoft.Kinect
{
	/// <summary>
	/// This is the wrapper class for the color camera settings.
	/// </summary>
	internal class NuiColorCameraSettings
	{
		/// <summary>
		/// This field holds on to the INuiColorCameraSettings object.
		/// </summary>
		private readonly INuiColorCameraSettings _nuiColorCameraSettings;

		/// <summary>
		/// Initializes a new instance of the NuiColorCameraSettings class.
		/// </summary>
		/// <param name="nuiColorCameraSettings">The underlying native settings object.</param>
		public NuiColorCameraSettings(INuiColorCameraSettings nuiColorCameraSettings)
		{
			_nuiColorCameraSettings = nuiColorCameraSettings;
		}

		/// <summary>
		/// This method enables or disables the sensor's auto exposure setting.
		/// </summary>
		/// <param name="bAutoWhiteBalanceEnabled">The bool indicating if the setting is enabled or disabled.</param>
		public void SetAutoWhiteBalance(bool bAutoWhiteBalanceEnabled)
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.SetAutoWhiteBalance(bAutoWhiteBalanceEnabled ? 1 : 0));
		}

		/// <summary>
		/// Gets the current value for auto white balance.
		/// </summary>
		/// <returns>Returns true if this is enabled, false otherwise.</returns>
		public bool GetAutoWhiteBalance()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetAutoWhiteBalance(out var pAutoWhiteBalanceEnabled));
			return pAutoWhiteBalanceEnabled != 0;
		}

		/// <summary>
		/// Sets the white balance.
		/// </summary>
		/// <remarks>This property only works when AutoWhiteBalance is disabled.</remarks>
		/// <param name="whiteBalance">The desired white balance.</param>
		public void SetWhiteBalance(int whiteBalance)
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.SetWhiteBalance(whiteBalance));
		}

		/// <summary>
		/// Gets the current white balance setting.
		/// </summary>
		/// <remarks>If AutoWhiteBalance is true, this will only return the last known value.</remarks>
		/// <returns>The current value of the white balance setting.</returns>
		public int GetWhiteBalance()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetWhiteBalance(out var pWhiteBalance));
			return pWhiteBalance;
		}

		/// <summary>
		/// Gets the minimum acceptable value for white balance.
		/// </summary>
		/// <returns>The minimum value.</returns>
		public int GetMinWhiteBalance()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMinWhiteBalance(out var pWhiteBalance));
			return pWhiteBalance;
		}

		/// <summary>
		/// Gets the maximum acceptable value for white balance.
		/// </summary>
		/// <returns>The maximum value.</returns>
		public int GetMaxWhiteBalance()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMaxWhiteBalance(out var pWhiteBalance));
			return pWhiteBalance;
		}

		/// <summary>
		/// Sets the desired contrast.
		/// </summary>
		/// <param name="contrast">The desired contrast.</param>
		public void SetContrast(double contrast)
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.SetContrast(contrast));
		}

		/// <summary>
		/// Gets the current contrast.
		/// </summary>
		/// <returns>The current contrast.</returns>
		public double GetContrast()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetContrast(out var pContrast));
			return pContrast;
		}

		/// <summary>
		/// Gets the minimum contrast.
		/// </summary>
		/// <returns>The minimum.</returns>
		public double GetMinContrast()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMinContrast(out var pContrast));
			return pContrast;
		}

		/// <summary>
		/// Gets the maximum contrast.
		/// </summary>
		/// <returns>The maximum.</returns>
		public double GetMaxContrast()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMaxContrast(out var pContrast));
			return pContrast;
		}

		/// <summary>
		/// Sets the desired hue.
		/// </summary>
		/// <param name="hue">The desired hue.</param>
		public void SetHue(double hue)
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.SetHue(hue));
		}

		/// <summary>
		/// Gets the current hue.
		/// </summary>
		/// <returns>The current hue.</returns>
		public double GetHue()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetHue(out var pHue));
			return pHue;
		}

		/// <summary>
		/// Gets the minimum hue.
		/// </summary>
		/// <returns>The minimum.</returns>
		public double GetMinHue()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMinHue(out var pHue));
			return pHue;
		}

		/// <summary>
		/// Gets the maximum hue.
		/// </summary>
		/// <returns>The maximum.</returns>
		public double GetMaxHue()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMaxHue(out var pHue));
			return pHue;
		}

		/// <summary>
		/// Sets the desired saturation.
		/// </summary>
		/// <param name="saturation">The desired saturation.</param>
		public void SetSaturation(double saturation)
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.SetSaturation(saturation));
		}

		/// <summary>
		/// Gets the current saturation.
		/// </summary>
		/// <returns>The current saturation.</returns>
		public double GetSaturation()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetSaturation(out var pSaturation));
			return pSaturation;
		}

		/// <summary>
		/// Gets the minimum saturation.
		/// </summary>
		/// <returns>The minimum.</returns>
		public double GetMinSaturation()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMinSaturation(out var pSaturation));
			return pSaturation;
		}

		/// <summary>
		/// Gets the maximum saturation.
		/// </summary>
		/// <returns>The maximum.</returns>
		public double GetMaxSaturation()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMaxSaturation(out var pSaturation));
			return pSaturation;
		}

		/// <summary>
		/// Sets the desired gamma.
		/// </summary>
		/// <param name="gamma">The desired gamma.</param>
		public void SetGamma(double gamma)
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.SetGamma(gamma));
		}

		/// <summary>
		/// Gets the current gamma.
		/// </summary>
		/// <returns>The current gamma.</returns>
		public double GetGamma()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetGamma(out var pGamma));
			return pGamma;
		}

		/// <summary>
		/// Gets the minimum gamma.
		/// </summary>
		/// <returns>The minimum.</returns>
		public double GetMinGamma()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMinGamma(out var pGamma));
			return pGamma;
		}

		/// <summary>
		/// Gets the maximum gamma.
		/// </summary>
		/// <returns>The maximum.</returns>
		public double GetMaxGamma()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMaxGamma(out var pGamma));
			return pGamma;
		}

		/// <summary>
		/// Sets the desired sharpness.
		/// </summary>
		/// <param name="sharpness">The desired sharpness.</param>
		public void SetSharpness(double sharpness)
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.SetSharpness(sharpness));
		}

		/// <summary>
		/// Gets the current sharpness.
		/// </summary>
		/// <returns>The current sharpness.</returns>
		public double GetSharpness()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetSharpness(out var pSharpness));
			return pSharpness;
		}

		/// <summary>
		/// Gets the minimum sharpness.
		/// </summary>
		/// <returns>The minimum.</returns>
		public double GetMinSharpness()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMinSharpness(out var pSharpness));
			return pSharpness;
		}

		/// <summary>
		/// Gets the maximum sharpness.
		/// </summary>
		/// <returns>The maximum.</returns>
		public double GetMaxSharpness()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMaxSharpness(out var pSharpness));
			return pSharpness;
		}

		/// <summary>
		/// This method enables or disables the sensor's auto exposure setting.
		/// </summary>
		/// <param name="bAutoExposureEnabled">The bool indicating if the setting is enabled or disabled.</param>
		public void SetAutoExposure(bool bAutoExposureEnabled)
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.SetAutoExposure(bAutoExposureEnabled ? 1 : 0));
		}

		/// <summary>
		/// Gets the current value for auto exposure.
		/// </summary>
		/// <returns>Returns true if enabled, false otherwise.</returns>
		public bool GetAutoExposure()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetAutoExposure(out var pAutoExposureEnabled));
			return pAutoExposureEnabled != 0;
		}

		/// <summary>
		/// Sets the desired exposure time.
		/// </summary>
		/// <param name="exposureTime">The desired exposure time.</param>
		public void SetExposureTime(double exposureTime)
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.SetExposureTime(exposureTime));
		}

		/// <summary>
		/// Gets the current exposure time.
		/// </summary>
		/// <returns>The current exposure time.</returns>
		public double GetExposureTime()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetExposureTime(out var pExposureTime));
			return pExposureTime;
		}

		/// <summary>
		/// Gets the minimum exposure time.
		/// </summary>
		/// <returns>The minimum.</returns>
		public double GetMinExposureTime()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMinExposureTime(out var pExposureTime));
			return pExposureTime;
		}

		/// <summary>
		/// Gets the maximum exposure time.
		/// </summary>
		/// <returns>The maximum.</returns>
		public double GetMaxExposureTime()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMaxExposureTime(out var pExposureTime));
			return pExposureTime;
		}

		/// <summary>
		/// Sets the desired frame interval.
		/// </summary>
		/// <param name="frameInterval">The desired frame interval.</param>
		public void SetFrameInterval(double frameInterval)
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.SetFrameInterval(frameInterval));
		}

		/// <summary>
		/// Gets the current frame interval.
		/// </summary>
		/// <returns>The current frame interval.</returns>
		public double GetFrameInterval()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetFrameInterval(out var pFrameInterval));
			return pFrameInterval;
		}

		/// <summary>
		/// Gets the minimum frame interval.
		/// </summary>
		/// <returns>The minimum.</returns>
		public double GetMinFrameInterval()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMinFrameInterval(out var pFrameInterval));
			return pFrameInterval;
		}

		/// <summary>
		/// Gets the maximum frame interval.
		/// </summary>
		/// <returns>The maximum.</returns>
		public double GetMaxFrameInterval()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMaxFrameInterval(out var pFrameInterval));
			return pFrameInterval;
		}

		/// <summary>
		/// Sets the desired brightness.
		/// </summary>
		/// <param name="brightness">The desired brightness..</param>
		public void SetBrightness(double brightness)
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.SetBrightness(brightness));
		}

		/// <summary>
		/// Gets the current brightness.
		/// </summary>
		/// <returns>The current brightness.</returns>
		public double GetBrightness()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetBrightness(out var pBrightness));
			return pBrightness;
		}

		/// <summary>
		/// Gets the minimum brightness.
		/// </summary>
		/// <returns>The minimum.</returns>
		public double GetMinBrightness()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMinBrightness(out var pBrightness));
			return pBrightness;
		}

		/// <summary>
		/// Gets the maximum brightness.
		/// </summary>
		/// <returns>The maximum.</returns>
		public double GetMaxBrightness()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMaxBrightness(out var pBrightness));
			return pBrightness;
		}

		/// <summary>
		/// Sets the desired power line frequency.
		/// </summary>
		/// <param name="powerLineFrequency">The desired frequency.</param>
		public void SetPowerLineFrequency(_NUI_POWER_LINE_FREQUENCY powerLineFrequency)
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.SetPowerLineFrequency(powerLineFrequency));
		}

		/// <summary>
		/// Gets the current power line frequency.
		/// </summary>
		/// <returns>The current frequency.</returns>
		public _NUI_POWER_LINE_FREQUENCY GetPowerLineFrequency()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetPowerLineFrequency(out var pPowerLineFrequency));
			return pPowerLineFrequency;
		}

		/// <summary>
		/// Sets the desired backlight compensation mode.
		/// </summary>
		/// <param name="backlightCompensationMode">The desired mode.</param>
		public void SetBacklightCompensationMode(_NUI_BACKLIGHT_COMPENSATION_MODE backlightCompensationMode)
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.SetBacklightCompensationMode(backlightCompensationMode));
		}

		/// <summary>
		/// Gets the current backlight compensation mode.
		/// </summary>
		/// <returns>The current mode.</returns>
		public _NUI_BACKLIGHT_COMPENSATION_MODE GetBacklightCompensationMode()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetBacklightCompensationMode(out var pBacklightCompensationMode));
			return pBacklightCompensationMode;
		}

		/// <summary>
		/// Sets the desired gain.
		/// </summary>
		/// <param name="gain">The desired gain.</param>
		public void SetGain(double gain)
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.SetGain(gain));
		}

		/// <summary>
		/// Gets the current gain.
		/// </summary>
		/// <returns>The current gain.</returns>
		public double GetGain()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetGain(out var pGain));
			return pGain;
		}

		/// <summary>
		/// Gets the minimum gain.
		/// </summary>
		/// <returns>The minimum.</returns>
		public double GetMinGain()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMinGain(out var pGain));
			return pGain;
		}

		/// <summary>
		/// Gets the maximum gain.
		/// </summary>
		/// <returns>The maximum.</returns>
		public double GetMaxGain()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.GetMaxGain(out var pGain));
			return pGain;
		}

		/// <summary>
		/// This resets the color camera settings to their defaults.
		/// </summary>
		public void ResetCameraSettingsToDefault()
		{
			KinectExceptionHelper.CheckHr(_nuiColorCameraSettings.ResetCameraSettingsToDefault());
		}
	}
}
