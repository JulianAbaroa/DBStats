using DBStats.DataTranslators.Profile;
using DBStats.DataTypes;
using System.Xml;

namespace DBStats.Parser;

public class ProfilesParser
{
    public static List<Profile> ParseProfiles(XmlNode playerNodes, string carnageName)
    {
        var profiles = new List<Profile>();

        foreach (XmlNode playerNode in playerNodes)
        {
            var profile = new Profile
            {
                PlayerID = playerNode.Attributes?["mXboxUserId"]?.Value!,
                PlayerName = playerNode.Attributes?["mGamertagText"]?.Value!,
                LastSeen = ExtractDate(carnageName),
            };

            if (profile.PlayerID == null)
            {
                throw new NullReferenceException("Error: PlayerID not found.");
            }
            else if (profile.PlayerName == null)
            {
                throw new NullReferenceException("Error: PlayerName not found.");
            }

            profile.Customization = CustomizationTranslator.Execute(playerNode, Paths.AssetsPath, profile.PlayerName);

            profiles.Add(profile);
        }

        return profiles;
    }

    private static DateTime ExtractDate(string carnageName)
    {
        try
        {
            string[] parts = carnageName.Split('_');
            string dateString = parts[2];
            string format = "yyyyMMdd";

            var invariantCulture = System.Globalization.CultureInfo.InvariantCulture;
            var dateTimeStyles = System.Globalization.DateTimeStyles.None;

            if (DateTime.TryParseExact(dateString, format, invariantCulture, dateTimeStyles, out DateTime dateObject))
            {
                return dateObject;
            }
            else
            {
                throw new InvalidOperationException("TryParseExact failed.");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"ExtractDate failed: {ex}");
        }
    }
}