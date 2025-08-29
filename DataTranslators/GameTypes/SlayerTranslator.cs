using DBStats.DataTypes.GameTypes;
using System.Xml;

namespace DBStats.DataTranslators.GameTypes;

public class SlayerTranslator
{
    public static Slayer Execute(XmlNode player)
    {
        var customStats = player.SelectSingleNode("CustomStats")
            ?? throw new NullReferenceException("CustomStats is null.");

        var ratingNode = customStats.SelectSingleNode("CustomStat[@mStatName='RATING']")
            ?? throw new NullReferenceException("ratingNode is null.");

        double rating = Convert.ToDouble(ratingNode.Attributes?["mValueForDisplay"]?.Value);

        return new Slayer
        {
            Rating = rating,
        };
    }

}