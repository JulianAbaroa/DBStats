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

        double timeinHill = Convert.ToDouble(timeinHillNode.Attributes?["mValueForDisplay"]?.Value);

        return new KingOfTheHill
        {
            TimeinHill = timeinHill,
        };
    }
}