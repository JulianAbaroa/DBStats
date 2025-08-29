using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class Oddball : IGameMode
{
    public double CarryTime { get; set; }
    public int BallKills { get; set; }

    public void AddStats(IGameMode other)
    {
        if (other is Oddball oddball)
        {
            CarryTime += oddball.CarryTime;
            BallKills += oddball.BallKills;
        }
    }

    public double GetScore(double timePlayed)
    {
        if (timePlayed <= 0) timePlayed = 1.0;

        const double CARRY_WEIGHT = 1.0;
        const double KILL_WEIGHT = 0.5;

        double carryPerMin = CarryTime / timePlayed;
        double killsPerMin = BallKills / timePlayed;

        double score = Math.Sqrt(carryPerMin) * CARRY_WEIGHT +
                       Math.Sqrt(killsPerMin) * KILL_WEIGHT;

        if (!double.IsFinite(score) || score < 0.0)
            score = 0.0;

        return score;
    }

}