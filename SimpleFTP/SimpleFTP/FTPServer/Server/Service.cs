namespace SimpleFTP.Server;

using System.Text;

/// <summary>
/// Represent method container for server working.
/// </summary>
public static class Service
{
    /// <summary>
    /// Method to get list of files in directory in form: "[size: int] ([name: string] [isDir: bool))".
    /// </summary>
    /// <param name="path">path to directory.</param>
    /// <param name="stream">The stream to which the result should be passed.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
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

    /// <summary>
    /// Method to get list of files in directory in form: "[size: long] [file: byte[]]".
    /// </summary>
    /// <param name="path">path to directory.</param>
    /// <param name="stream">The stream to which the result should be passed.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
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

    /// <summary>
    /// Method to send string in byte form to stream.
    /// </summary>
    /// <param name="message">string form of message.</param>
    /// <param name="stream">stream to which the result should be passed.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task SendResponse(string message, Stream stream)
    {
        await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
        await stream.FlushAsync();
    }
}