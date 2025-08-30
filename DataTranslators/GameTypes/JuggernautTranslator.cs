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

        string? juggernautTimeString = juggernautTimeNode.Attributes?["mValueForDisplay"]?.Value
            ?? throw new NullReferenceException("juggernautTime is null.");

        double juggernautTime = Utils.GetMinutesFromString(juggernautTimeString);

        return new Juggernaut
        {
            JuggernautTime = juggernautTime,
        };
    }
}