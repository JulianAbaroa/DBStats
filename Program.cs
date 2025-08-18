using Microsoft.Data.Sqlite;
using DBStats.DataTypes;

// TODO: FILTRADO DE ARCHIVOS DUPLICADOS.

namespace DBStats;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        string filePath = args[0];

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Error: El archivo no se encuentra en la ruta: {filePath}");
        }

        Match match = CarnageReportParser.ParseMatch(filePath);

        string folderPath = @"C:\Users\maste\OneDrive\Documents\Halo\DBStats DataBase";
        Directory.CreateDirectory(folderPath);
        string connectionString = $"Data Source={Path.Combine(folderPath, "dbstats.db")}";

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        DataBaseInitializer.Initialize(connection);

        // TODO: insertar los datos en las tablas, habilitar 'PRAGMA foreign_keys', hacer metodos para esto.
    }

}