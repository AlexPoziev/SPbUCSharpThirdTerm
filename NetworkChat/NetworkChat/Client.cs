using System.Net;
using System.Net.Sockets;

namespace NetworkChat;

/// <summary>
/// Provides client functionality for the Chat
/// </summary>
public class Client: ChatNode
{
    private readonly TcpClient client;
    private readonly IPAddress ip;
    private readonly int port;

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="Server"/> is running.
    /// </summary>
    public bool IsWorking;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class
    /// </summary>
    /// <param name="ip">ip address of server.</param>
    /// <param name="port">port of the server.</param>
    public Client(IPAddress ip, int port) : base(Console.In, Console.Out)
    {
        this.port = port;
        this.ip = ip;
        client = new TcpClient();
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class
    /// </summary>
    /// <param name="ip">ip address of server.</param>
    /// <param name="port">port of the server.</param>
    /// <param name="stream">stream to write discussion into it.</param>
    public Client(IPAddress ip, int port, Stream stream)
            : base(stream)
    {
        this.port = port;
        this.ip = ip;
        client = new TcpClient();
    }
    
    /// <summary>
    /// Starts the client async.
    /// </summary>
    public async Task StartAsync()
    {
        await client.ConnectAsync(ip, port);
        
        IsWorking = true;
        
        var stream = client.GetStream();
        
        Writer(stream);
        Reader(stream);

        await Task.WhenAny(tasks);
        
        client.Close();
        
        IsWorking = false;
    }
    
    /// <summary>
    /// Stops the client.
    /// </summary>
    public void Stop()
    {
        cancellationTokenSource.Cancel();
    }
}