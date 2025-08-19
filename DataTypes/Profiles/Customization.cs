
namespace DBStats.DataTypes.Profiles;

public struct Customization
{
    public string ServiceID { get; set; }
    public string ClanTag { get; set; }

    public int Nameplate { get; set; }

    public int EmblemTextureZero { get; set; }
    public int EmblemTextureOne { get; set; }

    public int EmblemColorZero { get; set; }
    public int EmblemColorOne { get; set; }
    public int EmblemColorTwo { get; set; }
}