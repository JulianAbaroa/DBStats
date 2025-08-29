using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class Slayer : IGameMode
{
    public double Rating { get; set; }

    public void AddStats(IGameMode other)
    {
        if (other is Slayer slayer)
        {
            Rating += slayer.Rating;
        }
    }

    public double GetScore(double timePlayed)
    {
        if (timePlayed <= 0) timePlayed = 1.0;

        const double SLAYER_WEIGHT = 1.0;

        double ratingPerMin = Rating / timePlayed;

        double score = Math.Sqrt(ratingPerMin) * SLAYER_WEIGHT;

        if (!double.IsFinite(score) || score < 0.0)
            score = 0.0;

        return score;
    }

}