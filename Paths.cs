using System.Runtime.InteropServices;

namespace DBStats;

public static class Paths
{
    public static readonly string BASE_PATH;

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
            throw new NotSupportedException("Operating system not supported.");
        }
    }

    public static readonly string CarnagesDir = Path.Combine(BASE_PATH!, "Halo", "Carnages");
    public static readonly string DatabaseDir = Path.Combine(BASE_PATH!, "Halo", "DBStats DataBase");

    public static readonly string SavedHashesPath = Path.Combine(
        AppContext.BaseDirectory, "Duplicates", "processed_hashes.json"
    );

    public static readonly string CustomizationPath = Path.Combine(
        BASE_PATH!, "Halo", "Discord Bot", "customization.xml"
    );
}