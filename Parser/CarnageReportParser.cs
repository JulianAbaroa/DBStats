using DBStats.DataTypes;
using DBStats.DataTypes.Enums;
using DBStats.DataTypes.GameTypes;
using DBStats.DataTranslators.GameTypes;
using DBStats.DataTranslators.Player;
using DBStats.DataTranslators;
using System.Xml;
using DBStats.Interfaces;

namespace DBStats.Parser;

public class CarnageReportParser
{
    public static Match ParseMatch(XmlDocument carnageReport, XmlNode playerNodes, string filePath)
    {
        XmlNode firstPlayer = playerNodes.FirstChild
            ?? throw new NullReferenceException("Error: First player wasn't found.");

        string matchID = Guid.NewGuid().ToString();

        var match = MatchTranslator.Excute(carnageReport, firstPlayer, matchID, filePath);
        var players = GetPlayers(playerNodes, match);
        var teams = GetTeams(players, match);
        AdjustPlayersRating(teams);
        SetTeamWinner(teams);

        match.Duration = players.Max(p => p.Survivability.MinutesPlayed);
        match.Teams = teams;

        return match;
    }

    private static List<PlayerMatchStats> GetPlayers(XmlNode playersNode, Match match)
    {
        var players = new List<PlayerMatchStats>();

        foreach (XmlNode playerNode in playersNode)
        {
            string playerID = playerNode.Attributes?["mXboxUserId"]?.Value!;
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
                PlayerID = playerID,
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
                // TODO: VER COMO REACCIONA ESTO.
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

                if (player.GameMode != null && team.GameMode == null)
                {
                    Type gamemodeType = player.GameMode.GetType();
                    team.GameMode = (IGameMode?)Activator.CreateInstance(gamemodeType);
                }
            }

            team.Players.Add(player);
            team.Kills += player.Combat.Kills;
            team.Deaths += player.Combat.Deaths;

            if (player.GameMode is IGameMode playerGameMode && team.GameMode is IGameMode teamGameMode)
            {
                teamGameMode.AddStats(playerGameMode);
            }
        }

        foreach (var team in teams.Values)
        {
            team.Rating = team.Players.Select(p => p.Rating).DefaultIfEmpty(0).Average();
        }

        return teams.Values.ToList();
    }

    private static void SetTeamWinner(List<Team> teams)
    {
        if (teams == null || teams.Count == 0)
        {
            throw new InvalidOperationException("Error: teams in null or empty");
        }

        // TODO: PROBABLEMENTE HAYA QUE DIFERENCIAR SEGUN QUE IGAMEMODES.
        var winningTeam = teams.OrderByDescending(t => t.GameMode?.GetScore()).FirstOrDefault()
            ?? throw new NullReferenceException("Error: winning team is null.");

        foreach (var team in teams)
        {
            if (team == winningTeam)
            {
                team.Winned = true;
            }
            else
            {
                team.Winned = false;
            }
        }
    }

    private static void AdjustPlayersRating(List<Team> teams)
    {
        if (teams == null || teams.Count == 0)
        {
            throw new InvalidOperationException("Error: teams in null or empty");
        }

        foreach (var currentTeam in teams)
        {
            double opponentAverageRating = teams
                .Where(t => t.Color != currentTeam.Color)
                .Select(t => t.Rating)
                .DefaultIfEmpty(0)
                .Average();

            double difficultyFactor = opponentAverageRating - currentTeam.Rating;

            foreach (var player in currentTeam.Players)
            {
                double ratingAdjustment = difficultyFactor * 0.25;
                player.Rating += ratingAdjustment;
            }
        }
    }

}