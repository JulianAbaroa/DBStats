using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class KingOfTheHill : IGameMode
{
    public double TimeinHill { get; set; }

    public void AddStats(IGameMode other)
    {
        if (other is KingOfTheHill kingOfTheHill)
        {
            TimeinHill += kingOfTheHill.TimeinHill;
        }
    }

    public double GetScore()
    {
        return TimeinHill;
    }
}