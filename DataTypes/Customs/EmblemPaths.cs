
namespace DBStats.DataTypes.Customs;

public class EmblemPaths(string primary, string secondary)
{
    public string Primary { get; set; } = primary;
    public string Secondary { get; set; } = secondary;
}