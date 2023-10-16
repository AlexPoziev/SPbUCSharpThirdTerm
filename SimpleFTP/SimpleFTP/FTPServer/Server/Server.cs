using System.Net;
using System.Net.Sockets;

namespace SimpleFTP.Server;

public class Server
{
    private readonly CancellationTokenSource tokenSource;

    private readonly List<TcpClient> clients;

    private readonly TcpListener listener;
    
    public Server(int port)
    {
        SocketAlreadyUsedException.ThrowIfInUse(port);
        
        listener = new TcpListener(IPAddress.Any, port);
        clients = new();
        tokenSource = new();
    }
    
    public async Task Start()
    {
        listener.Start();

        var tasks = new List<Task>();
        
        while (!tokenSource.IsCancellationRequested)
        {
            var client = await listener.AcceptTcpClientAsync(tokenSource.Token);
            Console.WriteLine("New Connection");
            clients.Add(client);
            tasks.Add(HandleRequests(client, tokenSource.Token));
        }

        Task.WaitAll(tasks.ToArray());
        ClearAfterStop();
    }

    public void Stop()
        => tokenSource.Cancel();

    private void ClearAfterStop()
    {
        foreach (var client in clients)
        {
            client.Close();
        }

        clients.Clear();

        listener.Stop();
    }

    private Task HandleRequests(TcpClient client, CancellationToken token)
    {
        return Task.Run(async () =>
        {
            try
            {
                await using var stream = client.GetStream();
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
            }
            catch
            {
                Disconnect(client);
                Console.WriteLine("User Disconnected.");
            }
        }, token);
    }

    private void Disconnect(TcpClient client)
    {
        if (clients.Remove(client))
        {
            client.Close();
        }
    }
}