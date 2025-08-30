using DBStats.DataTypes.GameTypes;
using System.Xml;

namespace DBStats.DataTranslators.GameTypes;

public class HeadHunterTranslator
{
    public static HeadHunter Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
                    ?? throw new NullReferenceException("CustomStats is null.");

        var maxSkullsNode = customStats.SelectSingleNode("CustomStat[@mStatName='MAX SKULLS']")
            ?? throw new NullReferenceException("ratingNode is null.");

        int maxSkulls = Convert.ToInt32(maxSkullsNode.Attributes?["mValueForDisplay"]?.Value);

        return new HeadHunter
        {
            MaxSkulls = maxSkulls,
        };
    }

}