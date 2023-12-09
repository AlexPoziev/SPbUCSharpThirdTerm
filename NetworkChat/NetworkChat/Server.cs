using System.Net;
using System.Net.Sockets;

namespace NetworkChat;

/// <summary>
/// Provides server of chat functionality.
/// </summary>
public class Server: ChatNode
{
    private readonly TcpListener listener;

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="Server"/> is running.
    /// </summary>
    public bool IsWorking { get; private set; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/>
    /// </summary>
    /// <param name="port">port to listen to.</param>
    public Server(int port) : base(Console.In, Console.Out)
    {
        listener = new TcpListener(IPAddress.Any, port);
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class
    /// </summary>
    /// <param name="port">port to listen to.</param>
    /// <param name="stream">stream to write conversation to</param>
    public Server(int port, Stream stream) : base(stream)
    {
        listener = new TcpListener(IPAddress.Any, port);
    }
    
    /// <summary>
    /// Starts the server async.
    /// </summary>
    public async Task StartAsync()
    {
        listener.Start();

        IsWorking = true;
        
        var client = await listener.AcceptTcpClientAsync();
        var stream = client.GetStream();
        
        Writer(stream);
        Reader(stream);
        
        await Task.WhenAny(tasks);
        
        listener.Stop();
        
        IsWorking = false;
    }
    
    /// <summary>
    /// stops the server.
    /// </summary>
    public void Stop()
    {
        cancellationTokenSource.Cancel();
    }
}