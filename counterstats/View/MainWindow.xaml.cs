using counterstats.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace counterstats
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MainWindowViewModel mainWindowViewModel = new();

		public MainWindow()
		{
			DataContext = mainWindowViewModel;
			InitializeComponent();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			_ = Process.Start(new ProcessStartInfo
			{
				FileName = e.Uri.AbsoluteUri,
				UseShellExecute = true
			});
		}

		private void ItemsControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
			{
				DragMove();
				e.Handled = true;
			}
		}

		private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
			{
				e.Handled = true;
			}
		}
	}
}
