using DBStats.Interfaces;

namespace DBStats.DataTypes;

public class Team
{
    public required string Result { get; set; }
    public required string Color { get; set; }
    public IGameMode? GameMode { get; set; }
    public double Rating { get; set; }
    public int Deaths { get; set; }
    public int Kills { get; set; }

    public List<PlayerMatchStats> Players { get; set; } = [];
}