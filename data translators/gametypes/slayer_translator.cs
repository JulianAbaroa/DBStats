using System.Xml;

namespace DBStats.DataTypes;

public class SlayerTranslator
{
    public static Slayer Execute(XmlNode player)
    {
        var customStats = player.SelectSingleNode("CustomStats")
            ?? throw new NullReferenceException("CustomStats is null.");

        double rating = Convert.ToDouble(customStats.Attributes?["RATING"]?.Value);

        return new Slayer
        {
            Rating = rating,
        };
    }

}