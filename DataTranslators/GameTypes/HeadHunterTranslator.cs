using System.Xml;

namespace DBStats.DataTranslators.GameTypes;

public class HeadHunterTranslator
{
    public static HeadHunter Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
                    ?? throw new NullReferenceException("CustomStats is null.");

        int maxSkulls = Convert.ToInt32(customStats.Attributes?["MAX SKULLS"]?.Value);

        return new HeadHunter
        {
            MaxSkulls = maxSkulls,
        };
    }
}