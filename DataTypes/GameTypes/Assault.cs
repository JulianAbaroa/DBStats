using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class Assault : IGameMode
{
    public int BombsPlanted { get; set; }
    public int Detonations { get; set; }
    public double BombCarryTime { get; set; }
    public int Defuses { get; set; }

    public void AddStats(IGameMode other)
    {
        if (other is Assault assault)
        {
            BombsPlanted += assault.BombsPlanted;
            Detonations += assault.Detonations;
            BombCarryTime += assault.BombCarryTime;
            Defuses += assault.Defuses;
        }
    }

    public double GetScore()
    {
        return Detonations;
    }
}