using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

using System.Windows;
using System.Windows.Media;

namespace Infokom.ML.Vision.YOLO.WPF.Detect
{
	/// <summary>
	/// Interaction logic for DetectWindow.xaml
	/// </summary>
	public partial class DetectWindow : Window
	{
		public DetectWindow()
		{
			this.InitializeComponent();
		}

		private void ButtonPlay_Click(object sender, RoutedEventArgs e)
		{

		}
	}


	public partial class DetectViewModel : ObservableObject
	{

		[ObservableProperty]
		public partial string ModelName { get; set; }

		[ObservableProperty]
		public partial string SourceName { get; set; }

		[ObservableProperty]
		public partial ImageSource CurrentSourceFrame { get; set; }

		[ObservableProperty]
		public partial ImageSource CurrentResultFrame { get; set; }





		[RelayCommand]
		private void PlayCommandHandler()
		{
			if(string.IsNullOrEmpty(this.SourceName))
			{
				OpenFileDialog dialog = new()
				{
					Multiselect = false,
					Filter = "mp4"
				};

				if(dialog.ShowDialog() ?? false)
				{
					this.SourceName = dialog.FileName;
				}
			}


		}

		[RelayCommand]
		private void PauseCommandHandler()
		{

		}

		[RelayCommand]
		private void StopCommandHandler()
		{

		}
	}
}
