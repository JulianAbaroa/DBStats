using DBStats.DataTypes.Dictionaries;
using DBStats.DataTypes.Player;
using DBStats.Interfaces;
using SQLitePCL;

namespace DBStats.DataTranslators.Player;

public class PlayerRating
{
    public static double GetRating(Combat combat, Breakdown breakdown, Medals medals, Survivability survivability, Penalties penalties, IGameMode? gameMode)
    {
        const double MAX_TIME_MINUTES = 10.0;
        const double BASE = 1000.0;

        const double MIN_EFF = 0.5;
        const double MAX_EFF = 2.0;

        const double KILL_WEIGHT = 50.0;
        const double ASSIST_WEIGHT = 25.0;
        const double INVOLVEMENT_WEIGHT = 75.0;
        const double CONSECUTIVE_WEIGHT = 50.0;
        const double DEATH_PENALTY_WEIGHT = 25.0;

        const double WEAPON_KILL_WEIGHT = 25.0;
        const double GRENADE_KILL_WEIGHT = 35;
        const double MELEE_KILL_WEIGHT = 18;

        const double OBJECTIVE_WEIGHT = 120.0;

        double actualMinutes = Math.Max(1.0, survivability.MinutesPlayed);
        double cappedMinutes = Math.Min(actualMinutes, MAX_TIME_MINUTES);
        double timeScale = cappedMinutes / actualMinutes;

        double scaledKills = combat.Kills * timeScale;
        double scaledAssists = combat.Assists * timeScale;
        double scaledInvolvements = combat.Involvements * timeScale;
        double scaledDeaths = combat.Deaths * timeScale;

        double combatRaw =
            Math.Sqrt(scaledKills) * KILL_WEIGHT +
            Math.Sqrt(scaledAssists) * ASSIST_WEIGHT +
            Math.Sqrt(scaledInvolvements) * INVOLVEMENT_WEIGHT +
            Math.Pow(combat.ConsecutiveKills * timeScale, 1.15) * CONSECUTIVE_WEIGHT -
            Math.Sqrt(scaledDeaths) * DEATH_PENALTY_WEIGHT;

        double kdaClamp = Math.Clamp(combat.KDARatio, 0.5, 3.0);
        double efficiencyMultiplier = Math.Clamp(0.5 + 0.5 * kdaClamp, MIN_EFF, MAX_EFF);

        double combatScore = combatRaw * efficiencyMultiplier;

        double killSuccessClamp = Math.Clamp(breakdown.KillSuccessRatio, 0.0, 1.5);
        double krsFactor = 0.8 + 0.4 * Math.Clamp(killSuccessClamp / 1.5, 0.0, 1.0);

        combatScore *= krsFactor;

        double scaledWeapon = breakdown.WeaponKills * timeScale;
        double scaledGrenade = breakdown.GrenadeKills * timeScale;
        double scaledMelee = breakdown.MeleeKills * timeScale;
        double scaledOther = breakdown.OtherKills * timeScale;

        double breakdownScore =
            Math.Sqrt(scaledWeapon) * WEAPON_KILL_WEIGHT +
            Math.Sqrt(scaledGrenade) * GRENADE_KILL_WEIGHT +
            Math.Sqrt(scaledMelee) * MELEE_KILL_WEIGHT +
            Math.Sqrt(scaledOther) * WEAPON_KILL_WEIGHT;

        double ksrClamp = Math.Clamp(breakdown.KillSuccessRatio, 0.0, 1.5);
        breakdownScore *= 0.9 + 0.2 * ksrClamp;

        double baseMedalsScore = 0.0;
        foreach (var medal in medals.MedalsInfo.Types)
        {
            MedalScores.AwardPointValues.TryGetValue(medal.Key, out var value);
            baseMedalsScore += value * medal.Value;
        }

        double scaledBaseMedals = baseMedalsScore * timeScale;
        scaledBaseMedals = Math.Sqrt(scaledBaseMedals) * 10.0;

        double medalsEfficiency = Math.Clamp(medals.MedalsPerKill, 0.5, 3.0);
        double medalsFactor = 0.75 + (medals.MedalsPerMinute * 0.5);
        double medalsScore = scaledBaseMedals * medalsEfficiency * medalsFactor;

        double survivalScore = survivability.AliveTimeRatio * 40.0 * (cappedMinutes / MAX_TIME_MINUTES);
        double penaltyScore = penalties.Betrayals * 100.0;

        double modeScore = 0.0;
        if (gameMode != null)
        {
            try
            {
                double rawMode = gameMode.GetScore(survivability.MinutesPlayed);

                if (!double.IsFinite(rawMode) || rawMode <= 0.0)
                {
                    modeScore = Math.Sqrt(rawMode * timeScale) * OBJECTIVE_WEIGHT;

                    if (modeScore < 0.0)
                    {
                        modeScore = 0.0;
                    }
                }
            }
            catch
            {
                modeScore = 0.0;
            }
        }

        double extra = combatScore + breakdownScore + medalsScore + survivalScore - penaltyScore + modeScore;

        double kdaNorm = (kdaClamp - 0.5) / (3.0 - 0.5);
        double ksrNorm = ksrClamp / 1.5;
        double skillRaw = 0.6 * kdaNorm + 0.4 * ksrNorm;

        const double SKILL_EXPONENT = 2.2;
        const double SKILL_SCALE = 9.4;
        const double SKILL_BASE = 0.6;

        double skillFactor = SKILL_BASE + Math.Pow(skillRaw, SKILL_EXPONENT) * SKILL_SCALE;

        double scaledExtra = extra * skillFactor;
        double final = BASE + scaledExtra;

        if (double.IsNaN(final) || double.IsInfinity(final))
        {
            final = BASE;
        }
        if (final < BASE)
        {
            final = BASE;
        }

        return final;
    }

}