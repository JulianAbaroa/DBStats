using DBStats.DataTypes.Customs;
using System.Xml;

namespace DBStats.DataTranslators.Profile;

public class AssetsMapper
{
    private static readonly Dictionary<int, string> _nameplateMap = [];
    private static readonly Dictionary<int, EmblemPaths> _emblemMap = [];

    public static void LoadMaps()
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(Paths.CUSTOMIZATION_PATH);

        XmlNodeList? nameplateNodes = xmlDoc.SelectNodes("//Nameplates/Nameplate")
            ?? throw new NullReferenceException("Error: nameplateNodes is null.");

        foreach (XmlNode nameplateNode in nameplateNodes)
        {
            int id = Convert.ToInt32(nameplateNode.Attributes?["id"]?.Value);
            string imageName = nameplateNode.Attributes?["image"]?.Value
                ?? throw new NullReferenceException("Error: imageName is null");

            _nameplateMap[id] = imageName;
        }

        XmlNodeList? emblemNodes = xmlDoc.SelectNodes("//Emblems/Emblem")
            ?? throw new NullReferenceException("Error: emblemNodes is null.");

        foreach (XmlNode emblemNode in emblemNodes)
        {
            int id = Convert.ToInt32(emblemNode.Attributes?["id"]?.Value);

            string primary;
            string secondary;

            if (id < 500)
            {
                primary = emblemNode.Attributes?["image1_small"]?.Value
                    ?? throw new NullReferenceException("Error: imageName is null.");

                secondary = emblemNode.Attributes?["image2_small"]?.Value
                    ?? throw new NullReferenceException("Error: imageName is null.");
            }
            else
            {
                primary = emblemNode.Attributes?["background_small"]?.Value
                    ?? throw new NullReferenceException("Error: imageName is null.");

                secondary = string.Empty;
            }

            _emblemMap[id] = new EmblemPaths(primary, secondary);
        }
    }

    public static string GetNameplateImageName(int id)
    {
        return _nameplateMap.TryGetValue(id, out string? value) ? value : throw new NullReferenceException($"Error: nameplate with the id {id} doesn't exists.");
    }

    public static EmblemPaths GetEmblemImageName(int id)
    {
        if (_emblemMap.TryGetValue(id, out EmblemPaths? value))
        {
            return value;
        }

        throw new NullReferenceException($"Error: nameplate with the id {id} doesn't exists.");
    }

}