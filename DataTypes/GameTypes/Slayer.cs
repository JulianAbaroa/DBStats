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

    public double GetScore()
    {
        return Rating;
    }
}