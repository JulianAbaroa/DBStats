using System.Text.RegularExpressions;
using System.Xml;

public class ChoiceTranslator
{
    public static Choice Execute(XmlNode player, int weaponKills)
    {
        int mostUsedWeaponID = Convert.ToInt32(player.Attributes!["mMostUsedWeapon"]?.Value!);
        int mostUsedWeaponKills = Convert.ToInt32(player.Attributes!["mMostUsedWeaponCount"]?.Value!);

        string mostUsedWeapon = GetMostUsedWeapon(mostUsedWeaponID);

        double mostUsedWeaponKillsRatio = weaponKills > 0.0d ? (double)mostUsedWeaponKills / weaponKills : 0.0d;

        return new Choice
        {
            MostUsedWeapon = mostUsedWeapon,
            MostUsedWeaponKills = mostUsedWeaponKills,
            MostUsedWeaponKillsRatio = mostUsedWeaponKillsRatio,
        };
    }

    private static string GetMostUsedWeapon(int mostUsedWeaponID)
    {
        var mostUsedWeapon = WeaponType.Unknown;

        if (int.TryParse(mostUsedWeaponID.ToString(), out var mostUsedWeaponValue)
            && Enum.IsDefined(typeof(WeaponType), mostUsedWeaponValue))
        {
            mostUsedWeapon = (WeaponType)mostUsedWeaponValue;
        }

        return Regex.Replace(
            mostUsedWeapon.ToString(),
            @"(?<=[a-z])(?=(?:[A-Z][a-z]|[A-Z]{2,}))",
            " "
        );
    }

}