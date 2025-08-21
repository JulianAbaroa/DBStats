using DBStats.DataTypes;
using DBStats.DataTypes.GameTypes;
using DBStats.DataTypes.Player;
using DBStats.DataTypes.Profiles;
using Microsoft.Data.Sqlite;

namespace DBStats.DataBase;

public class DataBaseInserter
{
    public static void Insert(SqliteConnection connection, Match match, List<Profile> profiles)
    {
        AddMatch(connection, match);

        foreach (var profile in profiles)
        {
            AddProfile(connection, profile);
            AddCustomization(connection, profile.Customization, profile.PlayerID);
        }

        foreach (var team in match.Teams)
        {
            long teamID = AddTeam(connection, team, match.MatchID);

            if (team.GameMode is Slayer teamSlayer)
            {
                AddTeamSlayer(connection, teamSlayer, teamID);
            }
            else if (team.GameMode is CaptureTheFlag teamCaptureTheFlag)
            {
                AddTeamCaptureTheFlag(connection, teamCaptureTheFlag, teamID);
            }
            else if (team.GameMode is Oddball teamOddball)
            {
                AddTeamOddball(connection, teamOddball, teamID);
            }
            else if (team.GameMode is KingOfTheHill teamKingOfTheHill)
            {
                AddTeamKingOfTheHill(connection, teamKingOfTheHill, teamID);
            }
            else if (team.GameMode is Juggernaut teamJuggernaut)
            {
                AddTeamJuggernaut(connection, teamJuggernaut, teamID);
            }
            else if (team.GameMode is Infection teamInfection)
            {
                AddTeamInfection(connection, teamInfection, teamID);
            }
            else if (team.GameMode is Territories teamTerritories)
            {
                AddTeamTerritories(connection, teamTerritories, teamID);
            }
            else if (team.GameMode is Assault teamAssault)
            {
                AddTeamAssault(connection, teamAssault, teamID);
            }
            else if (team.GameMode is Stockpile teamStockpile)
            {
                AddTeamStockpile(connection, teamStockpile, teamID);
            }
            else if (team.GameMode is HeadHunter teamHeadHunter)
            {
                AddTeamHeadHunter(connection, teamHeadHunter, teamID);
            }
            else if (team.GameMode is ActionSack)
            {
                AddTeamActionSack(connection, teamID);
            }

            foreach (var player in team.Players)
            {
                AddPlayer(connection, player, teamID);
                AddCombat(connection, player.Combat, player.PlayerID, teamID);
                AddBreakdown(connection, player.Breakdown, player.PlayerID, teamID);
                AddRivalries(connection, player.Rivalries, player.PlayerID, teamID);
                AddSurvivability(connection, player.Survivability, player.PlayerID, teamID);
                AddChoice(connection, player.Choice, player.PlayerID, teamID);
                long medalsID = AddMedals(connection, player.Medals, player.PlayerID, teamID);
                AddMedalsInfo(connection, player.Medals.MedalsInfo, medalsID);
                AddPenalties(connection, player.Penalties, player.PlayerID, teamID);

                if (player.GameMode is Slayer slayer)
                {
                    AddSlayer(connection, slayer, player.PlayerID, teamID);
                }
                else if (player.GameMode is CaptureTheFlag captureTheFlag)
                {
                    AddCaptureTheFlag(connection, captureTheFlag, player.PlayerID, teamID);
                }
                else if (player.GameMode is Oddball oddball)
                {
                    AddOddball(connection, oddball, player.PlayerID, teamID);
                }
                else if (player.GameMode is KingOfTheHill kingOfTheHill)
                {
                    AddKingOfTheHill(connection, kingOfTheHill, player.PlayerID, teamID);
                }
                else if (player.GameMode is Juggernaut juggernaut)
                {
                    AddJuggernaut(connection, juggernaut, player.PlayerID, teamID);
                }
                else if (player.GameMode is Infection infection)
                {
                    AddInfection(connection, infection, player.PlayerID, teamID);
                }
                else if (player.GameMode is Territories territories)
                {
                    AddTerritories(connection, territories, player.PlayerID, teamID);
                }
                else if (player.GameMode is Assault assault)
                {
                    AddAssault(connection, assault, player.PlayerID, teamID);
                }
                else if (player.GameMode is Stockpile stockpile)
                {
                    AddStockpile(connection, stockpile, player.PlayerID, teamID);
                }
                else if (player.GameMode is HeadHunter headHunter)
                {
                    AddHeadHunter(connection, headHunter, player.PlayerID, teamID);
                }
                else if (player.GameMode is ActionSack)
                {
                    AddActionSack(connection, player.PlayerID, teamID);
                }
            }
        }
    }

