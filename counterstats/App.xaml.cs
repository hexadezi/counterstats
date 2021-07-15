using System.Windows;

namespace counterstats
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			SettingsProvider.Load();
			if (SettingsProvider.Settings.ApiKey == "" || SettingsProvider.Settings.CsgoPath == "")
			{
				_ = MessageBox.Show("Please specify the Api Key and the path to the console output file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				System.Diagnostics.Process.Start("notepad.exe", SettingsProvider.Savefile);
				Current.Shutdown();
			}
		}
	}
}
