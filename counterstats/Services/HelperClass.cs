using System;
using System.IO;
using System.Net;
using System.Xml;

namespace counterstats.Services
{
	public class HelperClass
	{
		public static string DownloadText(string url)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			var response = (HttpWebResponse)request.GetResponse();

			using (var reader = new StreamReader(response.GetResponseStream()))
			{
				return reader.ReadToEnd();
			}
		}
		public static string GetXmlpage(string address)
		{
			return DownloadText(address);
		}

		public static string ConvertToID64(string steamID32)
		{
			long v = 0x0110000100000000;

			System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(steamID32, @"STEAM_(\d):(\d):(.*)");

			int y, z;

			y = Int32.Parse(match.Groups[2].ToString());
			z = Int32.Parse(match.Groups[3].ToString());

			//W=Z*2+V+Y
			return ((z * 2) + v + y).ToString();
		}
		public static XmlDocument GetXmlPlayerBans(string id32)
		{
			string id64 = ConvertToID64(id32);
			XmlDocument xmlPlayerBans = new XmlDocument();
			xmlPlayerBans.LoadXml(GetXmlpage(@"http://api.steampowered.com/ISteamUser/GetPlayerBans/v0001/?key=" + SettingsProvider.Settings.ApiKey + "&format=xml&steamids=" + id64));
			return xmlPlayerBans;
		}
		public static XmlDocument GetXmlPlayerSummaries(string id32)
		{
			string id64 = ConvertToID64(id32);
			XmlDocument xmlPlayerSummaries = new XmlDocument();
			xmlPlayerSummaries.LoadXml(GetXmlpage(@"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + SettingsProvider.Settings.ApiKey + "&format=xml&steamids=" + id64));
			return xmlPlayerSummaries;
		}
	}


}
