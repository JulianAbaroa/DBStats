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
    static void Main()
    {
        try
        {
            Console.WriteLine("=== DBStats START ===");

            string[] carnagePaths = Directory.GetFiles(Paths.CARNAGES_DIR, "*.xml");

            Console.WriteLine($"Found {carnagePaths.Length} XML files to process.");

            if (carnagePaths.Length == 0)
            {
                Console.WriteLine("No XML files found. Exiting.");
                return;
            }

            foreach (string carnagePath in carnagePaths)
            {
                if (!File.Exists(carnagePath))
                {
                    Console.WriteLine("File.Exists returned false for: " + carnagePath);
                    throw new FileNotFoundException($"Error: the file is not found in the path: {carnagePath}");
                }

                if (CheckForDuplicates(carnagePath))
                {
                    continue;
                }

                // Initialization.
                AssetsMapper.LoadMaps();

                var carnageReport = new XmlDocument();
                carnageReport.Load(carnagePath);

                XmlNode playerNodes = carnageReport.SelectSingleNode("/MultiplayerCarnageReport/Players")
                    ?? throw new NullReferenceException("Error: The 'Players' node was not found in the XML.");

                Match match = CarnageReportParser.ParseMatch(carnageReport, playerNodes, carnagePath);
                List<Profile> profiles = ProfilesParser.ParseProfiles(playerNodes);

                if (!File.Exists(Paths.DATABASE_DIR))
                {
                    Directory.CreateDirectory(Paths.DATABASE_DIR);
                }

                string connectionString = $"Data Source={Path.Combine(Paths.DATABASE_DIR, "dbstats.db")}";

                using var connection = new SqliteConnection(connectionString);
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "PRAGMA foreign_keys = ON;";
                    cmd.ExecuteNonQuery();
                }

                DataBaseInitializer.Initialize(connection);
                DataBaseInserter.Insert(connection, match, profiles);
                SaveProcessedCarnage(carnagePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unhandled exception in DBStats:");
            Console.WriteLine(ex.ToString());
        }
    }

    private static bool CheckForDuplicates(string carnagePath)
    {
        string? matchHash = MatchHasher.ComputeMatchHash(carnagePath);

        if (string.IsNullOrEmpty(matchHash))
        {
            throw new InvalidOperationException("Error: matchHash is not valid.");
        }

        List<string> processedHashes = [];
        if (File.Exists(Paths.SAVED_HASHES_PATH))
        {
            string json = File.ReadAllText(Paths.SAVED_HASHES_PATH);
            processedHashes = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }

        if (processedHashes.Contains(matchHash))
        {
            Console.WriteLine($"Skipping duplicate file with hash: {matchHash}");
            return true;
        }

        processedHashes.Add(matchHash);
        string updatedJson = System.Text.Json.JsonSerializer.Serialize(processedHashes);
        File.WriteAllText(Paths.SAVED_HASHES_PATH, updatedJson);

        return false;
    }

    private static void SaveProcessedCarnage(string initialPath)
    {
        string carnagesDirectory = Path.GetDirectoryName(initialPath)
            ?? throw new NullReferenceException("Error: carnagesDirectory is null");

        string processedCarnages = Path.Combine(carnagesDirectory, "Processed");

        if (!Directory.Exists(processedCarnages))
        {
            Directory.CreateDirectory(processedCarnages);
        }

        string carnageName = Path.GetFileName(initialPath);
        string destPath = Path.Combine(processedCarnages, carnageName);

        File.Move(initialPath, destPath);
    }

}