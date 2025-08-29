using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class KingOfTheHill : IGameMode
{
    public double TimeinHill { get; set; }

    public void AddStats(IGameMode other)
    {
        if (other is KingOfTheHill kingOfTheHill)
        {
            TimeinHill += kingOfTheHill.TimeinHill;
        }
    }

    public double GetScore(double timePlayed)
    {
        if (timePlayed <= 0) timePlayed = 1.0;

        const double HILL_WEIGHT = 1.0;

        double hillPerMin = TimeinHill / timePlayed;

        double score = Math.Sqrt(hillPerMin) * HILL_WEIGHT;

        if (!double.IsFinite(score) || score < 0.0)
            score = 0.0;

        return score;
    }

}