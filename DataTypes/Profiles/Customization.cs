
namespace DBStats.DataTypes.Profiles;

public struct Customization
{
    public string ServiceID { get; set; }
    public string ClanTag { get; set; }

    public int Nameplate { get; set; }

    public required string NameplatePath { get; set; }
    public required string EmblemPath { get; set; }
}