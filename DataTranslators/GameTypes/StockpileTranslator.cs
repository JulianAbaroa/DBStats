using DBStats.DataTypes.GameTypes;
using System.Xml;

namespace DBStats.DataTranslators.GameTypes;

public class StockpileTranslator
{
    public static Stockpile Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
                    ?? throw new NullReferenceException("CustomStats is null.");

        var carryTimeNode = customStats.SelectSingleNode("CustomStat[@mStatName='CARRY TIME']")
            ?? throw new NullReferenceException("carryTimeNode is null.");

        double carryTime = Convert.ToDouble(carryTimeNode.Attributes?["mValueForDisplay"]?.Value);

        return new Stockpile
        {
            CarryTime = carryTime,
        };
    }
}