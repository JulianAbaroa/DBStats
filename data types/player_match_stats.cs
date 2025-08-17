
namespace DBStats.DataTypes;

public class PlayerMatchStats
{
    public Combat Combat { get; set; }
    public Breakdown Breakdown { get; set; }
    public Rivalries Rivalries { get; set; }
    public Survivability Survivability { get; set; }
    public Choice Choice { get; set; }
    public required Medals Medals { get; set; }
    public Penalties Penalties { get; set; }
    public double Rating { get; set; }
    public required string Team { get; set; }
    public required object GameMode { get; set; }
}