using System.Net;
using System.Net.Sockets;

namespace SimpleFTP.Server;

public class Server : IDisposable
{
    private readonly CancellationTokenSource tokenSource;

    private readonly List<TcpClient> clients;

    private readonly TcpListener listener;
    
    public int GetPort { get; }
    
    public Server(int port)
    {
        SocketAlreadyUsedException.ThrowIfInUse(port);
        
        listener = new TcpListener(IPAddress.Any, port);
        GetPort = port;
        clients = new();
        tokenSource = new();
    }
    
    public async Task Start()
    {
        try
        {
            listener.Start();

            while (!tokenSource.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync();
                clients.Add(client);
                HandleRequests(client.GetStream(), tokenSource.Token);
            }
        }
        finally
        {
            Dispose();
        }
    }

    private static void HandleRequests(NetworkStream stream, CancellationToken token)
    {
        Task.Run(async () =>
        {
            await using var writer = new StreamWriter(stream);
            using var reader = new StreamReader(stream);
            
            while (!token.IsCancellationRequested)
            {
                var data = await reader.ReadLineAsync(token);

                if (data is null)
                {
                    continue;
                }

                var request = data.Split();
                var result = request[0] switch
                {
                    "2" => await Service.GetFileData(request[1]),
                    "1" => Service.ListDirectory(request[1]),
                    _ => "Request number not found"
                };

                await writer.WriteAsync(result);
                await writer.FlushAsync();
            }
        });
    }

    public void Dispose()
    {
        foreach (var client in clients)
        {
            client.Close();            
        }

        clients.Clear();
        
        listener.Stop();
    }
}