using Microsoft.Data.Sqlite;

namespace DBStats.DataBase;

public class DataBaseInitializer
{
    public static void Initialize(SqliteConnection connection)
    {
        using var transaction = connection.BeginTransaction();

        try
        {
            var createMatchesTable = @"CREATE TABLE IF NOT EXISTS Matches (
                match_id TEXT PRIMARY KEY NOT NULL,
                gametype INTEGER NOT NULL,
                gametype_name TEXT NOT NULL,
                is_matchmaking INTEGER NOT NULL,
                was_match_incomplete INTEGER NOT NULL,
                is_teams_enabled INTEGER NOT NULL,
                duration REAL NOT NULL,
                carnage_path TEXT NOT NULL,
                match_timestamp TEXT NOT NULL
            );";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createMatchesTable;
                cmd.ExecuteNonQuery();
            }

            var createTeamsTable = @"CREATE TABLE IF NOT EXISTS Teams (
                team_id INTEGER PRIMARY KEY AUTOINCREMENT,
                match_id TEXT NOT NULL,
                result TEXT NOT NULL,
                color TEXT NOT NULL,
                rating REAL NOT NULL,
                deaths INTEGER NOT NULL,
                kills INTEGER NOT NULL,
                FOREIGN KEY(match_id) REFERENCES Matches(match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createTeamsTable;
                cmd.ExecuteNonQuery();
            }

            var createSlayerTeamsTable = @"CREATE TABLE IF NOT EXISTS SlayerTeams (
                slayer_team_id INTEGER PRIMARY KEY AUTOINCREMENT,
                team_id INTEGER NOT NULL,
                rating REAL NOT NULL,
                FOREIGN KEY(team_id) REFERENCES Teams(team_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createSlayerTeamsTable;
                cmd.ExecuteNonQuery();
            }

            var createCTFTeamsTable = @"CREATE TABLE IF NOT EXISTS CTFTeams (
                ctf_team_id INTEGER PRIMARY KEY AUTOINCREMENT,
                team_id INTEGER NOT NULL,
                flag_captures INTEGER NOT NULL,
                flag_recovers INTEGER NOT NULL,
                flag_carry_time REAL NOT NULL,
                FOREIGN KEY(team_id) REFERENCES Teams(team_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createCTFTeamsTable;
                cmd.ExecuteNonQuery();
            }

            var createOddballTeamsTable = @"CREATE TABLE IF NOT EXISTS OddballTeams (
                oddball_team_id INTEGER PRIMARY KEY AUTOINCREMENT,
                team_id INTEGER NOT NULL,
                carry_time REAL NOT NULL,
                ball_kills INTEGER NOT NULL,
                FOREIGN KEY(team_id) REFERENCES Teams(team_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createOddballTeamsTable;
                cmd.ExecuteNonQuery();
            }

            var createKOTHTeamsTable = @"CREATE TABLE IF NOT EXISTS KingOfTheHillTeams (
                koth_team_id INTEGER PRIMARY KEY AUTOINCREMENT,
                team_id INTEGER NOT NULL,
                time_in_hill REAL NOT NULL,
                FOREIGN KEY(team_id) REFERENCES Teams(team_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createKOTHTeamsTable;
                cmd.ExecuteNonQuery();
            }

            var createJuggernautTeamsTable = @"CREATE TABLE IF NOT EXISTS JuggernautTeams (
                juggernaut_team_id INTEGER PRIMARY KEY AUTOINCREMENT,
                team_id INTEGER NOT NULL,
                juggernaut_time REAL NOT NULL,
                FOREIGN KEY(team_id) REFERENCES Teams(team_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createJuggernautTeamsTable;
                cmd.ExecuteNonQuery();
            }

            var createInfectionTeamsTable = @"CREATE TABLE IF NOT EXISTS InfectionTeams (
                infection_team_id INTEGER PRIMARY KEY AUTOINCREMENT,
                team_id INTEGER NOT NULL,
                survival_time REAL NOT NULL,
                infections INTEGER NOT NULL,
                FOREIGN KEY(team_id) REFERENCES Teams(team_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createInfectionTeamsTable;
                cmd.ExecuteNonQuery();
            }

