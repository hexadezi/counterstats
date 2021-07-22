using counterstats.Model;
using counterstats.Services;
using counterstats.ViewModel.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Xml;

namespace counterstats.ViewModel
{
	public class MainWindowViewModel : ViewModelBase
	{
		private readonly object _myCollectionLock = new();
		private TailNET tailNET;
		public ObservableCollection<Player> Players { get; set; } = new ObservableCollection<Player>();
		public List<string> IDsToIgnore { get; set; } = new();
		public RelayCommand OpenAll { get; set; }
		public RelayCommand Open { get; set; }
		public static bool OnTop => SettingsProvider.Settings.OnTop;
		public MainWindowViewModel()
		{
			Initialize();
			InitCommand();
		}

		private void Initialize()
		{
			BindingOperations.EnableCollectionSynchronization(Players, _myCollectionLock);
			tailNET = new TailNET(SettingsProvider.Settings.CsgoPath);
			tailNET.LineAdded += TailNET_LineAdded;
			tailNET.Start();

			_ = Task.Run(() =>
			  {
				  if (SettingsProvider.Settings.IgnoreFriends && SettingsProvider.Settings.MySteamID != "")
				  {
					  try
					  {
						  XmlDocument XmlFriends = HelperClass.GetXmlPlayerFriends(SettingsProvider.Settings.MySteamID);

						  foreach (XmlElement item in XmlFriends.DocumentElement.SelectNodes("/friendslist/friends/friend/steamid"))
						  {
							  IDsToIgnore.Add(item.InnerText);
							  Debug.WriteLine("Ignore: " + item.InnerText);
						  }
					  }
					  catch (WebException)
					  {
						  Debug.WriteLine("No friends for: " + SettingsProvider.Settings.MySteamID);
					  }
				  }
			  });

		}

		private void InitCommand()
		{

			OpenAll = new RelayCommand((param) =>
			{
				Debug.WriteLine($"Clicked: {param}");
				foreach (Player player in Players)
				{
					_ = Process.Start(new ProcessStartInfo
					{
						FileName = player[param.ToString()],
						UseShellExecute = true
					}); ;
				}
			});

			Open = new RelayCommand((param) =>
			{
				Debug.WriteLine($"Clicked: {param}");
				_ = Process.Start(new ProcessStartInfo
				{
					FileName = ((Hyperlink)param).NavigateUri.AbsoluteUri,
					UseShellExecute = true
				}); ;
			});

		}

		private void TailNET_LineAdded(object sender, string e)
		{
			if (e.StartsWith("# userid name uniqueid"))
			{
				lock (_myCollectionLock) { Players.Clear(); }
			}

			Match match = Regex.Match(e, ".*(STEAM_\\d:\\d:[^\\s]+)");
			if (match.Success)
			{
				_ = Task.Run(() =>
				   {
					   Player p = new(match.Groups[1].Value);
					   if (SettingsProvider.Settings.MySteamID != "")
					   {
						   if (p.SteamID64 == SettingsProvider.Settings.MySteamID && SettingsProvider.Settings.IgnoreOwnId)
						   {
							   return;
						   }

						   if (SettingsProvider.Settings.IgnoreFriends == true)
						   {
							   if (IDsToIgnore.Contains(p.SteamID64))
							   {
								   return;
							   }
						   }
					   }
					   lock (_myCollectionLock) { Players.Add(p); }
				   });
			}
		}
	}
}
