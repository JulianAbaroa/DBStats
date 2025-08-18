using System.Xml;

namespace DBStats.DataTranslators.Profile;

public class CustomizationTranslator
{
    public static Customization Execute(XmlNode player)
    {
        string serviceID = player.Attributes!["ServiceId"]?.Value!;
        string clanTag = player.Attributes!["ClantagText"]?.Value!;

        int nameplate = Convert.ToInt32(player.Attributes!["Nameplate"]?.Value!);

        int emblemTextureZero = Convert.ToInt32(player.Attributes!["EmblemTexture0"]?.Value!);
        int emblemTextureOne = Convert.ToInt32(player.Attributes!["EmblemTexture1"]?.Value!);

        int emblemColorZero = Convert.ToInt32(player.Attributes!["EmblemColor0"]?.Value!);
        int emblemColorOne = Convert.ToInt32(player.Attributes!["EmblemColor1"]?.Value!);
        int emblemColorTwo = Convert.ToInt32(player.Attributes!["EmblemColor2"]?.Value!);

        return new Customization
        {
            ServiceID = serviceID,
            ClanTag = clanTag,
            Nameplate = nameplate,
            EmblemTextureZero = emblemTextureZero,
            EmblemTextureOne = emblemTextureOne,
            EmblemColorZero = emblemColorZero,
            EmblemColorOne = emblemColorOne,
            EmblemColorTwo = emblemColorTwo
        };
    }
}