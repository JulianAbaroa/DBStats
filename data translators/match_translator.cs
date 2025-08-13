using System.Xml;

public class MatchTranslator
{
    public Match Excute(XmlNode file)
    {
        XmlNode typeNode = file.SelectSingleNode("GameEnum")!;

        // Este es el que tengo que analizar para obtener cuales son todos los otros modos de juego.
        string type = typeNode.Attributes!["mGameEnum"]?.Value! == "6" ? "CTF" : "Unknown";

        XmlNode idNode = file.SelectSingleNode("GameUniqueId")!;
        string id = idNode.Attributes!["GameUniqueId"]?.Value!;

        return new Match
        {
            Type = type,
            ID = id
        };
    }

}