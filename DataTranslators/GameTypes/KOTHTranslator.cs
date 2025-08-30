using DBStats.DataTypes.GameTypes;
using System.Xml;

namespace DBStats.DataTranslators.GameTypes;

public class KOTHTranslator
{
    public static KingOfTheHill Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
            ?? throw new NullReferenceException("CustomStats is null.");

        var timeinHillNode = customStats.SelectSingleNode("CustomStat[@mStatName='Time in Hill']")
            ?? throw new NullReferenceException("timeinHillNode is null.");

        string? timeinHillString = timeinHillNode.Attributes?["mValueForDisplay"]?.Value
            ?? throw new NullReferenceException("timeInHill is null.");

        double timeInHill = Utils.GetMinutesFromString(timeinHillString);

        return new KingOfTheHill
        {
            TimeinHill = timeInHill,
        };
    }
}