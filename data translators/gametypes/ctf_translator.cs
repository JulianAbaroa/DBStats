using System.Xml;

public class CTFTranslator
{
    public static CaptureTheFlag Execute(XmlNode player)
    {
        var customStats = player.SelectSingleNode("CustomStats");

        int flagCaptures = GetFlagCaptures(customStats!);
        int flagRecovers = GetFlagRecovers(customStats!);
        double flagCarryTime = GetFlagCarryTime(customStats!);

        return new CaptureTheFlag
        {
            FlagCaptures = flagCaptures,
            FlagRecovers = flagRecovers,
            FlagCarryTime = flagCarryTime,
        };
    }

    private static int GetFlagCaptures(XmlNode customStats)
    {
        var flagCapturesNode = customStats.SelectSingleNode("CustomStat[@mStatName='Flag Captures']");
        string flagCapturesString = flagCapturesNode?.Attributes!["mValueForDisplay"]?.Value!;
        return Convert.ToInt32(flagCapturesString);
    }

    private static int GetFlagRecovers(XmlNode customStats)
    {
        var flagRecoversNode = customStats.SelectSingleNode("CustomStat[@mStatName='Flag Returns']");
        string flagRecoversString = flagRecoversNode?.Attributes!["mValueForDisplay"]?.Value!;
        return Convert.ToInt32(flagRecoversString);
    }

    private static double GetFlagCarryTime(XmlNode customStats)
    {
        var flagCarryTimeNode = customStats.SelectSingleNode("CustomStat[@mStatName='Flag Carry Time']");
        string flagCarryTimeString = flagCarryTimeNode?.Attributes!["mValueForDisplay"]?.Value!;
        return GetMinutesFromString(flagCarryTimeString);
    }

    /// <summary>
    /// Only can transform string minutes in the format: 'MM:SS'.
    /// </summary>
    /// <param name="minutes"></param>
    /// <returns></returns>
    private static double GetMinutesFromString(string minutes)
    {
        var parts = minutes.Split(':');
        var timeSpan = TimeSpan.FromMinutes(int.Parse(parts[0])) + TimeSpan.FromSeconds(int.Parse(parts[1]));
        return timeSpan.TotalSeconds / 60.0d;
    }

}