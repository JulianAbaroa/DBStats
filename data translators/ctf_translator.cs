using System.Xml;

public class CTFTranslator
{
    public CTF Execute(XmlNode player, XmlNodeList players, int kills)
    {
        var customStats = player.SelectSingleNode("CustomStats");

        string playerTeam = GetPlayerTeamGroup(player);

        (int totalKills, int redTeamKills, int blueTeamKills) = CalculateKills(players);

        (double totalParticipation, double totalTeamParticipation) = CalculateParticipations
        (
            kills,
            totalKills,
            playerTeam == "First" ? redTeamKills : blueTeamKills
        );

        int flagCaptures = GetFlagCaptures(customStats!);
        int flagRecovers = GetFlagRecovers(customStats!);
        double flagCarryTime = GetFlagCarryTime(customStats!);

        return new CTF
        {
            PlayerTeam = playerTeam,
            TotalParticipation = totalParticipation,
            TotalTeamParticipation = totalTeamParticipation,
            FlagCaptures = flagCaptures,
            FlagRecovers = flagRecovers,
            FlagCarryTime = flagCarryTime,
            TotalKills = totalKills,
            RedTeamKills = redTeamKills,
            BlueTeamKills = blueTeamKills,
        };
    }

    private string GetPlayerTeamGroup(XmlNode player)
    {
        int player_team = Convert.ToInt32(player.Attributes!["mTeamId"]?.Value!);

        return player_team switch
        {
            0 => "Red",
            1 => "Blue",
            2 => "Green",
            3 => "Orange",
            4 => "Purple",
            5 => "Gold",
            6 => "Brown",
            7 => "Pink",
            _ => "Unknown",
        };
    }

    private (int totalKills, int redTeamKills, int blueTeamKills) CalculateKills(XmlNodeList players)
    {
        int totalKills = 0;
        int redTeamKills = 0;
        int blueTeamKills = 0;

        foreach (XmlNode player in players)
        {
            var name = player.Attributes!["mGamertagText"]?.Value!;
            var team = Convert.ToInt32(player.Attributes!["mTeamId"]?.Value!);
            var kills = Convert.ToInt32(player.Attributes!["mKills"]?.Value!);

            totalKills += kills;

            switch (team)
            {
                case 0:
                    redTeamKills += kills;
                    break;
                case 1:
                    blueTeamKills += kills;
                    break;
                default:
                    Console.WriteLine("Undefined team number.");
                    break;
            }
        }

        return (totalKills, redTeamKills, blueTeamKills);
    }

    private (double totalParticipation, double totalTeamParticipation) CalculateParticipations
    (
        int kills,
        int totalKills,
        int totalTeamKills)
    {
        double totalParticipation = totalKills > 0.0d ? (double)kills / totalKills : 0.0d;
        double totalGroupParticipation = totalTeamKills > 0.0d ? (double)kills / totalTeamKills : 0.0d;

        return (
            totalParticipation,
            totalGroupParticipation
        );
    }

    private int GetFlagCaptures(XmlNode customStats)
    {
        var flagCapturesNode = customStats.SelectSingleNode("CustomStat[@mStatName='Flag Captures']");
        string flagCapturesString = flagCapturesNode?.Attributes!["mValueForDisplay"]?.Value!;
        return Convert.ToInt32(flagCapturesString);
    }

    private int GetFlagRecovers(XmlNode customStats)
    {
        var flagRecoversNode = customStats.SelectSingleNode("CustomStat[@mStatName='Flag Returns']");
        string flagRecoversString = flagRecoversNode?.Attributes!["mValueForDisplay"]?.Value!;
        return Convert.ToInt32(flagRecoversString);
    }

    private double GetFlagCarryTime(XmlNode customStats)
    {
        var flagCarryTimeNode = customStats.SelectSingleNode("CustomStat[@mStatName='Flag Carry Time']");
        string flagCarryTimeString = flagCarryTimeNode?.Attributes!["mValueForDisplay"]?.Value!;
        return GetMinutesFromString(flagCarryTimeString);
    }

    /// <summary>
    /// Only can transform string minutes in the format: 'MM:SS'.
    /// </summary>
    /// <param name="minutes"></param>
    /// <returns></returns>
    private double GetMinutesFromString(string minutes)
    {
        var parts = minutes.Split(':');
        var timeSpan = TimeSpan.FromMinutes(int.Parse(parts[0])) + TimeSpan.FromSeconds(int.Parse(parts[1]));
        return timeSpan.TotalSeconds / 60.0d;
    }

}