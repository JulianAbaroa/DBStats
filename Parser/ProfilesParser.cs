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
                LastSeen = DateTime.UtcNow,
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
}