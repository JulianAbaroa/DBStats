using DBStats.DataTypes.Enums;
using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class UnknownGameMode : IGameMode
{
    public int Id { get; set; } = (int)GameType.Unknown;
    public string Name { get; set; } = "Unknown";

    public void AddStats(IGameMode other)
    {
        // Nothing.
    }

    public double GetScore(double minutesPlayed)
    {
        return 0.0;
    }

}