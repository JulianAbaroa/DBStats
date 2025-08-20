using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class Infection : IGameMode
{
    public double SurvivalTime { get; set; }
    public int Infections { get; set; }

    public void AddStats(IGameMode other)
    {
        if (other is Infection infection)
        {
            SurvivalTime += infection.SurvivalTime;
            Infections += infection.Infections;
        }
    }

    public double GetScore()
    {
        // TODO: TESTEAR, CREO QUE TAMPOCO SE PUEDE SABER QUIEN GANO.
        return 0.0;
    }
}