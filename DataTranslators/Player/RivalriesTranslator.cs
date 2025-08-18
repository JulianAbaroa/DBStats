using System.Xml;

namespace DBStats.DataTranslators.Player;

public class RivalriesTranslator
{
    public static Rivalries Execute(XmlNode player, int kills, int deaths)
    {
        int mostKilledID = Convert.ToInt32(player.Attributes!["mKilledMostPlayerIndex"]?.Value!);
        int mostKilledCount = Convert.ToInt32(player.Attributes!["mKilledMostPlayerCount"]?.Value!);

        int mostKillerID = Convert.ToInt32(player.Attributes!["mMostKilledByPlayerIndex"]?.Value!);
        int mostKillerCount = Convert.ToInt32(player.Attributes!["mMostKilledByPlayerCount"]?.Value!);

        (string mostKilledName, string mostKillerName) = GetNames(
            player,
            mostKilledID,
            mostKillerID
        );

        (double mostKilledKillRatio, double mostKillerDeathRatio) = CalculateRatios(
            kills,
            deaths,
            kills,
            mostKillerCount
        );

        return new Rivalries
        {
            MostKilledPlayer = mostKilledName,
            MostKilledCount = mostKilledCount,
            MostKilledKillRatio = mostKilledKillRatio,

            MostKillerPlayer = mostKillerName,
            MostKillerCount = mostKillerCount,
            MostKillerDeathRatio = mostKillerDeathRatio,
        };
    }

    private static (string mostKilledName, string mostKillerName) GetNames(
        XmlNode player,
        int mostKilledID,
        int mostKillerID)
    {
        string mostKilledName = "Unknown";
        string mostKillerName = "Unknown";

        XmlNodeList allUsers = player.ParentNode!.SelectNodes("Player")!;

        if (mostKilledID >= 0 && mostKilledID < allUsers.Count)
        {
            mostKilledName = allUsers[mostKilledID]!.Attributes!["mGamertagText"]!.Value!;
        }

        if (mostKillerID >= 0 && mostKillerID < allUsers.Count)
        {
            mostKillerName = allUsers[mostKillerID]!.Attributes!["mGamertagText"]!.Value!;
        }

        return (
            mostKilledName,
            mostKillerName
        );
    }

    private static (double mostKilledKillRatio, double mostKillerDeathRatio) CalculateRatios(
        int kills,
        int deaths,
        int mostKilledCount,
        int mostKillerCount)
    {
        double mostKilledKillRatio = kills > 0 ? (double)mostKilledCount / kills : 0.0d;
        double mostKillerDeathRatio = deaths > 0 ? (double)mostKillerCount / deaths : 0.0d;

        return (
            mostKilledKillRatio,
            mostKillerDeathRatio
        );
    }

}