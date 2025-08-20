using DBStats.Interfaces;

namespace DBStats.DataTypes.GameTypes;

public class CaptureTheFlag : IGameMode
{
    public int FlagCaptures { get; set; }
    public int FlagRecovers { get; set; }
    public double FlagCarryTime { get; set; }

    public void AddStats(IGameMode other)
    {
        if (other is CaptureTheFlag captureTheFlag)
        {
            FlagCaptures += captureTheFlag.FlagCaptures;
            FlagRecovers += captureTheFlag.FlagRecovers;
            FlagCarryTime += captureTheFlag.FlagCarryTime;
        }
    }

    public double GetScore()
    {
        return FlagCaptures;
    }
}