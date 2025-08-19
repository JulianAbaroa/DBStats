using Microsoft.Data.Sqlite;
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

        string filePath = @"C:\Users\maste\OneDrive\Documents\Halo\Carnages\mpcarnagereport1_3495_0_0.xml";

        // This is not the final path.
        //string lastHashPath = @"C:\Users\maste\OneDrive\Documents\Halo\DBStats\last_hash.json";

        //string lastHash = File.Exists(lastHashPath) ? File.ReadAllText(lastHashPath) : string.Empty;

        // TODO: test if this actually works, depends directly on how the game generates the xml files.

        //if (CarnageDuplicateFilter.IsDuplicate(filePath, lastHash))
        //{
        //    throw new InvalidOperationException("Error: duplicated file detected, aborting.");
        //}

        //lastHash = CarnageDuplicateFilter.ComputeFileHash(filePath);
        //File.WriteAllText(lastHashPath, lastHash);

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