using DBStats.DataTypes.Enums;
using DBStats.DataTypes.Player;
using System.Xml;

namespace DBStats.DataTranslators.Player;

public class MedalsTranslator
{
    public static Medals Execute(XmlNode player, int kills, double minutesAlive)
    {
        int totalMedals = Convert.ToInt32(player.Attributes!["mTotalMedalCount"]?.Value!);

        var medalsInfo = GetMedalsInfo(player);
        int medalsObtainedByKills = GetMedalsObtainedByKills(medalsInfo);
        double medalsPerKill = kills > 0.0d ? (double)medalsObtainedByKills / kills : 0.0d;
        double medalsPerMinute = minutesAlive > 0.0d ? totalMedals / minutesAlive : 0.0d;

        return new Medals
        {
            TotalMedals = totalMedals,
            MedalsPerKill = medalsPerKill,
            MedalsPerMinute = medalsPerMinute,
            MedalsInfo = medalsInfo,
        };
    }

    private static MedalsInfo GetMedalsInfo(XmlNode player)
    {
        var medalsInfo = new MedalsInfo();

        var medals = player.SelectSingleNode("MedalsCount")!.OfType<XmlElement>();

        foreach (var medal in medals)
        {
            string i = medal.GetAttribute("mId");
            string c = medal.GetAttribute("mCount");

            if (!int.TryParse(i, out int i2) || !int.TryParse(c, out int c2) || c2 == 0)
            {
                continue;
            }

            var type = Enum.IsDefined(typeof(MedalType), i2) ? (MedalType)i2 : MedalType.Unknown;

            medalsInfo.Add(type, c2);
        }

        return medalsInfo;
    }

    private static int GetMedalsObtainedByKills(MedalsInfo medalsInfo)
    {
        int medalsObtainedByKills = 0;

        foreach (var medalType in medalsInfo.Types)
        {
            medalsObtainedByKills += medalType.Value;
        }

        return medalsObtainedByKills;
    }

}