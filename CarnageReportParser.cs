using DBStats.DataTypes;
using DBStats.DataTypes.Enums;
using DBStats.DataTypes.GameTypes;
using DBStats.DataTranslators.GameTypes;
using DBStats.DataTranslators.Profile;
using DBStats.DataTranslators.Player;
using DBStats.DataTranslators;
using System.Xml;

namespace DBStats;

public class CarnageReportParser
{
    public static Match ParseMatch(string filePath)
    {
        var carnageReport = new XmlDocument();
        carnageReport.Load(filePath);

        XmlNode playerNodes = carnageReport.SelectSingleNode("/MultiplayerCarnageReport/Players")
            ?? throw new NullReferenceException("Error: The 'Players' node was not found in the XML.");

        XmlNode firstPlayer = playerNodes.FirstChild
            ?? throw new NullReferenceException("Error: First player wasn't found.");

        string matchID = Guid.NewGuid().ToString();

        var match = MatchTranslator.Excute(carnageReport, firstPlayer, matchID, filePath);
        var profiles = GetProfiles(playerNodes, match);
        var players = GetPlayers(playerNodes, match);
        var teams = GetTeams(players, match);

        match.Duration = players.Max(p => p.Survivability.MinutesPlayed);
        match.Teams = teams;

        return match;
    }

    private static List<PlayerMatchStats> GetPlayers(XmlNode playersNode, Match match)
    {
        var players = new List<PlayerMatchStats>();

        foreach (XmlNode playerNode in playersNode)
        {
            var survivability = SurvivabilityTranslator.Execute(playerNode);
            var combat = CombatTranslator.Execute(playerNode, survivability.MinutesAlive);
            var breakdown = BreakdownTranslator.Execute(playerNode, combat.Kills, combat.Deaths, combat.Assists, survivability.MinutesAlive);
            var rivalries = RivalriesTranslator.Execute(playerNode, combat.Kills, combat.Deaths);
            var choice = ChoiceTranslator.Execute(playerNode, breakdown.WeaponKills);
            var medals = MedalsTranslator.Execute(playerNode, combat.Kills, survivability.MinutesAlive);
            var penalties = PenaltiesTranslator.Execute(playerNode, combat.Kills, combat.Deaths);
            double rating = PlayerRating.GetRating(combat, breakdown, medals, survivability, penalties);
            string team = PlayerTeam.GetPlayerTeam(playerNode);
            object? gameMode = null;

            switch (match.GameType)
            {
                case GameType.Slayer:
                    gameMode = SlayerTranslator.Execute(playerNode);
                    break;
                case GameType.CaptureTheFlag:
                    gameMode = CTFTranslator.Execute(playerNode);
                    break;
                case GameType.Oddball:
                    gameMode = OddballTranslator.Execute(playerNode);
                    break;
                case GameType.KingOfTheHill:
                    gameMode = KOTHTranslator.Execute(playerNode);
                    break;
                case GameType.Juggernaut:
                    gameMode = JuggernautTranslator.Execute(playerNode);
                    break;
                case GameType.Infection:
                    gameMode = InfectionTranslator.Execute(playerNode);
                    break;
                case GameType.Territories:
                    gameMode = TerritoriesTranslator.Execute(playerNode);
                    break;
                case GameType.Assault:
                    gameMode = AssaultTranslator.Execute(playerNode);
                    break;
                case GameType.Stockpile:
                    gameMode = StockpileTranslator.Execute(playerNode);
                    break;
                case GameType.HeadHunter:
                    gameMode = HeadHunterTranslator.Execute(playerNode);
                    break;
                case GameType.ActionSack:
                    gameMode = new ActionSack();
                    break;
                case GameType.Unknown:
                    break;
            }

            gameMode ??= GameType.Unknown;

            var player = new PlayerMatchStats
            {
                Combat = combat,
                Breakdown = breakdown,
                Rivalries = rivalries,
                Survivability = survivability,
                Choice = choice,
                Medals = medals,
                Penalties = penalties,
                Rating = rating,
                Team = team,
                GameMode = gameMode,
            };

            players.Add(player);
        }

        return players;
    }

    private static List<Team> GetTeams(List<PlayerMatchStats> players, Match match)
    {
        var teams = new Dictionary<string, Team>();

        foreach (var player in players)
        {
            if (!match.IsTeamsEnabled)
            {
                player.Team = "FFA";
            }

            if (!teams.TryGetValue(player.Team, out var team))
            {
                team = new Team
                {
                    Color = player.Team,
                    Kills = 0,
                    Deaths = 0,
                    Players = [],
                };

                teams[player.Team] = team;
            }

            team.Players.Add(player);
            team.Kills += player.Combat.Kills;
            team.Deaths += player.Combat.Deaths;
        }

        foreach (var team in teams.Values)
        {
            team.Rating = team.Players.Average(p => p.Rating);
        }

        return teams.Values.ToList();
    }

    private static List<PlayerProfile> GetProfiles(XmlNode playerNodes, Match match)
    {
        var profiles = new List<PlayerProfile>();

        foreach (XmlNode playerNode in playerNodes)
        {
            var profile = new PlayerProfile
            {
                PlayerID = playerNode.Attributes?["mXboxUserId"]?.Value!,
                PlayerName = playerNode.Attributes?["mGamertagText"]?.Value!,
                Customization = CustomizationTranslator.Execute(playerNode),
                LastSeen = DateTime.UtcNow,
            };

            if (profile.PlayerID == null)
            {
                Console.WriteLine("Error: PlayerID not found.");
            }
            else if (profile.PlayerName == null)
            {
                Console.WriteLine("Error: PlayerName not found.");
            }

            profiles.Add(profile);
        }

        return profiles;
    }

}