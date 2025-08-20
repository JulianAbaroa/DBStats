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

    public double GetScore()
    {
        return CarryTime;
    }
}