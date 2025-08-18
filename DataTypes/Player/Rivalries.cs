
namespace DBStats.DataTypes.Player;

public struct Rivalries
{
    public string MostKilledPlayer { get; set; }
    public int MostKilledCount { get; set; }
    public double MostKilledKillRatio { get; set; }

    public string MostKillerPlayer { get; set; }
    public int MostKillerCount { get; set; }
    public double MostKillerDeathRatio { get; set; }
}