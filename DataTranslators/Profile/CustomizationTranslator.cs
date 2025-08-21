using DBStats.DataTypes.Profiles;
using System.Xml;

namespace DBStats.DataTranslators.Profile;

public class CustomizationTranslator
{
    public static Customization Execute(XmlNode player, string assetsPath, string playerName)
    {
        string serviceID = player.Attributes!["ServiceId"]?.Value!;
        string clanTag = player.Attributes!["ClantagText"]?.Value!;

        int nameplate = Convert.ToInt32(player.Attributes!["Nameplate"]?.Value!);

        int emblemTextureZero = Convert.ToInt32(player.Attributes!["EmblemTexture0"]?.Value!);
        int emblemTextureOne = Convert.ToInt32(player.Attributes!["EmblemTexture1"]?.Value!);

        int emblemColorZero = Convert.ToInt32(player.Attributes!["EmblemColor0"]?.Value!);
        int emblemColorOne = Convert.ToInt32(player.Attributes!["EmblemColor1"]?.Value!);
        int emblemColorTwo = Convert.ToInt32(player.Attributes!["EmblemColor2"]?.Value!);

        string nameplateImageName = AssetsMapper.GetNameplateImageName(nameplate);
        string nameplatePath = Path.Combine(assetsPath, "Nameplates", $"{nameplateImageName}.png");

        string emblemPath = ImageCreator.CreateEmblemImage(emblemTextureZero, emblemTextureOne, emblemColorZero, emblemColorOne, emblemColorTwo, assetsPath, playerName);

        return new Customization
        {
            ServiceID = serviceID,
            ClanTag = clanTag,
            NameplatePath = nameplatePath,
            EmblemPath = emblemPath
        };
    }
}