using System.Text;

namespace SimpleFTP.Server;

public static class Service
{
    public static string ListDirectory(string path)
    {
        const string incorrectPathResult = "-1";

        if (!Directory.Exists(path))
        {
            return incorrectPathResult;
        }

        var result = new StringBuilder();

        var directories = Directory.GetDirectories(path);
        var size = directories.Length;
            
        foreach (var directory in directories)
        {
            result.Append($" {directory} true");
        }
            
        var files = Directory.GetFiles(path);
        size += files.Length;
            
        foreach (var file in files)
        {
            result.Append($" {file} false");
        }

        result.Insert(0, $"{size}");
        result.Append('\n');

        return result.ToString();
    }

    public static async Task<string> GetFileData(string path)
    {
        const string incorrectPathResult = "-1";
        
        if (!File.Exists(path))
        {
            return incorrectPathResult;
        }

        var result = new StringBuilder();
        
        var file = await File.ReadAllBytesAsync(path);
        
        result.Append($"{file.Length} ");
        result.Append(file);

        return result.ToString();
    }
}