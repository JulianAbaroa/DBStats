using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class HeadHunter : IGameMode
{
    public int MaxSkulls { get; set; }

    public void AddStats(IGameMode other)
    {
        if (other is HeadHunter headHunter)
        {
            MaxSkulls += headHunter.MaxSkulls;
        }
    }

    public double GetScore(double timePlayed)
    {
        if (timePlayed <= 0) timePlayed = 1.0;

        const double SKULL_WEIGHT = 1.0;

        double skullsPerMin = MaxSkulls / timePlayed;

        double score = Math.Sqrt(skullsPerMin) * SKULL_WEIGHT;

        if (!double.IsFinite(score) || score < 0.0)
            score = 0.0;

        return score;
    }

}