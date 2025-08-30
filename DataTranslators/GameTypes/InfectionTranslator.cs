using DBStats.DataTypes.GameTypes;
using System.Xml;

namespace DBStats.DataTranslators.GameTypes;

public class InfectionTranslator
{
    public static Infection Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
                    ?? throw new NullReferenceException("CustomStats is null.");

        var survivalTimeNode = customStats.SelectSingleNode("CustomStat[@mStatName='Survival Time']")
            ?? throw new NullReferenceException("survivalTimeNode is null.");

        string? survivalTimeString = survivalTimeNode.Attributes?["mValueForDisplay"]?.Value
            ?? throw new NullReferenceException("survivalTimeString is null.");

        double survivalTime = Utils.GetMinutesFromString(survivalTimeString);

        var infectionsNode = customStats.SelectSingleNode("CustomStat[@mStatName='Infections']")
            ?? throw new NullReferenceException("infectionsNode is null.");

        int infections = Convert.ToInt32(infectionsNode.Attributes?["mValueForDisplay"]?.Value);

        return new Infection
        {
            SurvivalTime = survivalTime,
            Infections = infections,
        };
    }
}