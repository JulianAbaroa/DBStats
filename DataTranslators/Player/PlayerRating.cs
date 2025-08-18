
namespace DBStats.DataTranslators.Player;

public class PlayerRating
{
    public static double GetRating(Combat combat, Breakdown breakdown, Medals medals, Survivability survivability, Penalties penalties)
    {
        double combatScore = 0.0;
        combatScore += combat.Kills * 10;
        combatScore += combat.Assists * 5;
        combatScore += combat.Involvements * 3;
        combatScore *= breakdown.KillSuccessRatio;
        combatScore *= breakdown.ContributionRatio;

        double medalScore = 0.0;

        foreach (var medal in medals.MedalsInfo.Types)
        {
            MedalScores.AwardPointValues.TryGetValue(medal.Key, out var value);
            medalScore += value;
        }

        medalScore *= medals.MedalsPerMinute;

        double survivalScore = survivability.AliveTimeRatio * 40;

        double penaltyScore = 0.0;

        penaltyScore += penalties.SuicidesPerDeath * 20;
        penaltyScore += penalties.BetrayalsPerKill * 25;

        return 1000 + combatScore + medalScore + survivalScore + penaltyScore;
    }
}