
namespace DBStats.DataTypes;

public class Match
{
    public GameType GameType { get; set; }
    public required string GameTypeName { get; set; }
    public required string MatchID { get; set; }
    public bool IsMatchmaking { get; set; }
    public bool WasMatchIncomplete { get; set; }
    public bool IsTeamsEnabled { get; set; }
    public double Duration { get; set; }
    public required string CarnageReportPath { get; set; }

    public required List<Team> Teams { get; set; }
    public required List<PlayerMatchStats> Players { get; set; }
}