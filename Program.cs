using Microsoft.Data.Sqlite;
using DBStats.DataTranslators.Profile;
using DBStats.Duplicates;
using DBStats.DataTypes;
using DBStats.DataBase;
using DBStats.Parser;
using System.Text.Json;
using System.Xml;

namespace DBStats;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("=== DBStats START ===");

            string[] carnagePaths = Directory.GetFiles(Paths.CarnagesDir, "*.xml");

            Console.WriteLine($"Found {carnagePaths.Length} XML files to process.");

            if (carnagePaths.Length == 0)
            {
                Console.WriteLine("No XML files found. Exiting.");
                return;
            }

            try
            {
                var hashesDir = Path.GetDirectoryName(Paths.SavedHashesPath);
                if (!string.IsNullOrEmpty(hashesDir) && !Directory.Exists(hashesDir))
                {
                    Directory.CreateDirectory(hashesDir);
                    Console.WriteLine($"Created directory for saved hashes: {hashesDir}");
                }

                if (!File.Exists(Paths.SavedHashesPath))
                {
                    File.WriteAllText(Paths.SavedHashesPath, "[]");
                    Console.WriteLine($"Initialized saved hashes file at: {Paths.SavedHashesPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: could not ensure saved-hashes file/dir: {ex.Message}");
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

                AssetsMapper.LoadMaps();

                var carnageReport = new XmlDocument();
                carnageReport.Load(carnagePath);

                XmlNode playerNodes = carnageReport.SelectSingleNode("/MultiplayerCarnageReport/Players")
                    ?? throw new NullReferenceException("Error: The 'Players' node was not found in the XML.");

                Match match = CarnageReportParser.ParseMatch(carnageReport, playerNodes, carnagePath);
                List<Profile> profiles = ProfilesParser.ParseProfiles(playerNodes);

                if (!File.Exists(Paths.DatabaseDir))
                {
                    Directory.CreateDirectory(Paths.DatabaseDir);
                }

                string connectionString = $"Data Source={Path.Combine(Paths.DatabaseDir, "dbstats.db")}";

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
            throw new InvalidOperationException("Error: matchHash is not valid.");

        List<string> processedHashes = new List<string>();

        try
        {
            if (File.Exists(Paths.SavedHashesPath))
            {
                string json = File.ReadAllText(Paths.SavedHashesPath);
                var des = JsonSerializer.Deserialize<List<string>>(json);
                if (des != null)
                    processedHashes = des;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: could not read processed hashes JSON: {ex.Message}. Recreating file.");
            processedHashes = new List<string>();
        }

        if (processedHashes.Contains(matchHash))
        {
            Console.WriteLine($"Skipping duplicate file with hash: {matchHash}");
            return true;
        }

        processedHashes.Add(matchHash);

        try
        {
            string updatedJson = JsonSerializer.Serialize(processedHashes);
            // Asegurarse de que el directorio existe (doble chequeo por seguridad)
            var dir = Path.GetDirectoryName(Paths.SavedHashesPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(Paths.SavedHashesPath, updatedJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing saved hashes file: {ex.Message}");
            // No queremos fallar el procesamiento por no poder escribir el archivo,
            // así que devolvemos false (no es duplicado) y seguimos.
        }

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