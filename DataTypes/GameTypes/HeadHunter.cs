using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class HeadHunter : IGameMode
{
    public int MaxSkulls { get; set; }

    public void AddStats(IGameMode other)
    {
        if (other is HeadHunter headHunter)
        {
            MaxSkulls += headHunter.MaxSkulls;
        }
    }

    public double GetScore()
    {
        // TODO: TESTEAR COMO FUNCIONA ESTE MODO DE JUEGO, PROBABLEMENTE NO SE PUEDE SABER QUIEN GANO.
        return MaxSkulls;
    }
}