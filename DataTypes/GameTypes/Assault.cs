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

    public double GetPoints()
    {
        return Detonations;
    }

    public double GetScore(double minutesPlayed)
    {
        const double EXPECTED_DETONATIONS_PER_MIN = 0.05;
        const double EXPECTED_PLANTS_PER_MIN = 0.08;
        const double EXPECTED_DEFUSES_PER_MIN = 0.03;
        const double MAX_CARRY_TIME_PER_PLANT = 60.0;

        minutesPlayed = Math.Max(1e-4, minutesPlayed);

        double detPerMin = Detonations / minutesPlayed;
        double plantPerMin = BombsPlanted / minutesPlayed;
        double defusePerMin = Defuses / minutesPlayed;
        double carryPerPlant = BombsPlanted > 0 ? BombCarryTime / BombsPlanted : 0.0;

        double detScore = Math.Tanh(detPerMin / EXPECTED_DETONATIONS_PER_MIN);
        double plantScore = Math.Tanh(plantPerMin / EXPECTED_PLANTS_PER_MIN);
        double defuseScore = Math.Tanh(defusePerMin / EXPECTED_DEFUSES_PER_MIN);
        double carryScore = Math.Clamp(carryPerPlant / MAX_CARRY_TIME_PER_PLANT, 0.0, 1.0);

        double wDet = 0.55, wPlant = 0.20, wDefuse = 0.15, wCarry = 0.10;
        double combined = wDet * detScore + wPlant * plantScore + wDefuse * defuseScore + wCarry * carryScore;

        return Math.Clamp(combined, 0.0, 1.0);
    }
}