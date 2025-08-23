using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace DBStats.Duplicates;

public class MatchHasher
{
    // This method extracts consistent match data and returns a hash
    public static string ComputeMatchHash(string filePath)
    {
        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            // Find the map name
            string? mapName = xmlDoc.SelectSingleNode("//GameTypeName")?.Attributes?["GameTypeName"]?.Value;

            if (string.IsNullOrEmpty(mapName))
            {
                throw new InvalidOperationException("MapName not found in XML.");
            }

            // Find all player gamertags
            var players = new List<string>();
            XmlNodeList? playerNodes = xmlDoc.SelectNodes("/MultiplayerCarnageReport/Players/Player");

            if (playerNodes is not null)
            {
                foreach (XmlNode playerNode in playerNodes)
                {
                    XmlAttribute? gamertagAttr = playerNode.Attributes?["mGamertagText"];

                    if (gamertagAttr is not null && !string.IsNullOrEmpty(gamertagAttr.Value))
                    {
                        players.Add(gamertagAttr.Value);
                    }
                }
            }

            // Sort the gamertags to ensure consistent order
            players.Sort();

            // Combine the data into a single string
            string dataToHash = $"{mapName}|{string.Join(",", players)}";

            // Compute the SHA256 hash of the combined string
            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
            return Convert.ToHexString(hashBytes);
        }
        catch (Exception ex)
        {
            // Handle any XML parsing or file errors here
            throw new InvalidOperationException($"Error: match hasher failed with {ex}");
        }
    }

}