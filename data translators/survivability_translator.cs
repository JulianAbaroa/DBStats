using System.Xml;

public class SurvivabilityTranslator
{
    public Survivability Execute(XmlNode player)
    {
        double secondsAlive = Convert.ToDouble(player.Attributes!["mSecondsAlive"]?.Value!);
        double secondsPlayed = Convert.ToDouble(player.Attributes!["mSecondsPlayed"]?.Value!);

        double aliveMinutes = secondsAlive / 60.0d;
        double playedMinutes = secondsPlayed / 60.0d;
        double aliveTimeRatio = secondsPlayed > 0 ? secondsAlive / secondsPlayed : 0.0d;

        return new Survivability
        {
            MinutesAlive = aliveMinutes,
            MinutesPLayed = playedMinutes,
            AliveTimeRatio = aliveTimeRatio,
        };
    }

}