using Microsoft.Data.Sqlite;
using DBStats.DataTypes;
using System.Xml;

// TODO: FILTRADO DE ARCHIVOS DUPLICADOS.

namespace DBStats;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        string filePath = args[0];

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: El archivo no se encuentra en la ruta: {filePath}");
        }

        var carnageReport = new XmlDocument();
        carnageReport.Load(filePath);

        XmlNode? playerNodes = carnageReport.SelectSingleNode("/MultiplayerCarnageReport/Players");

        if (playerNodes == null)
        {
            Console.WriteLine("Error: The 'Players' node was not found in the XML.");
            return;
        }

        string matchID = Guid.NewGuid().ToString();
        XmlNode? firstPlayer = playerNodes.FirstChild;

        if (firstPlayer == null)
        {
            Console.WriteLine("Error: First player wasn't found.");
            return;
        }

        var match = MatchTranslator.Excute(carnageReport, firstPlayer, matchID, filePath);
        var profiles = GetProfiles(playerNodes, match);
        var players = GetPlayers(playerNodes, match);
        var teams = GetTeams(players, match);

        match.Duration = players.Max(p => p.Survivability.MinutesPlayed);
        match.Players = players;
        match.Teams = teams;

        string folderPath = @"C:\Users\maste\OneDrive\Documents\Halo\DBStats DataBase";
        Directory.CreateDirectory(folderPath);
        string connectionString = $"Data Source={Path.Combine(folderPath, "dbstats.db")}";

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var createMatchesTable = @"CREATE TABLE IF NOT EXISTS Matches (
            match_id TEXT PRIMARY KEY,
            gametype INTEGER NOT NULL,
            gametype_name TEXT NOT NULL,
            is_matchmaking INTEGER NOT NULL,
            was_match_incomplete INTEGER NOT NULL,
            is_teams_enabled INTEGER NOT NULL,
            duration REAL NOT NULL,
            carnage_path TEXT NOT NULL,
        );";

        var createTeamsTable = @"CREATE TABLE IF NOT EXISTS Teams (
            TeamID INTEGER PRIMARY KEY AUTOINCREMENT,
            match_id TEXT NOT NULL,
            color TEXT NOT NULL,
            rating TEXT NOT NULL,
            deaths INTEGER NOT NULL,
            kills INTEGER NOT NULL,
            most_valuable_player_id INTEGER NOT NULL,
            FOREING KEY(match_id) REFERENCES Matches(match_id)
        )";

        using var cmd = new SqliteCommand(createMatchesTable, connection);
        cmd.ExecuteNonQuery();
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
                    gameMode = KingOfTheHillTranslator.Execute(playerNode);
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
        if (!match.IsTeamsEnabled)
        {
            return [];
        }

        var teams = new Dictionary<string, Team>();

        foreach (var player in players)
        {
            if (!teams.TryGetValue(player.Team, out var team))
            {
                team = new Team
                {
                    Color = player.Team,
                    GameMode = player.GameMode,
                    Kills = 0,
                    Deaths = 0,
                    Players = [],
                    MostValuablePlayer = player,
                };

                teams[player.Team] = team;
            }

            team.Players.Add(player);
            team.Kills += player.Combat.Kills;
            team.Deaths += player.Combat.Deaths;

            if (team.MostValuablePlayer == null || player.Rating > team.MostValuablePlayer.Rating)
            {
                team.MostValuablePlayer = player;
            }
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
                MatchIDsDescending = [],
            };

            if (profile.PlayerID == null)
            {
                Console.WriteLine("Error: PlayerID not found.");
            }
            else if (profile.PlayerName == null)
            {
                Console.WriteLine("Error: PlayerName not found.");
            }

            profile.MatchIDsDescending.Add(match.MatchID);

            profiles.Add(profile);
        }

        return profiles;
    }

}