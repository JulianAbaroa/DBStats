namespace DBStats;

public class Paths
{
    //private const string BASE_PATH = @"C:\Users\maste\OneDrive\Documents";
    private const string BASE_PATH = "media/pi/MiDiscoExterno";

    public const string CARNAGES_DIR = BASE_PATH + @"\Halo\Carnages";
    public const string DATABASE_DIR = BASE_PATH + @"\Halo\DBStats DataBase";

    public static string SAVED_HASHES_PATH = Path.Combine(AppContext.BaseDirectory, "Duplicates", "processed_hashes.json");

    public const string CUSTOMIZATION_PATH = BASE_PATH + @"\Halo\MCC Assets";
}