    private static void AddMatch(SqliteConnection connection, Match match)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT OR IGNORE INTO Matches (
            match_id,
            gametype,
            gametype_name,
            is_matchmaking,
            was_match_incomplete,
            is_teams_enabled,
            duration,
            carnage_path,
            match_timestamp
        ) VALUES (
            $match_id,
            $gametype,
            $gametype_name,
            $is_matchmaking,
            $was_match_incomplete,
            $is_teams_enabled,
            $duration,
            $carnage_path,
            $match_timestamp
        );";

        cmd.Parameters.AddWithValue("$match_id", match.MatchID);
        cmd.Parameters.AddWithValue("$gametype", match.GameType);
        cmd.Parameters.AddWithValue("$gametype_name", match.GameTypeName);
        cmd.Parameters.AddWithValue("$is_matchmaking", match.IsMatchmaking ? 1 : 0);
        cmd.Parameters.AddWithValue("$was_match_incomplete", match.WasMatchIncomplete ? 1 : 0);
        cmd.Parameters.AddWithValue("$is_teams_enabled", match.IsTeamsEnabled ? 1 : 0);
        cmd.Parameters.AddWithValue("$duration", match.Duration);
        cmd.Parameters.AddWithValue("$carnage_path", match.CarnagePath);
        cmd.Parameters.AddWithValue("$match_timestamp", match.MatchTimestamp);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static long AddTeam(SqliteConnection connection, Team team, string matchID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Teams (
            match_id,
            color,
            rating,
            winned,
            deaths,
            kills
        ) VALUES (
            $match_id,
            $color,
            $rating,
            $winned,
            $deaths,
            $kills
        );";

        cmd.Parameters.AddWithValue("$match_id", matchID);
        cmd.Parameters.AddWithValue("$color", team.Color);
        cmd.Parameters.AddWithValue("$rating", team.Rating);
        cmd.Parameters.AddWithValue("$winned", team.Winned);
        cmd.Parameters.AddWithValue("$deaths", team.Deaths);
        cmd.Parameters.AddWithValue("$kills", team.Kills);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }

        using var lastIDCMD = connection.CreateCommand();
        lastIDCMD.CommandText = "SELECT last_insert_rowid();";
        object? result = lastIDCMD.ExecuteScalar();

        if (result == null || result == DBNull.Value)
        {
            throw new InvalidOperationException("Error: The generated team_id could not be obtained.");
        }

