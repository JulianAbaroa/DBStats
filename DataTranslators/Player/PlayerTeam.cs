using System.Xml;

namespace DBStats.DataTranslators.Player;

public class PlayerTeam
{
    public static string GetPlayerTeam(XmlNode player)
    {
        int playerTeam = Convert.ToInt32(player.Attributes!["mTeamId"]?.Value!);

        return playerTeam switch
        {
            0 => "Red",
            1 => "Blue",
            2 => "Green",
            3 => "Orange",
            4 => "Purple",
            5 => "Gold",
            6 => "Brown",
            7 => "Pink",
            _ => "Unknown",
        };
    }
}