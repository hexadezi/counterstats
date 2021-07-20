# counterstats

## Screenshot
![](https://github.com/labo89/counterstats/blob/master/screenshots/screen_01.png)

## Prerequisites
- .NET 5.0 Runtime: https://dotnet.microsoft.com/download/dotnet/5.0
- Steam API Key: https://steamcommunity.com/dev/apikey

## Usage
1. Set con_logfile in cs:go as follows: `con_logfile console.log`
2. Download and open the app
3. Fill out the settings and restart app
4. In cs:go open the console and type `status`

## Tip
- Add `con_logfile console.log` to your autoexec
- Create a bind for status as follows: `bind "v" "status"`

## Settings
Setting | Explanation
------------ | -------------
"CsgoPath" | Path to the console output file with escaping character. Example: `E:\\Steam\\steamapps\\common\\Counter-Strike Global Offensive\\csgo\\console.log`
"ApiKey | Your Steam API Key. Example: `02B4E128C242ACFD1C6B4340F216E3B4`
"MySteamID" | Your Steam ID. Example: `76561197960287930`
"IgnoreFriends" | Ignore own friends. Friends must be public and MySteamID must be set. `True` or `false`.
"IgnoreOwnId" | Ignore own id. MySteamID must be set. `True` or `false`.
"OnTop" | Sets a value that indicates whether a window appears in the topmost z-order. `True` or `false`.


## Download
Download here: https://github.com/labo89/counterstats/releases

## License
This project is licensed under the MIT License - see the LICENSE file for details
