using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class Territories : IGameMode
{
    public int Captures { get; set; }

    public void AddStats(IGameMode other)
    {
        if (other is Territories territories)
        {
            Captures += territories.Captures;
        }
    }

    public double GetScore(double timePlayed)
    {
        if (timePlayed <= 0) timePlayed = 1.0;

        const double CAPTURE_WEIGHT = 1.0;

        double capturesPerMin = Captures / timePlayed;

        double score = Math.Sqrt(capturesPerMin) * CAPTURE_WEIGHT;

        if (!double.IsFinite(score) || score < 0.0)
            score = 0.0;

        return score;
    }

}