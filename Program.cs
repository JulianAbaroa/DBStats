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
        match.Teams = teams;

        string folderPath = @"C:\Users\maste\OneDrive\Documents\Halo\DBStats DataBase";
        Directory.CreateDirectory(folderPath);
        string connectionString = $"Data Source={Path.Combine(folderPath, "dbstats.db")}";

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var createMatchesTable = @"CREATE TABLE IF NOT EXISTS Matches (
            match_id TEXT PRIMARY KEY NOT NULL,
            gametype INTEGER NOT NULL,
            gametype_name TEXT NOT NULL,
            is_matchmaking INTEGER NOT NULL,
            was_match_incomplete INTEGER NOT NULL,
            is_teams_enabled INTEGER NOT NULL,
            duration REAL NOT NULL,
            carnage_path TEXT NOT NULL
        );";

        var createTeamsTable = @"CREATE TABLE IF NOT EXISTS Teams (
            team_id INTEGER PRIMARY KEY AUTOINCREMENT,
            match_id TEXT NOT NULL,
            color TEXT NOT NULL,
            rating REAL NOT NULL,
            deaths INTEGER NOT NULL,
            kills INTEGER NOT NULL,
            FOREIGN KEY(match_id) REFERENCES Matches(match_id)
        )";

        var createProfilesTable = @"CREATE TABLE IF NOT EXISTS Profiles (
            player_id TEXT PRIMARY KEY NOT NULL,
            player_name TEXT NOT NULL,
            last_seen DATETIME            
        )";

        var createCustomizationTable = @"CREATE TABLE IF NOT EXISTS Customization (
            player_id TEXT NOT NULL,
            service_id TEXT NOT NULL,
            clan_tag TEXT NOT NULL,
            nameplate INTEGER NOT NULL,
            emblem_texture_zero INTEGER NOT NULL,
            emblem_texture_one INTEGER NOT NULL,
            emblem_color_zero INTEGER NOT NULL,
            emblem_color_one INTEGER NOT NULL,
            emblem_color_two INTEGER NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Profiles(player_id)
        )";

        var createPlayersTable = @"CREATE TABLE IF NOT EXISTS Players (
            player_id TEXT NOT NULL,
            team_id INTEGER NOT NULL,
            rating REAL NOT NULL,
            PRIMARY KEY (player_id, team_id),
            FOREIGN KEY(player_id) REFERENCES Profiles(player_id),
            FOREIGN KEY(team_id) REFERENCES Teams(team_id)
        )";

        var createCombatTable = @"CREATE TABLE IF NOT EXISTS Combat (
            combat_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            kills INTEGER NOT NULL,
            kills_per_minute REAL NOT NULL,
            deaths INTEGER NOT NULL,
            deaths_per_minute REAL NOT NULL,
            assists INTEGER NOT NULL,
            involvements INTEGER NOT NULL,
            involvements_per_minute REAL NOT NULL,
            consecutive_kills INTEGER NOT NULL,
            kill_death_ratio REAL NOT NULL,
            kill_death_assists_ratio REAL NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createBreakdownTable = @"CREATE TABLE IF NOT EXISTS Breakdown (
            breakdown_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            weapon_kills INTEGER NOT NULL,
            grenade_kills INTEGER NOT NULL,
            melee_kills INTEGER NOT NULL,
            other_kills INTEGER NOT NULL,
            weapon_kills_ratio REAL NOT NULL,
            grenade_kills_ratio REAL NOT NULL,
            melee_kills_ratio REAL NOT NULL,
            other_kills_ratio REAL NOT NULL,
            contribution_ratio REAL NOT NULL,
            kill_success_ratio REAL NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createRivalriesTable = @"CREATE TABLE IF NOT EXISTS Rivalries (
            rivalries_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            most_killed_player TEXT NOT NULL,
            most_killed_count INTEGER NOT NULL,
            most_killed_kill_ratio REAL NOT NULL,
            most_killer_player TEXT NOT NULL,
            most_killer_count INTEGER NOT NULL,
            most_killer_death_ratio REAL NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createSurvivabilityTable = @"CREATE TABLE IF NOT EXISTS Survivability (
            survivability_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            minutes_alive REAL NOT NULL,
            minutes_played REAL NOT NULL,
            alive_time_ratio REAL NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createChoiceTable = @"CREATE TABLE IF NOT EXISTS Choice (
            choice_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            most_used_weapon TEXT NOT NULL,
            most_used_weapon_kills INTEGER NOT NULL,
            most_used_weapon_kills_ratio REAL NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createMedalsTable = @"CREATE TABLE IF NOT EXISTS Medals (
            medals_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            total_medals INTEGER NOT NULL,
            medals_per_kill REAL NOT NULL,
            medals_per_minute REAL NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createMedalsInfo = @"CREATE TABLE IF NOT EXISTS MedalsInfo (
            medals_info_id INTEGER PRIMARY KEY AUTOINCREMENT,
            medals_id INTEGER NOT NULL,
            medal_type INTEGER NOT NULL,
            count INTEGER NOT NULL,
            FOREIGN KEY(medals_id) REFERENCES Medals(medals_id)
        )";

        var createPenaltiesTable = @"CREATE TABLE IF NOT EXISTS Penalties (
            penalties_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            suicides INTEGER NOT NULL,
            suicides_per_death REAL NOT NULL,
            betrayals INTEGER NOT NULL,
            betrayals_per_kill REAL NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createSlayerTable = @"CREATE TABLE IF NOT EXISTS Slayer (
            slayer_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            rating REAL NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createCTFTable = @"CREATE TABLE IF NOT EXISTS CaptureTheFlag (
            ctf_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            flag_captures INTEGER NOT NULL,
            flag_recovers INTEGER NOT NULL,
            flag_carry_time REAL NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createOddballTable = @"CREATE TABLE IF NOT EXISTS Oddball (
            oddball_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            carry_time REAL NOT NULL,
            ball_kills INTEGER NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createKOTHTable = @"CREATE TABLE IF NOT EXISTS KingOfTheHill (
            koth_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            time_in_hill REAL NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createJuggernautTable = @"CREATE TABLE IF NOT EXISTS Juggernaut (
            juggernaut_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            juggernaut_time REAL NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createInfectionTable = @"CREATE TABLE IF NOT EXISTS Infection (
            infection_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            survival_time REAL NOT NULL,
            infections INTEGER NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createTerritoriesTable = @"CREATE TABLE IF NOT EXISTS Territories (
            territories_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            captures INTEGER NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createAssaultTable = @"CREATE TABLE IF NOT EXISTS Assault (
            assault_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            bombs_planted INTEGER NOT NULL,
            detonations INTEGER NOT NULL,
            bomb_carry_time REAL NOT NULL,
            defuses INTEGER NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createStockpileTable = @"CREATE TABLE IF NOT EXISTS Stockpile (
            stockpile_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            carry_time REAL NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createHeadHunterTable = @"CREATE TABLE IF NOT EXISTS HeadHunter (
            head_hunter_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            max_skulls INTEGER NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var createActionSackTable = @"CREATE TABLE IF NOT EXISTS ActionSack (
            action_sack_id INTEGER PRIMARY KEY AUTOINCREMENT,
            player_id TEXT NOT NULL,
            FOREIGN KEY(player_id) REFERENCES Players(player_id)
        )";

        var commands = new string[]
        {
            createMatchesTable,
            createTeamsTable,
            createProfilesTable,
            createCustomizationTable,
            createPlayersTable,
            createCombatTable,
            createBreakdownTable,
            createRivalriesTable,
            createSurvivabilityTable,
            createChoiceTable,
            createMedalsTable,
            createMedalsInfo,
            createPenaltiesTable,
            createSlayerTable,
            createCTFTable,
            createOddballTable,
            createKOTHTable,
            createJuggernautTable,
            createInfectionTable,
            createTerritoriesTable,
            createAssaultTable,
            createStockpileTable,
            createHeadHunterTable,
            createActionSackTable
        };

        foreach (var command in commands)
        {
            using var cmd = new SqliteCommand(command, connection);
            cmd.ExecuteNonQuery();
        }

        // TODO: insertar los datos en las tablas, habilitar 'PRAGMA foreign_keys', hacer metodos para esto.
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