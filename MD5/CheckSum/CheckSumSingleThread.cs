using System.Text;
using System.Security.Cryptography;

namespace CheckSum;

/// <inheritdoc cref="ICheckSumCalculator"/>
public class CheckSumSingleThread : ICheckSumCalculator
{
    /// <summary>
    /// Calculates check sum of the directory in one thread
    /// </summary>
    /// <param name="path">path to directory</param>
    /// <returns>Hash value in bytes</returns>
    /// <exception cref="DirectoryNotFoundException">Throws is directory not exists</exception>
    public IEnumerable<byte> CalculateDirectoryCheckSum(string path)
    {
        if (!Path.Exists(path))
        {
            throw new DirectoryNotFoundException();
        }
        
        var files = Directory.GetFiles(path).OrderBy(p => p).ToArray();
        var directories = Directory.GetDirectories(path).OrderBy(p => p).ToArray();
        
        var filesBytes = files.Select(file =>
        {
            using var fileStream = File.Open(file, FileMode.Open);
            return MD5.HashData(fileStream);
        }).ToList();
        
        var directoriesBytes = directories
            .Select(CalculateDirectoryCheckSum).ToList();
        
        var result = Encoding.UTF8.GetBytes(Path.GetDirectoryName(path) ?? string.Empty).ToList(); 
        foreach (var bytes in filesBytes)
        {
            result.AddRange(bytes);
        }
        foreach (var bytes in directoriesBytes)
        {
            result.AddRange(bytes);
        }

        return MD5.HashData(result.ToArray());
    }
}