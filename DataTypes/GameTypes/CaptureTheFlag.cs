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

    public double GetScore(double timePlayed)
    {
        if (timePlayed <= 0) timePlayed = 1.0;

        const double CAPTURE_WEIGHT = 1.0;
        const double RECOVER_WEIGHT = 0.5;
        const double CARRY_WEIGHT = 0.8;

        const double EXPECTED_CAPTURES_PER_MIN = 0.03;
        const double EXPECTED_RECOVERS_PER_MIN = 0.02;
        const double EXPECTED_CARRY_PER_CAPTURE_MIN = 0.5;
        const double MIN_MEAN_CARRY_FOR_VALID_CAPTURE = 0.05;

        double capturesPerMin = FlagCaptures / timePlayed;
        double recoversPerMin = FlagRecovers / timePlayed;
        double carryPerMin = FlagCarryTime / timePlayed;

        double avgCarryPerCapture = FlagCaptures > 0
            ? (FlagCarryTime / FlagCaptures)
            : carryPerMin;

        double captureRateNormalized = Math.Tanh(capturesPerMin / Math.Max(1e-6, EXPECTED_CAPTURES_PER_MIN));
        double captureQuality = Math.Clamp(avgCarryPerCapture / MIN_MEAN_CARRY_FOR_VALID_CAPTURE, 0.0, 1.0);
        double captureScore = captureRateNormalized * captureQuality;

        double recoverScore = Math.Tanh(recoversPerMin / Math.Max(1e-6, EXPECTED_RECOVERS_PER_MIN));

        double objectivePresence = capturesPerMin + 0.5 * recoversPerMin;
        double objectiveFactor = Math.Tanh(objectivePresence / Math.Max(1e-6, EXPECTED_CAPTURES_PER_MIN)); // 0..~1

        double carryQuality = Math.Clamp(avgCarryPerCapture / EXPECTED_CARRY_PER_CAPTURE_MIN, 0.0, 3.0);
        double carryQualityNorm = Math.Tanh(carryQuality);

        double carryContribution = Math.Sqrt(Math.Max(0.0, carryPerMin));
        double carryScore = carryContribution * carryQualityNorm * objectiveFactor;

        double score =
            CAPTURE_WEIGHT * captureScore +
            RECOVER_WEIGHT * recoverScore +
            CARRY_WEIGHT * carryScore;

        if (!double.IsFinite(score) || score < 0.0)
            score = 0.0;

        return score;
    }

}