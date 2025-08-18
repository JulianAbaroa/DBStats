using System.Xml;

public class OddballTranslator
{
    public static Oddball Execute(XmlNode playerNode)
    {
        var customStats = playerNode.SelectSingleNode("CustomStats")
            ?? throw new NullReferenceException("CustomStats is null.");

        double carryTime = Convert.ToDouble(customStats.Attributes?["CARRY TIME"]?.Value);
        int ballKills = Convert.ToInt32(customStats.Attributes?["CARRY TIME"]?.Value);

        return new Oddball
        {
            CarryTime = carryTime,
            BallKills = ballKills,
        };
    }
}