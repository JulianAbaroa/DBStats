using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class Territories : IGameMode
{
    public int Captures { get; set; }

    public void AddStats(IGameMode other)
    {
        if (other is Territories territories)
        {
            Captures += territories.Captures;
        }
    }

    public double GetScore()
    {
        // TODO: CREO QUE ESTE VALOR NO IMPLICA UNA VICTORIA NECESARIAMENTE.
        return Captures;
    }
}