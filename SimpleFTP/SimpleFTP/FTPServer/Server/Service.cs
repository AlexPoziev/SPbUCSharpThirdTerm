using System.Text;

namespace SimpleFTP.Server;

public static class Service
{
    public static async Task ListAsync(string path, Stream stream)
    {
        const string incorrectPathResult = "-1\n";

        if (!Directory.Exists(path))
        {
            await SendResponse(incorrectPathResult, stream);
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

        await SendResponse(result.ToString(), stream);
    }

    public static async Task GetFileAsync(string path, Stream stream)
    {
        const string incorrectPathResult = "-1 ";
        
        if (!File.Exists(path))
        {
            await SendResponse(incorrectPathResult, stream);
        }

        await SendResponse($"{new FileInfo(path).Length} ", stream);

        var file = File.OpenRead(path);

        await file.CopyToAsync(stream);
    }

    public static async Task SendResponse(string response, Stream stream)
    {
        await stream.WriteAsync(Encoding.UTF8.GetBytes(response));
        await stream.FlushAsync();
    }
}