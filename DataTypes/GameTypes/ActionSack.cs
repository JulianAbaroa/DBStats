using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class ActionSack : IGameMode
{
    public void AddStats(IGameMode other) { }
    public double GetPoints() { return 0.0; }
    public double GetScore(double timePlayed) { return 0.0; }
}