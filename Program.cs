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

            // TODO: TEMPORAL PATH.
            const string CARNAGES_PATH = @"C:\Users\maste\OneDrive\Documents\Halo\Carnages";

            string[] carnagePaths = Directory.GetFiles(CARNAGES_PATH, "*.xml");

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
                string dataBasePath = @"C:\Users\maste\OneDrive\Documents\Halo\DBStats DataBase";

                if (!File.Exists(dataBasePath))
                {
                    Directory.CreateDirectory(dataBasePath);
                }

                string connectionString = $"Data Source={Path.Combine(dataBasePath, "dbstats.db")}";

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

    private static void CheckForDuplicates(string carnagePath)
    {
        string? matchHash = MatchHasher.ComputeMatchHash(carnagePath);

        if (string.IsNullOrEmpty(matchHash))
        {
            throw new InvalidOperationException("Error: matchHash is not valid.");
        }

        // TODO: TEMPORAL PATH.
        string processedHashesPath = @"C:\Users\maste\OneDrive\Documents\Halo\DBStats\Duplicates\processed_hashes.json";

        List<string> processedHashes = [];
        if (File.Exists(processedHashesPath))
        {
            string json = File.ReadAllText(processedHashesPath);
            processedHashes = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }

        if (processedHashes.Contains(matchHash))
        {
            Console.WriteLine($"Skipping duplicate file with hash: {matchHash}");
            return;
        }

        processedHashes.Add(matchHash);
        string updatedJson = System.Text.Json.JsonSerializer.Serialize(processedHashes);
        File.WriteAllText(processedHashesPath, updatedJson);
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