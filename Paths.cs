namespace DBStats;

public static class Paths
{
#if LINUX
    const string BASE_PATH = "/media/pi/MiDiscoExterno";
#else
    const string BASE_PATH = @"C:\Users\maste\OneDrive\Documents";
#endif

    public static readonly string CarnagesDir = Path.Combine(BASE_PATH, "Halo", "Carnages");
    public static readonly string DatabaseDir = Path.Combine(BASE_PATH, "Halo", "DBStats DataBase");

    public static readonly string SavedHashesPath = Path.Combine(
        AppContext.BaseDirectory, "Duplicates", "processed_hashes.json"
    );

    public static readonly string CustomizationPath = Path.Combine(
        BASE_PATH, "Halo", "Discord Bot", "customization.xml"
    );
}