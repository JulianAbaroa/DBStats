using DBStats.DataTypes.GameTypes;
using System.Xml;

namespace DBStats.DataTranslators.GameTypes;

public class TerritoriesTranslator
{
    public static Territories Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
                    ?? throw new NullReferenceException("CustomStats is null.");

        int captures = Convert.ToInt32(customStats.Attributes?["Captures"]?.Value);

        return new Territories
        {
            Captures = captures,
        };
    }
}