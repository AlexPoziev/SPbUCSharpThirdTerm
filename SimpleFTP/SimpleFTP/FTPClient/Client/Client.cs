namespace FTPClient.Client;

using System.Net.Sockets;
using System.Text;
using Models;

/// <summary>
/// Client which support SimpleFTP protocol.
/// </summary>
public class Client
{
    private readonly int port;
    private readonly string hostName;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="port">Port for connection.</param>
    /// <param name="hostName">name of host.</param>
    public Client(int port, string hostName)
    {
        this.port = port;
        this.hostName = hostName;
    }

    /// <summary>
    /// Method to get list of all files and subdirectories of directory in path.
    /// </summary>
    /// <param name="path">Path to directory.</param>
    /// <returns>List of directory elements.</returns>
    public async Task<List<DirectoryElement>> ListAsync(string path)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(hostName, port);

        var request = $"1 {path}\n";

        var stream = client.GetStream();

        await stream.WriteAsync(Encoding.UTF8.GetBytes(request));
        await stream.FlushAsync();

        return await HandleListResponse(stream);
    }

    /// <summary>
    /// Method to get file content by it's path.
    /// </summary>
    /// <param name="path">path to file.</param>
    /// <param name="outStreamer">stream for writing result to.</param>
    /// <returns>File content.</returns>
    public async Task<byte[]> GetAsync(string path, Stream outStream)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(hostName, port);

        var request = $"2 {path}\n";

        var stream = client.GetStream();

        await using var writer = new StreamWriter(stream);

        await writer.WriteAsync(request);
        await writer.FlushAsync();

        return await HandleGetResponse(stream, outStream);
    }
    
    private static async Task<List<DirectoryElement>> HandleListResponse(NetworkStream stream)
    {
        const int bufferSize = 8192;

        var buffer = new byte[bufferSize];
        var builder = new StringBuilder();
        int bytesRead;

        do
        {
            bytesRead = await stream.ReadAsync(buffer);
            builder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
        }
        while (buffer[bytesRead - 1] != '\n');

        var response = builder.ToString().Split();

        if (response[0] == "-1")
        {
            throw new DirectoryNotFoundException();
        }

        var size = int.Parse(response[0]);

        var result = new List<DirectoryElement>();

        for (var i = 1; i <= size; ++i)
        {
            var fileName = response[(2 * i) - 1];
            var isDirectory = response[2 * i] switch
            {
                "true" => true,
                "false" => false,
                _ => throw new InvalidDataException(),
            };

            result.Add(new DirectoryElement(fileName, isDirectory));
        }

        return result.OrderBy(element => element.ElementName).ToList();
    }

    private static async Task<byte[]> HandleGetResponse(NetworkStream stream, Stream outStream)
    {
        const int bodyBufferSize = 8192;
        
        var sizeList = new List<byte>();

        int readByte;
        while ((readByte = stream.ReadByte()) != ' ')
        {
            sizeList.Add((byte)readByte);
        }

        var bodySize = int.Parse(Encoding.UTF8.GetString(sizeList.ToArray()));

        if (bodySize == -1)
        {
            throw new FileNotFoundException();
        }

        var bodyBuffer = new byte[bodyBufferSize];

        var downloadedAmount = 0;

        while (downloadedAmount < bodySize)
        {
            var charsRead = await stream.ReadAsync(bodyBuffer);

            downloadedAmount += charsRead;
            await outStream.WriteAsync(bodyBuffer.Take(charsRead).ToArray());
        }

        return Array.Empty<byte>();
    }
}