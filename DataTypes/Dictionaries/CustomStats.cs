
namespace DBStats.DataTypes.Dictionaries;

public static class CustomStats
{
    public static readonly Dictionary<GameType, List<string>> GameTypeStats = new()
    {
        // Slayer:
        { GameType.Slayer, new List<string> { "RATING" } },

        // Capture The Flag:
        { GameType.CaptureTheFlag, new List<string> { "Flag Captures", "Flag Carry Time", "Flag Returns" } },

        // Oddball:
        { GameType.Oddball, new List<string> { "CARRY TIME", "BALL KILLS" } },

        // KingOfTheHill:
        { GameType.KingOfTheHill, new List<string> { "Time in Hill" } },

        // Juggernaut:
        { GameType.Juggernaut, new List<string> { "Juggernaut Time" } },

        // Infection:
        { GameType.Infection, new List<string> { "Survival Time", "Infections" } },

        // Territories:
        { GameType.Territories, new List<string> { "Captures" } },

        // Assault:
        { GameType.Assault, new List<string> { "Bombs Planted", "Detonations", "Bomb Carry Time", "Defuses" } },

        // Stockpile:
        { GameType.Stockpile, new List<string> { "CARRY TIME" } },

        // HeadHunter:
        { GameType.HeadHunter, new List<string> { "MAX SKULLS" } },

        // ActionSack:
        { GameType.ActionSack, new List<string> { "" } },
    };
}