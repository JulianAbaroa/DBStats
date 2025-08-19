using DBStats.DataTranslators.Profile;
using DBStats.DataTypes;
using System.Xml;

namespace DBStats.Parser;

public class ProfilesParser
{
    public static List<Profile> ParseProfiles(XmlNode playerNodes)
    {
        var profiles = new List<Profile>();

        foreach (XmlNode playerNode in playerNodes)
        {
            var profile = new Profile
            {
                PlayerID = playerNode.Attributes?["mXboxUserId"]?.Value!,
                PlayerName = playerNode.Attributes?["mGamertagText"]?.Value!,
                Customization = CustomizationTranslator.Execute(playerNode),
                LastSeen = DateTime.UtcNow,
            };

            if (profile.PlayerID == null)
            {
                Console.WriteLine("Error: PlayerID not found.");
            }
            else if (profile.PlayerName == null)
            {
                Console.WriteLine("Error: PlayerName not found.");
            }

            profiles.Add(profile);
        }

        return profiles;
    }
}