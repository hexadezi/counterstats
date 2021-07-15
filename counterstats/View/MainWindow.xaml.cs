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
			InitializeComponent();
			DataContext = mainWindowViewModel;
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			_ = Process.Start(new ProcessStartInfo
			{
				FileName = e.Uri.AbsoluteUri,
				UseShellExecute = true
			});
		}
	}
}
