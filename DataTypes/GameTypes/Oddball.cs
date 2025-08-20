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

    public double GetScore()
    {
        return CarryTime;
    }
}