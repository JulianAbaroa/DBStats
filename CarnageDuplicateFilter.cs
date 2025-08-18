using System.Security.Cryptography;

namespace DBStats;

public class CarnageDuplicateFilter
{
    public static string ComputeFileHash(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        byte[] hashBytes = sha256.ComputeHash(stream);
        return Convert.ToHexString(hashBytes); // hash en formato hexadecimal
    }

    public static bool IsDuplicate(string newFilePath, string lastProcessedHash)
    {
        string newHash = ComputeFileHash(newFilePath);
        return newHash == lastProcessedHash;
    }

}