            var createTerritoriesTeamsTable = @"CREATE TABLE IF NOT EXISTS TerritoriesTeams (
                territories_team_id INTEGER PRIMARY KEY AUTOINCREMENT,
                team_id INTEGER NOT NULL,
                captures INTEGER NOT NULL,
                FOREIGN KEY(team_id) REFERENCES Teams(team_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createTerritoriesTeamsTable;
                cmd.ExecuteNonQuery();
            }

            var createAssaultTeamsTable = @"CREATE TABLE IF NOT EXISTS AssaultTeams (
                assault_team_id INTEGER PRIMARY KEY AUTOINCREMENT,
                team_id INTEGER NOT NULL,
                bombs_planted INTEGER NOT NULL,
                detonations INTEGER NOT NULL,
                bomb_carry_time REAL NOT NULL,
                defuses INTEGER NOT NULL,
                FOREIGN KEY(team_id) REFERENCES Teams(team_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createAssaultTeamsTable;
                cmd.ExecuteNonQuery();
            }

            var createStockpileTeamsTable = @"CREATE TABLE IF NOT EXISTS StockpileTeams (
                stockpile_team_id INTEGER PRIMARY KEY AUTOINCREMENT,
                team_id INTEGER NOT NULL,
                carry_time REAL NOT NULL,
                FOREIGN KEY(team_id) REFERENCES Teams(team_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createStockpileTeamsTable;
                cmd.ExecuteNonQuery();
            }

            var createHeadHunterTeamsTable = @"CREATE TABLE IF NOT EXISTS HeadHunterTeams (
                head_hunter_team_id INTEGER PRIMARY KEY AUTOINCREMENT,
                team_id INTEGER NOT NULL,
                max_skulls INTEGER NOT NULL,
                FOREIGN KEY(team_id) REFERENCES Teams(team_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createHeadHunterTeamsTable;
                cmd.ExecuteNonQuery();
            }

            var createActionSackTeamsTable = @"CREATE TABLE IF NOT EXISTS ActionSackTeams (
                action_sack_id INTEGER PRIMARY KEY AUTOINCREMENT,
                team_id INTEGER NOT NULL,
                FOREIGN KEY(team_id) REFERENCES Teams(team_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createActionSackTeamsTable;
                cmd.ExecuteNonQuery();
            }