        long teamID = Convert.ToInt64(result);
        return teamID;
    }

    private static void AddTeamSlayer(SqliteConnection connection, Slayer slayer, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO SlayerTeams (
            team_id,
            rating
        ) VALUES (
            $team_id,
            $rating
        );";

        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$rating", slayer.Rating);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddTeamCaptureTheFlag(SqliteConnection connection, CaptureTheFlag captureTheFlag, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO CTFTeams (
            team_id,
            flag_captures,
            flag_recovers,
            flag_carry_time
        ) VALUES (
            $team_id,
            $flag_captures,
            $flag_recovers,
            $flag_carry_time
        );";

        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$flag_captures", captureTheFlag.FlagCaptures);
        cmd.Parameters.AddWithValue("$flag_recovers", captureTheFlag.FlagRecovers);
        cmd.Parameters.AddWithValue("$flag_carry_time", captureTheFlag.FlagCarryTime);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddTeamOddball(SqliteConnection connection, Oddball oddball, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO OddballTeams (
            team_id,
            carry_time,
            ball_kills
        ) VALUES (
            $team_id,
            $carry_time,
            $ball_kills
        );";

        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$carry_time", oddball.CarryTime);
        cmd.Parameters.AddWithValue("$ball_kills", oddball.BallKills);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddTeamKingOfTheHill(SqliteConnection connection, KingOfTheHill kingOfTheHill, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO KingOfTheHillTeams (
            team_id,
            time_in_hill
        ) VALUES (
            $team_id,
            $time_in_hill
        );";

        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$time_in_hill", kingOfTheHill.TimeinHill);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddTeamJuggernaut(SqliteConnection connection, Juggernaut juggernaut, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO JuggernautTeams (
            team_id,
            juggernaut_time
        ) VALUES (
            $team_id,
            $juggernaut_time
        );";

        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$juggernaut_time", juggernaut.JuggernautTime);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddTeamInfection(SqliteConnection connection, Infection infection, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO InfectionTeams (
            team_id,
            survival_time,
            infections
        ) VALUES (
            $team_id,
            $survival_time,
            $infections
        );";

        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$survival_time", infection.SurvivalTime);
        cmd.Parameters.AddWithValue("$infections", infection.Infections);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddTeamTerritories(SqliteConnection connection, Territories territories, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO TerritoriesTeams (
            team_id,
            captures
        ) VALUES (
            $team_id,
            $captures
        );";

        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$captures", territories.Captures);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddTeamAssault(SqliteConnection connection, Assault assault, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO AssaultTeams (
            team_id,
            bombs_planted,
            detonations,
            bomb_carry_time,
            defuses
        ) VALUES (
            $team_id,
            $bombs_planted,
            $detonations,
            $bomb_carry_time,
            $defuses
        );";

        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$bombs_planted", assault.BombsPlanted);
        cmd.Parameters.AddWithValue("$detonations", assault.Detonations);
        cmd.Parameters.AddWithValue("$bomb_carry_time", assault.BombCarryTime);
        cmd.Parameters.AddWithValue("$defuses", assault.Defuses);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddTeamStockpile(SqliteConnection connection, Stockpile stockpile, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO StockpileTeams (
            team_id,
            carry_time
        ) VALUES (
            $team_id,
            $carry_time
        );";

        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$carry_time", stockpile.CarryTime);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddTeamHeadHunter(SqliteConnection connection, HeadHunter headHunter, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO HeadHunterTeams (
            team_id,
            max_skulls
        ) VALUES (
            $team_id,
            $max_skulls
        );";

        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$max_skulls", headHunter.MaxSkulls);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddTeamActionSack(SqliteConnection connection, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO ActionSackTeams (
            team_id
        ) VALUES (
            $team_id
        );";

        cmd.Parameters.AddWithValue("$team_id", teamID);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddProfile(SqliteConnection connection, Profile profile)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT OR REPLACE INTO Profiles (
            player_id,
            player_name,
            last_seen
        ) VALUES (
            $player_id,
            $player_name,
            $last_seen
        );";

        cmd.Parameters.AddWithValue("$player_id", profile.PlayerID);
        cmd.Parameters.AddWithValue("$player_name", profile.PlayerName);
        cmd.Parameters.AddWithValue("$last_seen", profile.LastSeen);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddCustomization(SqliteConnection connection, Customization customization, string playerID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT OR REPLACE INTO Customizations (
            player_id,
            service_id,
            clan_tag,
            nameplate_path,
            emblem_path
        ) VALUES (
            $player_id,
            $service_id,
            $clan_tag,
            $nameplate_path,
            $emblem_path
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$service_id", customization.ServiceID);
        cmd.Parameters.AddWithValue("$clan_tag", customization.ClanTag);
        cmd.Parameters.AddWithValue("$nameplate_path", customization.NameplatePath);
        cmd.Parameters.AddWithValue("$emblem_path", customization.EmblemPath);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddPlayer(SqliteConnection connection, PlayerMatchStats player, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Players (
            player_id,
            team_id,
            rating
        ) VALUES (
            $player_id,
            $team_id,
            $rating
        );";

        cmd.Parameters.AddWithValue("$player_id", player.PlayerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$rating", player.Rating);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddCombat(SqliteConnection connection, Combat combat, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Combat (
            player_id,
            team_id,
            kills,
            kills_per_minute,
            deaths,
            deaths_per_minute,
            assists,
            involvements,
            involvements_per_minute,
            consecutive_kills,
            kill_death_ratio,
            kill_death_assists_ratio
        ) VALUES (
            $player_id,
            $team_id,
            $kills,
            $kills_per_minute,
            $deaths,
            $deaths_per_minute,
            $assists,
            $involvements,
            $involvements_per_minute,
            $consecutive_kills,
            $kill_death_ratio,
            $kill_death_assists_ratio
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$kills", combat.Kills);
        cmd.Parameters.AddWithValue("$kills_per_minute", combat.KillsPerMinute);
        cmd.Parameters.AddWithValue("$deaths", combat.Deaths);
        cmd.Parameters.AddWithValue("$deaths_per_minute", combat.DeathsPerMinute);
        cmd.Parameters.AddWithValue("$assists", combat.Assists);
        cmd.Parameters.AddWithValue("$involvements", combat.Involvements);
        cmd.Parameters.AddWithValue("$involvements_per_minute", combat.InvolvementsPerMinute);
        cmd.Parameters.AddWithValue("$consecutive_kills", combat.ConsecutiveKills);
        cmd.Parameters.AddWithValue("$kill_death_ratio", combat.KDRatio);
        cmd.Parameters.AddWithValue("$kill_death_assists_ratio", combat.KDARatio);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddBreakdown(SqliteConnection connection, Breakdown breakdown, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Breakdown (
            player_id,
            team_id,
            weapon_kills,
            grenade_kills,
            melee_kills,
            other_kills,
            weapon_kills_ratio,
            grenade_kills_ratio,
            melee_kills_ratio,
            other_kills_ratio,
            kill_success_ratio
        ) VALUES (
            $player_id,
            $team_id,
            $weapon_kills,
            $grenade_kills,
            $melee_kills,
            $other_kills,
            $weapon_kills_ratio,
            $grenade_kills_ratio,
            $melee_kills_ratio,
            $other_kills_ratio,
            $kill_success_ratio
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$weapon_kills", breakdown.WeaponKills);
        cmd.Parameters.AddWithValue("$grenade_kills", breakdown.GrenadeKills);
        cmd.Parameters.AddWithValue("$melee_kills", breakdown.MeleeKills);
        cmd.Parameters.AddWithValue("$other_kills", breakdown.OtherKills);
        cmd.Parameters.AddWithValue("$weapon_kills_ratio", breakdown.WeaponKillsRatio);
        cmd.Parameters.AddWithValue("$grenade_kills_ratio", breakdown.GrenadeKillsRatio);
        cmd.Parameters.AddWithValue("$melee_kills_ratio", breakdown.MeleeKillsRatio);
        cmd.Parameters.AddWithValue("$other_kills_ratio", breakdown.OtherKillsRatio);
        cmd.Parameters.AddWithValue("$kill_success_ratio", breakdown.KillSuccessRatio);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddRivalries(SqliteConnection connection, Rivalries rivalries, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Rivalries (
            player_id,
            team_id,
            most_killed_player,
            most_killed_count,
            most_killed_kill_ratio,
            most_killer_player,
            most_killer_count,
            most_killer_death_ratio
        ) VALUES (
            $player_id,
            $team_id,
            $most_killed_player,
            $most_killed_count,
            $most_killed_kill_ratio,
            $most_killer_player,
            $most_killer_count,
            $most_killer_death_ratio
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$most_killed_player", rivalries.MostKilledPlayer);
        cmd.Parameters.AddWithValue("$most_killed_count", rivalries.MostKilledCount);
        cmd.Parameters.AddWithValue("$most_killed_kill_ratio", rivalries.MostKilledKillRatio);
        cmd.Parameters.AddWithValue("$most_killer_player", rivalries.MostKillerPlayer);
        cmd.Parameters.AddWithValue("$most_killer_count", rivalries.MostKillerCount);
        cmd.Parameters.AddWithValue("$most_killer_death_ratio", rivalries.MostKillerDeathRatio);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddSurvivability(SqliteConnection connection, Survivability survivability, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Survivability (
            player_id,
            team_id,
            minutes_alive,
            minutes_played,
            alive_time_ratio
        ) VALUES (
            $player_id,
            $team_id,
            $minutes_alive,
            $minutes_played,
            $alive_time_ratio
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$minutes_alive", survivability.MinutesAlive);
        cmd.Parameters.AddWithValue("$minutes_played", survivability.MinutesPlayed);
        cmd.Parameters.AddWithValue("$alive_time_ratio", survivability.AliveTimeRatio);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddChoice(SqliteConnection connection, Choice choice, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Choice (
            player_id,
            team_id,
            most_used_weapon,
            most_used_weapon_kills,
            most_used_weapon_kills_ratio
        ) VALUES (
            $player_id,
            $team_id,
            $most_used_weapon,
            $most_used_weapon_kills,
            $most_used_weapon_kills_ratio
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$most_used_weapon", choice.MostUsedWeapon);
        cmd.Parameters.AddWithValue("$most_used_weapon_kills", choice.MostUsedWeaponKills);
        cmd.Parameters.AddWithValue("$most_used_weapon_kills_ratio", choice.MostUsedWeaponKillsRatio);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static long AddMedals(SqliteConnection connection, Medals medals, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Medals (
            player_id,
            team_id,
            total_medals,
            medals_per_kill,
            medals_per_minute
        ) VALUES (
            $player_id,
            $team_id,
            $total_medals,
            $medals_per_kill,
            $medals_per_minute
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$total_medals", medals.TotalMedals);
        cmd.Parameters.AddWithValue("$medals_per_kill", medals.MedalsPerKill);
        cmd.Parameters.AddWithValue("$medals_per_minute", medals.MedalsPerMinute);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }

        using var lastMedalID = connection.CreateCommand();
        lastMedalID.CommandText = "SELECT last_insert_rowid();";
        object? result = lastMedalID.ExecuteScalar();

        if (result == null || result == DBNull.Value)
        {
            throw new InvalidOperationException("Error: Could not get the generated medal_id.");
        }

        long medalsID = Convert.ToInt64(result);
        return medalsID;
    }

    private static void AddMedalsInfo(SqliteConnection connection, MedalsInfo medalsInfo, long medalsID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO MedalsInfo (
            medals_id,
            medal_type,
            count
        ) VALUES (
            $medals_id,
            $medal_type,
            $count
        );";

        var mID = cmd.CreateParameter();
        mID.ParameterName = "$medals_id";
        mID.SqliteType = SqliteType.Integer;
        cmd.Parameters.Add(mID);

        var mType = cmd.CreateParameter();
        mType.ParameterName = "$medal_type";
        mType.SqliteType = SqliteType.Text;
        cmd.Parameters.Add(mType);

        var mCount = cmd.CreateParameter();
        mCount.ParameterName = "$count";
        mCount.SqliteType = SqliteType.Integer;
        cmd.Parameters.Add(mCount);

        foreach (var kv in medalsInfo.Types.Where(kv => kv.Value > 0))
        {
            mID.Value = medalsID;
            mType.Value = kv.Key.ToString();
            mCount.Value = kv.Value;

            cmd.ExecuteNonQuery();
        }
    }

    private static void AddPenalties(SqliteConnection connection, Penalties penalties, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Penalties (
            player_id,
            team_id,
            suicides,
            suicides_per_death,
            betrayals,
            betrayals_per_kill
        ) VALUES (
            $player_id,
            $team_id,
            $suicides,
            $suicides_per_death,
            $betrayals,
            $betrayals_per_kill
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$suicides", penalties.Suicides);
        cmd.Parameters.AddWithValue("$suicides_per_death", penalties.SuicidesPerDeath);
        cmd.Parameters.AddWithValue("$betrayals", penalties.Betrayals);
        cmd.Parameters.AddWithValue("$betrayals_per_kill", penalties.BetrayalsPerKill);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddSlayer(SqliteConnection connection, Slayer slayer, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Slayer (
            player_id,
            team_id,
            rating
        ) VALUES (
            $player_id,
            $team_id,
            $rating
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$rating", slayer.Rating);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddCaptureTheFlag(SqliteConnection connection, CaptureTheFlag captureTheFlag, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO CaptureTheFlag (
            player_id,
            team_id,
            flag_captures,
            flag_recovers,
            flag_carry_time
        ) VALUES (
            $player_id,
            $team_id,
            $flag_captures,
            $flag_recovers,
            $flag_carry_time
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$flag_captures", captureTheFlag.FlagCaptures);
        cmd.Parameters.AddWithValue("$flag_recovers", captureTheFlag.FlagRecovers);
        cmd.Parameters.AddWithValue("$flag_carry_time", captureTheFlag.FlagCarryTime);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddOddball(SqliteConnection connection, Oddball oddball, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Oddball (
            player_id,
            team_id,
            carry_time,
            ball_kills
        ) VALUES (
            $player_id,
            $team_id,
            $carry_time,
            $ball_kills
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$carry_time", oddball.CarryTime);
        cmd.Parameters.AddWithValue("$ball_kills", oddball.BallKills);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddKingOfTheHill(SqliteConnection connection, KingOfTheHill kingOfTheHill, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO KingOfTheHill (
            player_id,
            team_id,
            time_in_hill
        ) VALUES (
            $player_id,
            $team_id,
            $time_in_hill
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$time_in_hill", kingOfTheHill.TimeinHill);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddJuggernaut(SqliteConnection connection, Juggernaut juggernaut, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Juggernaut (
            player_id,
            team_id,
            juggernaut_time
        ) VALUES (
            $player_id,
            $team_id,
            $juggernaut_time
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$juggernaut_time", juggernaut.JuggernautTime);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddInfection(SqliteConnection connection, Infection infection, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Infection (
            player_id,
            team_id,
            survival_time,
            infections
        ) VALUES (
            $player_id,
            $team_id,
            $survival_time,
            $infections
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$survival_time", infection.SurvivalTime);
        cmd.Parameters.AddWithValue("$infections", infection.Infections);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddTerritories(SqliteConnection connection, Territories territories, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Territories (
            player_id,
            team_id,
            captures
        ) VALUES (
            $player_id,
            $team_id,
            $captures
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$captures", territories.Captures);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddAssault(SqliteConnection connection, Assault assault, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Assault (
            player_id,
            team_id,
            bombs_planted,
            detonations,
            bomb_carry_time,
            defuses
        ) VALUES (
            $player_id,
            $team_id,
            $bombs_planted,
            $detonations,
            $bomb_carry_time,
            $defuses
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$bombs_planted", assault.BombsPlanted);
        cmd.Parameters.AddWithValue("$detonations", assault.Detonations);
        cmd.Parameters.AddWithValue("$bomb_carry_time", assault.BombCarryTime);
        cmd.Parameters.AddWithValue("$defuses", assault.Defuses);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddStockpile(SqliteConnection connection, Stockpile stockpile, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Stockpile (
            player_id,
            team_id,
            carry_time
        ) VALUES (
            $player_id,
            $team_id,
            $carry_time
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$carry_time", stockpile.CarryTime);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddHeadHunter(SqliteConnection connection, HeadHunter headHunter, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO HeadHunter (
            player_id,
            team_id,
            max_skulls
        ) VALUES (
            $player_id,
            $team_id,
            $max_skulls
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);
        cmd.Parameters.AddWithValue("$max_skulls", headHunter.MaxSkulls);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

    private static void AddActionSack(SqliteConnection connection, string playerID, long teamID)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO ActionSack (
            player_id,
            team_id
        ) VALUES (
            $player_id,
            $team_id
        );";

        cmd.Parameters.AddWithValue("$player_id", playerID);
        cmd.Parameters.AddWithValue("$team_id", teamID);

        int rows = cmd.ExecuteNonQuery();
        if (rows == 0)
        {
            Console.WriteLine($"[WARN] Insert ignorado en {cmd.CommandText}");
        }
    }

}