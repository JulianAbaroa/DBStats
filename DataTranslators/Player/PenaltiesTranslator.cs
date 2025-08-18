using System.Xml;

namespace DBStats.DataTranslators.Player;

public class PenaltiesTranslator
{
    public static Penalties Execute(XmlNode player, int kills, int deaths)
    {
        int suicides = Convert.ToInt32(player.Attributes!["mSuicides"]?.Value!);
        int betrayals = Convert.ToInt32(player.Attributes!["mBetrayals"]?.Value!);

        double suicidesPerDeaths = deaths > 0.0d ? (double)suicides / deaths : 0.0d;
        double betrayalsPerKill = kills > 0.0d ? (double)betrayals / kills : 0.0d;

        return new Penalties
        {
            Suicides = suicides,
            Betrayals = betrayals,
            SuicidesPerDeath = suicidesPerDeaths,
            BetrayalsPerKill = betrayalsPerKill,
        };
    }

}