            var createProfilesTable = @"CREATE TABLE IF NOT EXISTS Profiles (
                player_id TEXT PRIMARY KEY NOT NULL,
                player_name TEXT NOT NULL,
                last_seen DATETIME            
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createProfilesTable;
                cmd.ExecuteNonQuery();
            }

            var createCustomizationTable = @"CREATE TABLE IF NOT EXISTS Customizations (
                player_id TEXT PRIMARY KEY NOT NULL,
                service_id TEXT NOT NULL,
                clan_tag TEXT NOT NULL,
                nameplate_path TEXT NOT NULL,
                emblem_path TEXT NOT NULL,
                FOREIGN KEY(player_id) REFERENCES Profiles(player_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createCustomizationTable;
                cmd.ExecuteNonQuery();
            }

            var createPlayersTable = @"CREATE TABLE IF NOT EXISTS Players (
                player_match_id INTEGER PRIMARY KEY AUTOINCREMENT,  
                player_id TEXT NOT NULL,
                team_id INTEGER NOT NULL,
                score INTEGER NOT NULL,
                rating REAL NOT NULL,
                FOREIGN KEY(player_id) REFERENCES Profiles(player_id) ON DELETE CASCADE,
                FOREIGN KEY(team_id) REFERENCES Teams(team_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createPlayersTable;
                cmd.ExecuteNonQuery();
            }

            var createCombatTable = @"CREATE TABLE IF NOT EXISTS Combat (
                combat_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
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
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createCombatTable;
                cmd.ExecuteNonQuery();
            }

            var createBreakdownTable = @"CREATE TABLE IF NOT EXISTS Breakdown (
                breakdown_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                weapon_kills INTEGER NOT NULL,
                grenade_kills INTEGER NOT NULL,
                melee_kills INTEGER NOT NULL,
                other_kills INTEGER NOT NULL,
                weapon_kills_ratio REAL NOT NULL,
                grenade_kills_ratio REAL NOT NULL,
                melee_kills_ratio REAL NOT NULL,
                other_kills_ratio REAL NOT NULL,
                kill_success_ratio REAL NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createBreakdownTable;
                cmd.ExecuteNonQuery();
            }

            var createRivalriesTable = @"CREATE TABLE IF NOT EXISTS Rivalries (
                rivalries_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                most_killed_player TEXT NOT NULL,
                most_killed_count INTEGER NOT NULL,
                most_killed_kill_ratio REAL NOT NULL,
                most_killer_player TEXT NOT NULL,
                most_killer_count INTEGER NOT NULL,
                most_killer_death_ratio REAL NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createRivalriesTable;
                cmd.ExecuteNonQuery();
            }

            var createSurvivabilityTable = @"CREATE TABLE IF NOT EXISTS Survivability (
                survivability_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                minutes_alive REAL NOT NULL,
                minutes_played REAL NOT NULL,
                alive_time_ratio REAL NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createSurvivabilityTable;
                cmd.ExecuteNonQuery();
            }

            var createChoiceTable = @"CREATE TABLE IF NOT EXISTS Choice (
                choice_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                most_used_weapon TEXT NOT NULL,
                most_used_weapon_kills INTEGER NOT NULL,
                most_used_weapon_kills_ratio REAL NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createChoiceTable;
                cmd.ExecuteNonQuery();
            }

            var createMedalsTable = @"CREATE TABLE IF NOT EXISTS Medals (
                medals_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                total_medals INTEGER NOT NULL,
                medals_per_kill REAL NOT NULL,
                medals_per_minute REAL NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createMedalsTable;
                cmd.ExecuteNonQuery();
            }

            var createMedalsInfo = @"CREATE TABLE IF NOT EXISTS MedalsInfo (
                medals_info_id INTEGER PRIMARY KEY AUTOINCREMENT,
                medals_id INTEGER NOT NULL,
                medal_type TEXT NOT NULL,
                count INTEGER NOT NULL,
                FOREIGN KEY(medals_id) REFERENCES Medals(medals_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createMedalsInfo;
                cmd.ExecuteNonQuery();
            }

            var createPenaltiesTable = @"CREATE TABLE IF NOT EXISTS Penalties (
                penalties_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                suicides INTEGER NOT NULL,
                suicides_per_death REAL NOT NULL,
                betrayals INTEGER NOT NULL,
                betrayals_per_kill REAL NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createPenaltiesTable;
                cmd.ExecuteNonQuery();
            }

            var createSlayerTable = @"CREATE TABLE IF NOT EXISTS Slayer (
                slayer_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                rating REAL NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createSlayerTable;
                cmd.ExecuteNonQuery();
            }

            var createCTFTable = @"CREATE TABLE IF NOT EXISTS CaptureTheFlag (
                ctf_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                flag_captures INTEGER NOT NULL,
                flag_recovers INTEGER NOT NULL,
                flag_carry_time REAL NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createCTFTable;
                cmd.ExecuteNonQuery();
            }

            var createOddballTable = @"CREATE TABLE IF NOT EXISTS Oddball (
                oddball_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                carry_time REAL NOT NULL,
                ball_kills INTEGER NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createOddballTable;
                cmd.ExecuteNonQuery();
            }

            var createKOTHTable = @"CREATE TABLE IF NOT EXISTS KingOfTheHill (
                koth_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                time_in_hill REAL NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createKOTHTable;
                cmd.ExecuteNonQuery();
            }

            var createJuggernautTable = @"CREATE TABLE IF NOT EXISTS Juggernaut (
                juggernaut_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                juggernaut_time REAL NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createJuggernautTable;
                cmd.ExecuteNonQuery();
            }

            var createInfectionTable = @"CREATE TABLE IF NOT EXISTS Infection (
                infection_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                survival_time REAL NOT NULL,
                infections INTEGER NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createInfectionTable;
                cmd.ExecuteNonQuery();
            }

            var createTerritoriesTable = @"CREATE TABLE IF NOT EXISTS Territories (
                territories_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                captures INTEGER NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createTerritoriesTable;
                cmd.ExecuteNonQuery();
            }

            var createAssaultTable = @"CREATE TABLE IF NOT EXISTS Assault (
                assault_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                bombs_planted INTEGER NOT NULL,
                detonations INTEGER NOT NULL,
                bomb_carry_time REAL NOT NULL,
                defuses INTEGER NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createAssaultTable;
                cmd.ExecuteNonQuery();
            }

            var createStockpileTable = @"CREATE TABLE IF NOT EXISTS Stockpile (
                stockpile_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                carry_time REAL NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createStockpileTable;
                cmd.ExecuteNonQuery();
            }

            var createHeadHunterTable = @"CREATE TABLE IF NOT EXISTS HeadHunter (
                head_hunter_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                max_skulls INTEGER NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createHeadHunterTable;
                cmd.ExecuteNonQuery();
            }

            var createActionSackTable = @"CREATE TABLE IF NOT EXISTS ActionSack (
                action_sack_id INTEGER PRIMARY KEY AUTOINCREMENT,
                player_match_id INTEGER NOT NULL,
                FOREIGN KEY(player_match_id) REFERENCES Players(player_match_id) ON DELETE CASCADE
            )";

            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = createActionSackTable;
                cmd.ExecuteNonQuery();
            }

            var indexCommands = new string[]
            {
                "CREATE INDEX IF NOT EXISTS idx_matches_timestamp ON Matches(match_timestamp);",
                "CREATE INDEX IF NOT EXISTS idx_matches_gametype_timestamp ON Matches(gametype_name, match_timestamp);",
                "CREATE INDEX IF NOT EXISTS idx_teams_match ON Teams(match_id);",

                "CREATE INDEX IF NOT EXISTS idx_players_playerid ON Players(player_id);",
                "CREATE INDEX IF NOT EXISTS idx_players_teamid ON Players(team_id);",

                "CREATE INDEX IF NOT EXISTS idx_combat_player_match ON Combat(player_match_id);",
                "CREATE INDEX IF NOT EXISTS idx_breakdown_player_match ON Breakdown(player_match_id);",
                "CREATE INDEX IF NOT EXISTS idx_rivalries_player_match ON Rivalries(player_match_id);",
                "CREATE INDEX IF NOT EXISTS idx_survivability_player_match ON Survivability(player_match_id);",
                "CREATE INDEX IF NOT EXISTS idx_choice_player_match ON Choice(player_match_id);",
                "CREATE INDEX IF NOT EXISTS idx_medals_player_match ON Medals(player_match_id);",
                "CREATE INDEX IF NOT EXISTS idx_penalties_player_match ON Penalties(player_match_id);",

                "CREATE INDEX IF NOT EXISTS idx_slayer_player_match ON Slayer(player_match_id);",
                "CREATE INDEX IF NOT EXISTS idx_ctf_player_match ON CaptureTheFlag(player_match_id);",
                "CREATE INDEX IF NOT EXISTS idx_oddball_player_match ON Oddball(player_match_id);",
                "CREATE INDEX IF NOT EXISTS idx_koth_player_match ON KingOfTheHill(player_match_id);",
                "CREATE INDEX IF NOT EXISTS idx_juggernaut_player_match ON Juggernaut(player_match_id);",
                "CREATE INDEX IF NOT EXISTS idx_infection_player_match ON Infection(player_match_id);",
                "CREATE INDEX IF NOT EXISTS idx_territories_player_match ON Territories(player_match_id);",
                "CREATE INDEX IF NOT EXISTS idx_assault_player_match ON Assault(player_match_id);",
                "CREATE INDEX IF NOT EXISTS idx_stockpile_player_match ON Stockpile(player_match_id);",

                "CREATE INDEX IF NOT EXISTS idx_medalsinfo_medalsid ON MedalsInfo(medals_id);",
                "CREATE INDEX IF NOT EXISTS idx_profiles_playername ON Profiles(player_name COLLATE NOCASE);",
                "CREATE INDEX IF NOT EXISTS idx_profiles_lastseen ON Profiles(last_seen);",
                "CREATE INDEX IF NOT EXISTS idx_matches_gametype ON Matches(gametype_name);"
            };



            foreach (var idx in indexCommands)
            {
                using var cmd = connection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = idx;
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {

            }

            throw;
        }
    }

}