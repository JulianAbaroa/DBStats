using System.Xml;

public class KingOfTheHillTranslator
{
    public static KingOfTheHill Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
            ?? throw new NullReferenceException("CustomStats is null.");

        double timeinHill = Convert.ToDouble(customStats.Attributes?["Time in Hill"]?.Value);

        return new KingOfTheHill
        {
            TimeinHill = timeinHill,
        };
    }
}