using DBStats.DataTypes.Dictionaries;
using DBStats.DataTypes.Enums;
using DBStats.DataTypes;
using System.Xml;

namespace DBStats.DataTranslators;

public class MatchTranslator
{
    public static Match Excute(XmlNode carnageReport, XmlNode player, string matchID, string carnageReportPath)
    {
        var customStats = player.SelectSingleNode("CustomStats")!;

        var gameType = DetectGameType(customStats);

        var gameTypeNode = carnageReport.SelectSingleNode("//GameTypeName");
        string gameTypeName = gameTypeNode?.Attributes?["GameTypeName"]?.Value
            ?? throw new NullReferenceException("Error: GameTypeName is null.");

        var isMatchmakingNode = carnageReport.SelectSingleNode("//IsMatchmaking");
        string isMatchmaking = isMatchmakingNode?.Attributes?["IsMatchmaking"]?.Value
            ?? throw new NullReferenceException("Error: IsMatchmaking is null.");

        var wasMatchIncompleteNode = carnageReport.SelectSingleNode("//mLastMatchIncomplete");
        string wasMatchIncomplete = wasMatchIncompleteNode?.Attributes?["mLastMatchIncomplete"]?.Value
            ?? throw new NullReferenceException("Error: WasMatchIncomplete is null.");

        var isTeamsEnabledNode = carnageReport.SelectSingleNode("//IsTeamsEnabled");
        string isTeamsEnabled = isTeamsEnabledNode?.Attributes?["IsTeamsEnabled"]?.Value
            ?? throw new NullReferenceException("Error: IsTeamsEnabled is null.");

        var matchTimeStamp = DateTime.UtcNow;

        return new Match
        {
            GameType = gameType,
            GameTypeName = gameTypeName,
            MatchID = matchID,
            IsMatchmaking = Convert.ToBoolean(isMatchmaking),
            WasMatchIncomplete = Convert.ToBoolean(wasMatchIncomplete),
            IsTeamsEnabled = Convert.ToBoolean(isTeamsEnabled),
            Duration = 0.0,
            CarnagePath = carnageReportPath,
            MatchTimestamp = matchTimeStamp,
            Teams = [],
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

        foreach (var kv in CustomStats.GameTypeStats)
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