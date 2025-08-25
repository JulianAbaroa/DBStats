using System.IO;
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
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            BASE_PATH = "/media/pi/MiDiscoExterno";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            BASE_PATH = @"C:\Users\maste\OneDrive\Documents";
        }
        else
        {
            BASE_PATH = string.Empty;
        }

        CarnagesDir = Path.Combine(BASE_PATH, "Halo", "Carnages");
        DatabaseDir = Path.Combine(BASE_PATH, "Halo", "DBStats DataBase");
        SavedHashesPath = Path.Combine(
            AppContext.BaseDirectory, "Duplicates", "processed_hashes.json"
        );
        CustomizationPath = Path.Combine(BASE_PATH, "Halo", "Discord Bot", "customization.xml");
    }
}