using DBStats.DataTypes.GameTypes;
using System.Xml;

namespace DBStats.DataTranslators.GameTypes;

public class TerritoriesTranslator
{
    public static Territories Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
            ?? throw new NullReferenceException("CustomStats is null.");

        var capturesNode = customStats.SelectSingleNode("CustomStat[@mStatName='Captures']")
            ?? throw new NullReferenceException("capturesNode is null.");

        int captures = Convert.ToInt32(capturesNode.Attributes?["mValueForDisplay"]?.Value);

        return new Territories
        {
            Captures = captures,
        };
    }
}