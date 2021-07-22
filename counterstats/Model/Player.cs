using counterstats.Services;
using System;
using System.Globalization;
using System.Xml;

namespace counterstats.Model
{
	public class Player
	{
		public string SteamID64 { get; set; }
		public string CsgoStats => @"https://csgostats.gg/player/" + SteamID64;
		public string SteamProfile => @"https://steamcommunity.com/profiles/" + SteamID64;
		public string Steamrep => @"https://steamrep.com/profiles/" + SteamID64;
		public string Friends => @"https://steamcommunity.com/profiles/" + SteamID64 + "/friends";
		public string Inventory => @"https://steamcommunity.com/profiles/" + SteamID64 + "/inventory";
		public string FaceIt => @"https://www.faceit.com/de/search/player/" + SteamID64;
		public string Name { get; set; }
		public string Avatar { get; set; }
		public string TimeCreated { get; set; }
		public string CommunityVisibilityState { get; set; }
		public string EconomyBan { get; set; }
		public string CommunityBanned { get; set; }
		public string VACBanned { get; set; }
		public string DaysSinceLastBan { get; set; }
		public string AvatarFull { get; set; }
		public XmlDocument XmlPlayerSummaries { get; set; }
		public XmlDocument XmlPlayerBans { get; set; }
		public XmlDocument XmlFriends { get; set; }
		public dynamic this[string property] => GetType().GetProperty(property).GetValue(this, null);

		//public List<string> FriendList { get; set; } = new();

		public Player(string id32)
		{
			XmlPlayerBans = HelperClass.GetXmlPlayerBans(id32);
			XmlPlayerSummaries = HelperClass.GetXmlPlayerSummaries(id32);

			SteamID64 = XmlPlayerSummaries.DocumentElement.SelectSingleNode("/response/players/player/steamid").InnerText;
			Name = XmlPlayerSummaries.DocumentElement.SelectSingleNode("/response/players/player/personaname").InnerText;
			Avatar = XmlPlayerSummaries.DocumentElement.SelectSingleNode("/response/players/player/avatar").InnerText;
			AvatarFull = XmlPlayerSummaries.DocumentElement.SelectSingleNode("/response/players/player/avatarfull").InnerText;
			CommunityVisibilityState = XmlPlayerSummaries.DocumentElement.SelectSingleNode("/response/players/player/communityvisibilitystate").InnerText;




			if (CommunityVisibilityState == "3")
			{
				TimeCreated = DateTimeOffset.FromUnixTimeSeconds(Int32.Parse(XmlPlayerSummaries.DocumentElement.SelectSingleNode("/response/players/player/timecreated").InnerText, CultureInfo.InvariantCulture)).DateTime.ToString("[dd.MM.yy]", CultureInfo.InvariantCulture);
			}


			if (XmlPlayerBans.DocumentElement.SelectSingleNode("/response/players/player/CommunityBanned").InnerText == "true")
			{
				CommunityBanned = "COM";
			}

			if (XmlPlayerBans.DocumentElement.SelectSingleNode("/response/players/player/VACBanned").InnerText == "true")
			{
				VACBanned = "VAC";
			}

			if (XmlPlayerBans.DocumentElement.SelectSingleNode("/response/players/player/EconomyBan").InnerText != "none")
			{
				EconomyBan = "ECO";
			}

			if (XmlPlayerBans.DocumentElement.SelectSingleNode("/response/players/player/DaysSinceLastBan").InnerText != "0")
			{
				DaysSinceLastBan = "[" + XmlPlayerBans.DocumentElement.SelectSingleNode("/response/players/player/DaysSinceLastBan").InnerText + "]";
			}
		}
	}
}
