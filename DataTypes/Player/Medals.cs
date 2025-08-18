using System.Text.Json.Serialization;
using DBStats.DataTypes.Enums;

namespace DBStats.DataTypes.Player;

public class Medals
{
    public int TotalMedals { get; set; }
    public double MedalsPerKill { get; set; }
    public double MedalsPerMinute { get; set; }
    public MedalsInfo MedalsInfo { get; set; } = new();
}

public class MedalsInfo
{
    private readonly Dictionary<MedalType, int> _types
        = Enum.GetValues<MedalType>().ToDictionary(mt => mt, mt => 0);

    public void Add(MedalType medalType, int value)
    {
        if (_types.ContainsKey(medalType))
        {
            _types[medalType] += value;
        }
        else
        {
            _types[medalType] = value;
        }

    }

    public static MedalsInfo operator +(MedalsInfo a, MedalsInfo b)
    {
        var result = new MedalsInfo();
        foreach (var mt in Enum.GetValues<MedalType>())
        {
            int total = a[mt] + b[mt];
            if (total > 0)
            {
                result.Add(mt, total);
            }
        }
        return result;
    }

    [JsonInclude]
    public Dictionary<MedalType, int> Types
    {
        get => _types;
        set
        {
            foreach (var kv in value)
            {
                _types[kv.Key] += kv.Value;
            }
        }
    }

    public int this[MedalType medalType] => _types.TryGetValue(medalType, out int type) ? type : 0;

}