using System.Runtime.InteropServices;

namespace DBStats;

public static class Paths
{
    public static readonly string BASE_PATH;
    public static readonly string CarnagesDir;
    public static readonly string DatabaseDir;
    public static readonly string SavedHashesPath;
    public static readonly string CustomizationPath;

    static Paths()
    {
        // 1) Override por variable de entorno (si quieres forzar desde la Pi/Windows)
        var envOverride = Environment.GetEnvironmentVariable("DBSTATS_BASE_PATH");
        if (!string.IsNullOrWhiteSpace(envOverride))
        {
            BASE_PATH = envOverride;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            BASE_PATH = DetectLinuxExternalDriveWithHalo() ?? Path.Combine("/home", Environment.UserName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            BASE_PATH = DetectWindowsHaloPath() ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
        else
        {
            BASE_PATH = string.Empty;
        }

        CarnagesDir = PathCombineSafe(BASE_PATH, "Halo", "Carnages");
        DatabaseDir = PathCombineSafe(BASE_PATH, "Halo", "DBStats DataBase");
        SavedHashesPath = PathCombineSafe(AppContext.BaseDirectory, "Duplicates", "processed_hashes.json");
        CustomizationPath = PathCombineSafe(BASE_PATH, "Halo", "Discord Bot", "customization.xml");
    }

    private static string? DetectLinuxExternalDriveWithHalo()
    {
        var user = Environment.UserName;
        var candidateRoots = new List<string>
        {
            Path.Combine("/media", user),
            "/media/pi",
            "/media",
            "/mnt"
        };

        foreach (var root in candidateRoots)
        {
            try
            {
                if (!Directory.Exists(root)) continue;

                if (Directory.Exists(Path.Combine(root, "Halo")))
                {
                    return root;
                }

                foreach (var sub in Directory.GetDirectories(root))
                {
                    if (Directory.Exists(Path.Combine(sub, "Halo")))
                    {
                        return sub;
                    }
                }
            }
            catch
            {
            }
        }

        return null;
    }

    private static string DetectWindowsHaloPath()
    {
        var oneDrive = Environment.GetEnvironmentVariable("OneDrive");
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (!string.IsNullOrWhiteSpace(oneDrive))
        {
            var cand = Path.Combine(oneDrive, "Documents");
            if (Directory.Exists(Path.Combine(cand, "Halo")))
                return cand;
        }

        var cand2 = Path.Combine(userProfile, "OneDrive", "Documents");
        if (Directory.Exists(Path.Combine(cand2, "Halo")))
            return cand2;

        var cand3 = Path.Combine(userProfile, "Documents");
        if (Directory.Exists(Path.Combine(cand3, "Halo")))
            return cand3;

        return userProfile;
    }

    private static string PathCombineSafe(params string[] parts)
    {
        if (parts == null || parts.Length == 0) return string.Empty;
        var valid = parts.Where(p => !string.IsNullOrEmpty(p)).ToArray();
        if (valid.Length == 0) return string.Empty;
        return Path.Combine(valid);
    }
}