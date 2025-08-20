using DBStats.DataTypes.Player;
using System.Xml;

namespace DBStats.DataTranslators.Player;

public class BreakdownTranslator
{
    public static Breakdown Execute(XmlNode player, int kills, int deaths, int assists, double minutesAlive)
    {
        int weaponKills = Convert.ToInt32(player.Attributes!["mKillsWeapon"]?.Value!);
        int grenadeKills = Convert.ToInt32(player.Attributes!["mKillsGrenade"]?.Value!);
        int meleeKills = Convert.ToInt32(player.Attributes!["mKillsMelee"]?.Value!);
        int otherKills = Convert.ToInt32(player.Attributes!["mKillsOther"]?.Value!);

        (double weaponKillsRatio, double grenadeKillsRatio, double meleeKillsRatio, double otherKillsRatio) = CalculateWeaponRatios(
            kills,
            weaponKills,
            grenadeKills,
            meleeKills,
            otherKills
        );

        double killsSuccessRatio = CalculateCombatRatio(
            kills,
            deaths
        );

        return new Breakdown
        {
            WeaponKills = weaponKills,
            GrenadeKills = grenadeKills,
            MeleeKills = meleeKills,
            OtherKills = otherKills,

            WeaponKillsRatio = weaponKillsRatio,
            GrenadeKillsRatio = grenadeKillsRatio,
            MeleeKillsRatio = meleeKillsRatio,
            OtherKillsRatio = otherKillsRatio,

            KillSuccessRatio = killsSuccessRatio,
        };
    }

    private static (double weaponKillsRatio, double grenadeKillsRatio, double meleeKillsRatio, double otherKillsRatio) CalculateWeaponRatios(
        int kills,
        int weaponKills,
        int grenadeKills,
        int meleeKills,
        int otherKills)
    {
        double weaponKillsRatio = kills > 0.0d ? (double)weaponKills / kills : 0.0d;
        double grenadeKillsRatio = kills > 0.0d ? (double)grenadeKills / kills : 0.0d;
        double meleeKillsRatio = kills > 0.0d ? (double)meleeKills / kills : 0.0d;
        double otherKillsRatio = kills > 0.0d ? (double)otherKills / kills : 0.0d;

        return (
            weaponKillsRatio,
            grenadeKillsRatio,
            meleeKillsRatio,
            otherKillsRatio
        );
    }

    private static double CalculateCombatRatio
    (
        int kills,
        int deaths
    )
    {
        double killsSuccessRatio = (kills + deaths) > 0.0d ? (double)kills / (kills + deaths) : 0.0d;

        return (
            killsSuccessRatio
        );
    }

}