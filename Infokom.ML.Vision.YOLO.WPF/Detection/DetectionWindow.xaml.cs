using Microsoft.Win32;

using System.Windows;

namespace Infokom.ML.Vision.YOLO.WPF.Detection
{
	public partial class DetectionWindow : Window
	{
		private readonly DetectionViewModel _viewModel;

		public DetectionWindow()
		{
			InitializeComponent();
			_viewModel = new DetectionViewModel(Dispatcher);
			DataContext = _viewModel;
		}

		private void BrowseModel_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				CheckFileExists = true,
				Filter = "ONNX models (*.onnx)|*.onnx|All files (*.*)|*.*",
				Title = "Choose an ONNX model"
			};

			if (dialog.ShowDialog(this) == true)
				_viewModel.ModelPath = dialog.FileName;
		}

		private async void BrowseVideo_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				CheckFileExists = true,
				Filter = "Video files|*.mp4;*.avi;*.mov;*.mkv;*.wmv|All files (*.*)|*.*",
				Title = "Choose a video file"
			};

			if (dialog.ShowDialog(this) == true)
				await _viewModel.SetVideoPathAsync(dialog.FileName);
		}

		protected override async void OnClosed(EventArgs e)
		{
			await _viewModel.DisposeAsync();
			base.OnClosed(e);
		}
	}
}
