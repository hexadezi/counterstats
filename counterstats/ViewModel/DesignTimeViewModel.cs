using counterstats.ViewModel.Base;
using System;
using System.Collections.ObjectModel;

namespace counterstats.ViewModel
{
	public class DesignTimeViewModel : ViewModelBase
	{
		public ObservableCollection<Playerr> Players { get; set; } = new ObservableCollection<Playerr>();

		public DesignTimeViewModel()
		{
			Players.Add(new Playerr("nonas1dark"));
			Players.Add(new Playerr("popeye"));
			Players.Add(new Playerr("LIMONAD"));
			Players.Add(new Playerr("Mahatma"));
			Players.Add(new Playerr("Gomorrita"));
			Players.Add(new Playerr("Keiko*San"));
			Players.Add(new Playerr("kurelq"));
			Players.Add(new Playerr("-Rotax-"));
			Players.Add(new Playerr("󠀡󠀡󠀡⁧⁧8nano"));
			Players.Add(new Playerr("ecz_777"));
		}
	}

	public class Playerr
	{
		public string SteamID32 { get; set; } = "STEAM_1:0:126886670";
		public string SteamID64 { get; set; } = "76561198987564063";
		public string Name { get; set; } = "Max Mustermann";
		public string SteamProfile { get; set; } = @"https://steamcommunity.com/profiles/76561198987564063";
		public string CsgoStats { get; set; } = @"https://csgostats.gg/player/76561198987564063";
		public string Csgorunner { get; set; }
		public string Avatar { get; set; } = @"https://cdn.akamai.steamstatic.com/steamcommunity/public/images/avatars/fe/fef49e7fa7e1997310d705b2a6158ff8dc1cdfeb_full.jpg";
		public string TimeCreated { get; set; } = DateTime.Now.ToString("(dd.MM.yy)");
		public string Inventory { get; set; } = @"https://steamcommunity.com/profiles/76561198987564063/inventory";
		public string CommunityBanned { get; set; }
		public string VACBanned { get; set; } = "VAC";
		public string EconomyBan { get; set; } = "Trade banned";

		public Playerr(string s)
		{
			Name = s;
		}
	}
}
