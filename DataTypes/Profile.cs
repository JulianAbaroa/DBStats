using DBStats.DataTypes.Profiles;

namespace DBStats.DataTypes;

public class Profile
{
    public required string PlayerID { get; set; }
    public required string PlayerName { get; set; }
    public Customization Customization { get; set; }
    public DateTime LastSeen { get; set; }
}