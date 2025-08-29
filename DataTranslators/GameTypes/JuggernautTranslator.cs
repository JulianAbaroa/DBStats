using DBStats.DataTypes.GameTypes;
using System.Xml;

namespace DBStats.DataTranslators.GameTypes;

public class JuggernautTranslator
{
    public static Juggernaut Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
                    ?? throw new NullReferenceException("CustomStats is null.");

        var juggernautTimeNode = customStats.SelectSingleNode("CustomStat[@mStatName='Juggernaut Time']")
            ?? throw new NullReferenceException("juggernautTimeNode is null.");

        double juggernautTime = Convert.ToDouble(juggernautTimeNode.Attributes?["mValueForDisplay"]?.Value);

        return new Juggernaut
        {
            JuggernautTime = juggernautTime,
        };
    }
}