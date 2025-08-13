using System.Xml;

public class PlayerTranslator
{
    public Player Execute(XmlNode player)
    {
        string name = player.Attributes!["mGamertagText"]?.Value!;
        string serviceID = player.Attributes!["ServiceId"]?.Value!;
        string clanTag = player.Attributes!["ClantagText"]?.Value!;

        return new Player
        {
            Name = name,
            ClanTag = clanTag,
            ServiceID = serviceID,
        };
    }

}