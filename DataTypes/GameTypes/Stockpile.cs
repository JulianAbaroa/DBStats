using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class Stockpile : IGameMode
{
    public double CarryTime { get; set; }

    public void AddStats(IGameMode other)
    {
        if (other is Stockpile stockpile)
        {
            CarryTime += stockpile.CarryTime;
        }
    }

    public double GetScore(double timePlayed)
    {
        if (timePlayed <= 0) timePlayed = 1.0;

        const double CARRY_WEIGHT = 1.0;

        double carryPerMin = CarryTime / timePlayed;

        double score = Math.Sqrt(carryPerMin) * CARRY_WEIGHT;

        if (!double.IsFinite(score) || score < 0.0)
            score = 0.0;

        return score;
    }

}