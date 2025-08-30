using DBStats.DataTypes.GameTypes;
using System.Xml;

namespace DBStats.DataTranslators.GameTypes;

public class AssaultTranslator
{
    public static Assault Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
            ?? throw new NullReferenceException("CustomStats is null.");

        var bombsPlantedNode = customStats.SelectSingleNode("CustomStat[@mStatName='Bombs Planted']")
            ?? throw new NullReferenceException("bombsPlantedNode is null.");

        int bombsPlanted = Convert.ToInt32(bombsPlantedNode.Attributes?["mValueForDisplay"]?.Value);

        var detonationsNode = customStats.SelectSingleNode("CustomStat[@mStatName='Detonations']")
            ?? throw new NullReferenceException("detonationsNode is null.");

        int detonations = Convert.ToInt32(detonationsNode.Attributes?["mValueForDisplay"]?.Value);

        var bombCarryTimeNode = customStats.SelectSingleNode("CustomStat[@mStatName='Bomb Carry Time']")
            ?? throw new NullReferenceException("bombCarryTimeNode is null.");

        string? bombCarryTimeString = bombCarryTimeNode.Attributes?["mValueForDisplay"]?.Value
            ?? throw new NullReferenceException("bombCarryTimeString is null.");

        double bombCarryTime = Utils.GetMinutesFromString(bombCarryTimeString);

        var defusesNode = customStats.SelectSingleNode("CustomStat[@mStatName='Defuses']")
            ?? throw new NullReferenceException("defusesNode is null.");

        int defuses = Convert.ToInt32(defusesNode.Attributes?["mValueForDisplay"]?.Value);

        return new Assault
        {
            BombsPlanted = bombsPlanted,
            Detonations = detonations,
            BombCarryTime = bombCarryTime,
            Defuses = defuses,
        };
    }
}