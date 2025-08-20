using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class Juggernaut : IGameMode
{
    public double JuggernautTime { get; set; }

    public void AddStats(IGameMode other)
    {
        if (other is Juggernaut juggernaut)
        {
            JuggernautTime += juggernaut.JuggernautTime;
        }
    }

    public double GetScore()
    {
        return JuggernautTime;
    }
}