
namespace DBStats.DataTypes;

public class Team
{
    public List<PlayerMatchStats> Players { get; set; } = [];
    public required PlayerMatchStats MostValuablePlayer { get; set; }
    public required object GameMode { get; set; }
    public required string Color { get; set; }
    public double Rating { get; set; }
    public int Deaths { get; set; }
    public int Kills { get; set; }
}