using System.Xml;

public class StockpileTranslator
{
    public static Stockpile Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
                    ?? throw new NullReferenceException("CustomStats is null.");

        double carryTime = Convert.ToDouble(customStats.Attributes?["CARRY TIME"]?.Value);

        return new Stockpile
        {
            CarryTime = carryTime,
        };
    }
}