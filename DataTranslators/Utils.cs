namespace DBStats.DataTranslators;

public class Utils
{
    /// <summary>
    /// Only can transform string minutes in the format: 'MM:SS'.
    /// </summary>
    /// <param name="minutes"></param>
    /// <returns></returns>
    public static double GetMinutesFromString(string minutes)
    {
        var parts = minutes.Split(':');
        var timeSpan = TimeSpan.FromMinutes(int.Parse(parts[0])) + TimeSpan.FromSeconds(int.Parse(parts[1]));
        return timeSpan.TotalSeconds / 60.0d;
    }
}