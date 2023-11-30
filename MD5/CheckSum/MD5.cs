using System.Text;
using System.Security.Cryptography;

namespace Test;

public static class MD5SingleThread
{
    public static IEnumerable<byte> GetDirectoryCheckSum(string path)
    {
        var files = Directory.GetFiles(path).OrderBy(p => p).ToArray();
        var directories = Directory.GetDirectories(path).OrderBy(p => p).ToArray();

        var filesBytes = files.Select(file =>  MD5.HashData(File.ReadAllBytes(file).ToArray())).ToList();
        var directoriesBytes = directories
            .Select(GetDirectoryCheckSum).ToList();
        
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