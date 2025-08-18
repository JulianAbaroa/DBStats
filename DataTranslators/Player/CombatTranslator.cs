using System.Xml;

namespace DBStats.DataTranslators.Player;

public class CombatTranslator
{
    public static Combat Execute(XmlNode player, double minutesAlive)
    {
        int kills = Convert.ToInt32(player.Attributes!["mKills"]?.Value!);
        int deaths = Convert.ToInt32(player.Attributes!["mDeaths"]?.Value!);
        int assists = Convert.ToInt32(player.Attributes!["mAssists"]?.Value!);
        int consecutiveKills = Convert.ToInt32(player.Attributes!["mMostKillsInARow"]?.Value!);

        (int totalInvolvements, double kdRatio, double kdaRatio) = CalculateMetrics
        (
            kills,
            deaths,
            assists
        );

        (double killsPerMinute, double deathsPerMinute, double involvementsPerMinute) = CalculatePerMinute
        (
            kills,
            deaths,
            totalInvolvements,
            minutesAlive
        );

        return new Combat
        {
            Kills = kills,
            KillsPerMinute = killsPerMinute,
            Deaths = deaths,
            DeathsPerMinute = deathsPerMinute,
            Assists = assists,
            Involvements = totalInvolvements,
            InvolvementsPerMinute = involvementsPerMinute,
            ConsecutiveKills = consecutiveKills,
            KDRatio = kdRatio,
            KDARatio = kdaRatio,
        };
    }

    private static (int totalInvolvements, double kdRatio, double kdaRatio) CalculateMetrics
    (int kills,
    int deaths,
    int assists)
    {
        int totalInvolvements = kills + deaths + assists;
        double kdRatio = deaths > 0.0d ? (double)kills / deaths : 0.0d;
        double kdaRatio = deaths > 0.0d ? (double)(kills + assists) / deaths : 0.0d;
        return (totalInvolvements, kdRatio, kdaRatio);
    }

    private static (double killsPerMinute, double deathsPerMinute, double involvementsPerMinute) CalculatePerMinute
    (
        int kills,
        int deaths,
        int totalInvolvements,
        double aliveMinutes)
    {
        double killsPerMinute = aliveMinutes > 0.0d ? kills / aliveMinutes : 0.0d;
        double deathsPerMinute = aliveMinutes > 0.0d ? deaths / aliveMinutes : 0.0d;
        double involvementsPerMinute = aliveMinutes > 0.0d ? totalInvolvements / aliveMinutes : 0.0d;
        return (killsPerMinute, deathsPerMinute, involvementsPerMinute);
    }

}