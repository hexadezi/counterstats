using counterstats.Model;
using counterstats.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace counterstats.ViewModel
{
	public class MainWindowViewModel : ViewModelBase
	{
		private readonly object _myCollectionLock = new();
		private readonly TailNET tailNET;
		public ObservableCollection<Player> Players { get; set; } = new ObservableCollection<Player>();
		public MainWindowViewModel()
		{
			BindingOperations.EnableCollectionSynchronization(Players, _myCollectionLock);
			tailNET = new TailNET(SettingsProvider.Settings.CsgoPath);
			tailNET.LineAdded += TailNET_LineAdded;
			tailNET.Start();
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
					   Player p = new(match.Groups[1].ToString());
					   lock (_myCollectionLock) { Players.Add(p); }
				   });
			}
		}
	}
}
