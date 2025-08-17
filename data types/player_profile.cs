
namespace DBStats.DataTypes;

public class PlayerProfile
{
    public required string PlayerID { get; set; }
    public required string PlayerName { get; set; }
    public Customization Customization { get; set; }
    public DateTime LastSeen { get; set; }

    public List<string> MatchIDsDescending { get; set; } = [];
}