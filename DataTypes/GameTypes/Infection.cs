using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class Infection : IGameMode
{
    public double SurvivalTime { get; set; }
    public int Infections { get; set; }

    public void AddStats(IGameMode other)
    {
        if (other is Infection infection)
        {
            SurvivalTime += infection.SurvivalTime;
            Infections += infection.Infections;
        }
    }

    public double GetScore(double timePlayed)
    {
        if (timePlayed <= 0)
        {
            timePlayed = 1.0;
        }

        const double INFECTION_WEIGHT = 1.0;
        const double SURVIVAL_WEIGHT = 0.5;

        double infectionsPerMin = Infections / timePlayed;
        double survivalPerMin = SurvivalTime / timePlayed;

        double score = Math.Sqrt(infectionsPerMin) * INFECTION_WEIGHT +
                       Math.Sqrt(survivalPerMin) * SURVIVAL_WEIGHT;

        if (!double.IsFinite(score) || score < 0.0)
            score = 0.0;

        return score;
    }

}