using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using FTPClient.Client.Models;

namespace FTPClient.Client;

public class Client
{
    private readonly int port;
    private readonly string hostName;
    
    public Client(int port, string hostName)
    {
        this.port = port;
        this.hostName = hostName;
    }

    public async Task<List<DirectoryElement>> ListAsync(string path)
    {
        using var client = new TcpClient(hostName, port);

        var stream = client.GetStream();
        
        await SendRequest(stream, $"1 {path}\n");
        
        return await HandleListResponse(stream);
    }

    public async Task<byte[]> GetAsync(string path)
    {
        using var client = new TcpClient(hostName, port);

        var stream = client.GetStream();
        
        await SendRequest(stream, $"2 {path}\n");

        return await HandleGetResponse(stream);
    }

    private static async Task SendRequest(NetworkStream stream, string request)
    {
        await using var writer = new StreamWriter(stream);

        await writer.WriteAsync(request);
    }

    private static async Task<List<DirectoryElement>> HandleListResponse(NetworkStream stream)
    {
        using var reader = new StreamReader(stream);
        var response = (await reader.ReadLineAsync())?.Split() ?? throw new InvalidDataContractException();
        if (response[0] == "-1")
        {
            throw new DirectoryNotFoundException();
        }

        var size = int.Parse(response[0]);

        var result = new List<DirectoryElement>();
        
        for (var i = 1; i < size; i += 2)
        {
            var fileName = response[i];
            var isDirectory = response[i + 1] switch
            {
                "true" => true,
                "false" => false,
                _ => throw new InvalidDataException(),
            };
            
            result.Add(new DirectoryElement(fileName, isDirectory));
        }

        return result;
    }

    private static async Task<byte[]> HandleGetResponse(NetworkStream stream)
    {
        using var reader = new StreamReader(stream);

        var bufferForSize = new char[1];
        var builder = new StringBuilder();

        while (bufferForSize[0] != ' ')
        {
            builder.Append(bufferForSize[0]);
            await reader.ReadAsync(bufferForSize, 0, bufferForSize.Length);
        }

        const int bodyBufferSize = 8;
        var bodyBuffer = new char[bodyBufferSize];
        
        var bodySize = int.Parse(builder.ToString());
        builder.Clear();
        var downloadedAmount = 0;

        while (downloadedAmount < bodySize)
        {
            var charsRead = await reader.ReadAsync(bodyBuffer, 0, bodyBufferSize);
            
            downloadedAmount += charsRead;
            builder.Append(bodyBuffer);
        }

        return Encoding.UTF8.GetBytes(builder.ToString());
    }
}