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

        string? carryTimeString = carryTimeNode.Attributes?["mValueForDisplay"]?.Value
            ?? throw new NullReferenceException("carryTime is null.");

        double carryTime = Utils.GetMinutesFromString(carryTimeString);

        return new Stockpile
        {
            CarryTime = carryTime,
        };
    }
}