
namespace DBStats.DataTypes.Player;

public struct Breakdown
{
    public int WeaponKills { get; set; }
    public int GrenadeKills { get; set; }
    public int MeleeKills { get; set; }
    public int OtherKills { get; set; }

    public double WeaponKillsRatio { get; set; }
    public double GrenadeKillsRatio { get; set; }
    public double MeleeKillsRatio { get; set; }
    public double OtherKillsRatio { get; set; }

    public double KillSuccessRatio { get; set; }
}