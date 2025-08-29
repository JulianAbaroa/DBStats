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

    public double GetScore(double timePlayed)
    {
        if (timePlayed <= 0) timePlayed = 1.0;

        const double JUGGERNAUT_WEIGHT = 1.0;

        double juggernautPerMin = JuggernautTime / timePlayed;

        double score = Math.Sqrt(juggernautPerMin) * JUGGERNAUT_WEIGHT;

        if (!double.IsFinite(score) || score < 0.0)
            score = 0.0;

        return score;
    }

}