using System.Xml;

public class InfectionTranslator
{
    public static Infection Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
                    ?? throw new NullReferenceException("CustomStats is null.");

        double survivalTime = Convert.ToDouble(customStats.Attributes?["Survival Time"]?.Value);
        int infections = Convert.ToInt32(customStats.Attributes?["Infections"]?.Value);

        return new Infection
        {
            SurvivalTime = survivalTime,
            Infections = infections,
        };
    }
}