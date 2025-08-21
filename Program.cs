using Microsoft.Data.Sqlite;
using DBStats.DataTranslators.Profile;
using DBStats.Duplicates;
using DBStats.DataTypes;
using DBStats.DataBase;
using DBStats.Parser;
using System.Xml;

namespace DBStats;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            //return;
        }

        //string filePath = args[0];

        //if (!File.Exists(filePath))
        //{
        //    throw new FileNotFoundException($"Error: the file is not found in the path: {filePath}");
        //}

        // TODO: TEMPORAL PATH.
        string carnagePath = @"C:\Users\maste\OneDrive\Documents\Halo\Carnages\CarnageReport_PlaceHolder021_20250820_230210.xml";

        // Initialization.
        AssetsMapper.LoadMaps();
        CheckForDuplicates(carnagePath);

        var carnageReport = new XmlDocument();
        carnageReport.Load(carnagePath);

        XmlNode playerNodes = carnageReport.SelectSingleNode("/MultiplayerCarnageReport/Players")
            ?? throw new NullReferenceException("Error: The 'Players' node was not found in the XML.");

        Match match = CarnageReportParser.ParseMatch(carnageReport, playerNodes, carnagePath);
        List<Profile> profiles = ProfilesParser.ParseProfiles(playerNodes);

        // TODO: TEMPORAL PATH.
        string folderPath = @"C:\Users\maste\OneDrive\Documents\Halo\DBStats DataBase";

        if (!File.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string connectionString = $"Data Source={Path.Combine(folderPath, "dbstats.db")}";

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        InitializeCommands(connection);
        DataBaseInitializer.Initialize(connection);
        DataBaseInserter.Insert(connection, match, profiles);
    }

    private static void CheckForDuplicates(string carnagePath)
    {
        string? matchHash = MatchHasher.ComputeMatchHash(carnagePath);

        if (string.IsNullOrEmpty(matchHash))
        {
            throw new InvalidOperationException("Error: matchHash is not valid.");
        }

        // TODO: TEMPORAL PATH.
        string lastHashPath = @"C:\Users\maste\OneDrive\Documents\Halo\DBStats\Duplicates\last_hash.json";

        string lastHash = File.Exists(lastHashPath) ? File.ReadAllText(lastHashPath) : string.Empty;

        if (matchHash == lastHash)
        {
            throw new InvalidOperationException("Error: duplicated file detected, aborting.");
        }

        File.WriteAllText(lastHashPath, matchHash);
    }

    private static void InitializeCommands(SqliteConnection connection)
    {
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "PRAGMA foreign_keys = ON;";
            cmd.ExecuteNonQuery();
        }

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "PRAGMA table_info('Teams');";
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"col: {reader["name"]} type: {reader["type"]} pk: {reader["pk"]}");
            }
        }

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "PRAGMA foreign_key_list('Teams');";
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"fk: from {reader["from"]} -> {reader["table"]}.{reader["to"]} ondelete={reader["on_delete"]}");
            }
        }
    }

}