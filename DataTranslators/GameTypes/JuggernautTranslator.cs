using System.Xml;

namespace DBStats.DataTranslators.GameTypes;

public class JuggernautTranslator
{
    public static Juggernaut Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
                    ?? throw new NullReferenceException("CustomStats is null.");

        double juggernautTime = Convert.ToDouble(customStats.Attributes?["Juggernaut Time"]?.Value);

        return new Juggernaut
        {
            JuggernautTime = juggernautTime,
        };
    }
}