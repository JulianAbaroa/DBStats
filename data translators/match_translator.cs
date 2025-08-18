using System.Xml;

namespace DBStats.DataTypes;

public class MatchTranslator
{
    private static readonly Dictionary<GameType, List<string>> _gameTypeStats = new()
    {
        // Slayer:
        { GameType.Slayer, new List<string> { "RATING" } },

        // Capture The Flag:
        { GameType.CaptureTheFlag, new List<string> { "Flag Captures", "Flag Carry Time", "Flag Returns" } },

        // Oddball:
        { GameType.Oddball, new List<string> { "CARRY TIME", "BALL KILLS" } },

        // KingOfTheHill:
        { GameType.KingOfTheHill, new List<string> { "Time in Hill" } },

        // Juggernaut:
        { GameType.Juggernaut, new List<string> { "Juggernaut Time" } },

        // Infection:
        { GameType.Infection, new List<string> { "Survival Time", "Infections" } },

        // Territories:
        { GameType.Territories, new List<string> { "Captures" } },

        // Assault:
        { GameType.Assault, new List<string> { "Bombs Planted", "Detonations", "Bomb Carry Time", "Defuses" } },

        // Stockpile:
        { GameType.Stockpile, new List<string> { "CARRY TIME" } },

        // HeadHunter:
        { GameType.HeadHunter, new List<string> { "MAX SKULLS" } },

        // ActionSack:
        { GameType.ActionSack, new List<string> { "" } },
    };

    public static Match Excute(XmlNode carnageReport, XmlNode player, string matchID, string carnageReportPath)
    {
        var customStats = player.SelectSingleNode("CustomStats")!;

        var gameType = DetectGameType(customStats);
        string gameTypeName = carnageReport.SelectSingleNode("GameTypeName")?.Value!;

        bool isMatchmaking = Convert.ToBoolean(carnageReport.SelectSingleNode("IsMatchmaking")?.Value);

        bool wasMatchIncomplete = Convert.ToBoolean(carnageReport.SelectSingleNode("mLastMatchIncomplete")?.Value);
        bool isTeamsEnabled = Convert.ToBoolean(carnageReport.SelectSingleNode("IsTeamsEnabled")?.Value);

        return new Match
        {
            GameType = gameType,
            GameTypeName = gameTypeName,
            MatchID = matchID,
            IsMatchmaking = isMatchmaking,
            WasMatchIncomplete = wasMatchIncomplete,
            IsTeamsEnabled = isTeamsEnabled,
            Duration = 0.0,
            CarnagePath = carnageReportPath,
            Teams = [],
            Players = [],
        };
    }

    public static GameType DetectGameType(XmlNode customStats)
    {
        var statsNames = new HashSet<string>(
            customStats.SelectNodes("CustomStat")
                       ?.Cast<XmlNode>()
                       .Select(stat => stat.Attributes?["mStatName"]?.Value ?? "")
                       .Where(name => !string.IsNullOrWhiteSpace(name))
                       ?? Enumerable.Empty<string>(),
            StringComparer.OrdinalIgnoreCase
        );

        foreach (var kv in _gameTypeStats)
        {
            var gameType = kv.Key;
            var requiredStats = kv.Value;

            if (requiredStats.All(statsNames.Contains))
            {
                return gameType;
            }
        }

        int emptyStats = customStats.SelectNodes("CustomStat")
                                    ?.Cast<XmlNode>()
                                    .Count(stat => string.IsNullOrWhiteSpace(stat.Attributes?["mStatName"]?.Value)) ?? 0;

        if (emptyStats >= 4)
        {
            return GameType.Stockpile;
        }

        return GameType.Unknown;
    }

}