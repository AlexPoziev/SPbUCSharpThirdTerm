namespace SimpleFTP.Server;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Server which supports SimpleFTP protocol.
/// </summary>
public class Server
{
    private readonly CancellationTokenSource tokenSource;

    private readonly List<TcpClient> clients;

    private readonly TcpListener listener;

    /// <summary>
    /// Initializes a new instance of the <see cref="Server"/> class.
    /// </summary>
    /// <param name="port">port number.</param>
    public Server(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        clients = new();
        tokenSource = new();
    }

    /// <summary>
    /// Starts work of server.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Method that stops work of server.
    /// </summary>
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
            await using var stream = client.GetStream();

            using var streamReader = new StreamReader(stream);
            
            while (!token.IsCancellationRequested)
            {
                var request = (await streamReader.ReadLineAsync(token))?.Split() ?? throw new InvalidDataException();

                switch (request[0])
                {
                    case "1":
                        await Service.ListAsync(request[1], stream);
                        break;
                    case "2":
                        await Service.GetFileAsync(request[1], stream);
                        break;
                    default:
                        await Service.SendResponse("Operation not found", stream);
                        break;
                }
            }
            
            Disconnect(client);
            Console.WriteLine("User Disconnected.");
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