using DBStats.DataTypes.Enums;

namespace DBStats.DataTypes;

public class Match
{
    public required string MatchID { get; set; }
    public GameType GameType { get; set; }
    public required string GameTypeName { get; set; }
    public bool IsMatchmaking { get; set; }
    public bool WasMatchIncomplete { get; set; }
    public bool IsTeamsEnabled { get; set; }
    public double Duration { get; set; }
    public required string CarnagePath { get; set; }
    public DateTime MatchTimestamp { get; set; }

    public required List<Team> Teams { get; set; }
}