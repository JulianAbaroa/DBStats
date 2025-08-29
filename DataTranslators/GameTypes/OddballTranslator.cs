using DBStats.DataTypes.GameTypes;
using System.Xml;

namespace DBStats.DataTranslators.GameTypes;

public class OddballTranslator
{
    public static Oddball Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
            ?? throw new NullReferenceException("CustomStats is null.");

        var carryTimeNode = customStats.SelectSingleNode("CustomStat[@mStatName='CARRY TIME']")
            ?? throw new NullReferenceException("carryTimeNode is null.");

        double carryTime = Convert.ToDouble(carryTimeNode.Attributes?["mValueForDisplay"]?.Value);

        var ballKillsNode = customStats.SelectSingleNode("CustomStat[@mStatName='BALL KILLS']")
            ?? throw new NullReferenceException("ballKillsNode is null.");

        int ballKills = Convert.ToInt32(ballKillsNode.Attributes?["mValueForDisplay"]?.Value);

        return new Oddball
        {
            CarryTime = carryTime,
            BallKills = ballKills,
        };
    }
}