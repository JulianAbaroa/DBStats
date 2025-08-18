
public struct Combat
{
    public int Kills { get; set; }
    public double KillsPerMinute { get; set; }

    public int Deaths { get; set; }
    public double DeathsPerMinute { get; set; }

    public int Assists { get; set; }

    public int Involvements { get; set; }
    public double InvolvementsPerMinute { get; set; }

    public int ConsecutiveKills { get; set; }
    public double KDRatio { get; set; }
    public double KDARatio { get; set; }
}