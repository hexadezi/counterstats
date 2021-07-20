using System;
using System.IO;
using System.Net;
using System.Xml;

namespace counterstats.Services
{
	public class HelperClass
	{
		internal static string DownloadText(string url)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			using StreamReader reader = new(response.GetResponseStream());
			return reader.ReadToEnd();
		}
		internal static string GetXmlpage(string address)
		{
			return DownloadText(address);
		}

		internal static string ConvertToID64(string steamID32)
		{
			long v = 0x0110000100000000;

			System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(steamID32, @"STEAM_(\d):(\d):(.*)");

			int y, z;

			y = Int32.Parse(match.Groups[2].ToString());
			z = Int32.Parse(match.Groups[3].ToString());

			//W=Z*2+V+Y
			return ((z * 2) + v + y).ToString();
		}
		internal static XmlDocument GetXmlPlayerBans(string id32)
		{
			string id64 = ConvertToID64(id32);
			XmlDocument xmlPlayerBans = new();
			xmlPlayerBans.LoadXml(GetXmlpage(@"http://api.steampowered.com/ISteamUser/GetPlayerBans/v0001/?key=" + SettingsProvider.Settings.ApiKey + "&format=xml&steamids=" + id64));
			return xmlPlayerBans;
		}
		internal static XmlDocument GetXmlPlayerSummaries(string id32)
		{
			string id64 = ConvertToID64(id32);
			XmlDocument xmlPlayerSummaries = new();
			xmlPlayerSummaries.LoadXml(GetXmlpage(@"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + SettingsProvider.Settings.ApiKey + "&format=xml&steamids=" + id64));
			return xmlPlayerSummaries;
		}

		internal static XmlDocument GetXmlPlayerFriends(string id64)
		{
			XmlDocument xmlPlayerFriends = new();
			xmlPlayerFriends.LoadXml(GetXmlpage(@"http://api.steampowered.com/ISteamUser/GetFriendList/v1/?key=" + SettingsProvider.Settings.ApiKey + "&format=xml&steamid=" + id64 + "&relationship=friend"));
			return xmlPlayerFriends;
		}
	}


}
