using DBStats.DataTypes.GameTypes;
using System.Xml;

namespace DBStats.DataTranslators.GameTypes;

public class AssaultTranslator
{
    public static Assault Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
                    ?? throw new NullReferenceException("CustomStats is null.");

        int bombsPlanted = Convert.ToInt32(customStats.Attributes?["Bombs Planted"]?.Value);
        int detonations = Convert.ToInt32(customStats.Attributes?["Detonations"]?.Value);
        double bombCarryTime = Convert.ToDouble(customStats.Attributes?["Bomb Carry Time"]?.Value);
        int defuses = Convert.ToInt32(customStats.Attributes?["Defuses"]?.Value);

        return new Assault
        {
            BombsPlanted = bombsPlanted,
            Detonations = detonations,
            BombCarryTime = bombCarryTime,
            Defuses = defuses,
        };
    }
}