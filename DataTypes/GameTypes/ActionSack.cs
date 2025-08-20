using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class ActionSack : IGameMode
{
    public void AddStats(IGameMode other) { }
    public double GetScore() { return 0.0; }
}