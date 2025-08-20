using Microsoft.Data.Sqlite;
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

        // Temporal path.
        string filePath = @"C:\Users\maste\OneDrive\Documents\Halo\Carnages\CarnageReport_PlaceHolderAlt_20250820_163525.xml";

        string? matchHash = MatchHasher.ComputeMatchHash(filePath);

        if (string.IsNullOrEmpty(matchHash))
        {
            throw new InvalidOperationException("Error: matchHash is not valid.");
        }

        // This is not the final path.
        string lastHashPath = @"C:\Users\maste\OneDrive\Documents\Halo\DBStats\Duplicates\last_hash.json";

        string lastHash = File.Exists(lastHashPath) ? File.ReadAllText(lastHashPath) : string.Empty;

        if (matchHash == lastHash)
        {
            throw new InvalidOperationException("Error: duplicated file detected, aborting.");
        }

        File.WriteAllText(lastHashPath, matchHash);

        var carnageReport = new XmlDocument();
        carnageReport.Load(filePath);

        XmlNode playerNodes = carnageReport.SelectSingleNode("/MultiplayerCarnageReport/Players")
            ?? throw new NullReferenceException("Error: The 'Players' node was not found in the XML.");

        Match match = CarnageReportParser.ParseMatch(carnageReport, playerNodes, filePath);
        List<Profile> profiles = ProfilesParser.ParseProfiles(playerNodes);

        // This is not the final path.
        string folderPath = @"C:\Users\maste\OneDrive\Documents\Halo\DBStats DataBase";
        Directory.CreateDirectory(folderPath);
        string connectionString = $"Data Source={Path.Combine(folderPath, "dbstats.db")}";

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using (var pragmaCMD = connection.CreateCommand())
        {
            pragmaCMD.CommandText = "PRAGMA foreign_keys = ON;";
            pragmaCMD.ExecuteNonQuery();
        }

        DataBaseInitializer.Initialize(connection);
        DataBaseInserter.Insert(connection, match, profiles);
    }

}