using DBStats.DataTypes.Enums;

namespace DBStats.DataTypes.Dictionaries;

public static class MedalScores
{
    public static readonly Dictionary<MedalType, int> AwardPointValues = new() {
        //{ MedalType.AvegeMe,              10 }
        //{ MedalType.BrosToTheEnd,         10 },
        //{ MedalType.Extermination,        250 },
        //{ MedalType.TripleDouble,         500 },

        { MedalType.Unknown,                -1 },

        { MedalType.FirstStrike,            5 },

        { MedalType.Assist,                 5 },
        { MedalType.AssitsSpree,            10 },
        { MedalType.Sidekick,               20 },
        { MedalType.SecondGunman,           40 },

        { MedalType.Avenger,                10 },
        { MedalType.Protector,              10 },
        { MedalType.Revenge,                10 },

        { MedalType.Wheelman,               5 },
        { MedalType.WheelmanSpree,          10 },
        { MedalType.RoadHog,                20 },
        { MedalType.RoadRage,               40 },

        { MedalType.LaserKill,              5 },
        { MedalType.SniperKill,             5 },
        { MedalType.EMPKill,                5 },

        { MedalType.Yoink,                  5 },
        { MedalType.Showstopper,            20 },

        { MedalType.BullTrue,               15 },

        { MedalType.Pummel,                 5 },
        { MedalType.BeatDown,               10 },
        { MedalType.Assassin,               15 },

        { MedalType.ReloadThis,             5 },
        { MedalType.CloseCall,              5 },

        { MedalType.NeedleCombineKill,      5 },

        { MedalType.KillFromTheGrave,       15 },

        { MedalType.Firebird,               10 },
        { MedalType.Pull,                   10 },

        { MedalType.KilledFlagCarrier,      5 },
        { MedalType.FlagKill,               15 },
        { MedalType.FlagScore,              50 },

        { MedalType.Splatter,               5 },
        { MedalType.SplatterSpree,          10 },
        { MedalType.VehicularManslaughter,  20 },
        { MedalType.SundayDriver,           40 },

        { MedalType.Hijack,                 15 },
        { MedalType.Skyjack,                15 },

        { MedalType.Headshot,               5 },
        { MedalType.Headcase,               10 },

        { MedalType.Killjoy,                20 },

        { MedalType.DoubleKill,             5 },
        { MedalType.TripleKill,             10 },
        { MedalType.Overkill,               20 },
        { MedalType.Killtacular,            40 },
        { MedalType.Killtrocity,            80 },
        { MedalType.KillimanKijaro,         160 },
        { MedalType.Killtastrophe,          320 },
        { MedalType.Killpocalypse,          640 },
        { MedalType.Killionaire,            1280 },

        { MedalType.KillingSpree,           5 },
        { MedalType.KillingFrenzy,          10 },
        { MedalType.RunningRiot,            20 },
        { MedalType.Rampage,                40 },
        { MedalType.Untouchable,            80 },
        { MedalType.Invincible,             160 },
        { MedalType.Inconceivable,          320 },
        { MedalType.Unfrigginbelievable,    640 },

        { MedalType.GrenadeStick,           5 },
        { MedalType.StickSpree,             10 },
        { MedalType.StickyFingers,          20 },
        { MedalType.Corrected,              40 },

        { MedalType.SwordSpree,             5 },
        { MedalType.CuttingCrew,            10 },
        { MedalType.SliceNDice,             20 },

        { MedalType.HammerSpree,            5 },
        { MedalType.DreamCrusher,           10 },
        { MedalType.WreckingCrew,           20 },

        { MedalType.SpawnSpree,             10 },
        { MedalType.Wingman,                20 },
        { MedalType.Broseidon,              40 },

        { MedalType.ShotgunSpree,           5 },
        { MedalType.OpenSeason,             10 },
        { MedalType.BuckWild,               20 },
    };
}