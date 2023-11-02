using System.Net;
using System.Net.Sockets;

namespace NetworkChat;

public class Server: ChatNode
{
    private readonly TcpListener listener;

    public bool IsWorking = false;
    
    public Server(int port) : base(Console.In, Console.Out)
    {
        listener = new TcpListener(IPAddress.Any, port);
    }
    
    public Server(int port, TextReader reader, TextWriter writer) : base(reader, writer)
    {
        listener = new TcpListener(IPAddress.Any, port);
    }
    
    public async Task Start()
    {
        listener.Start();
        
        Console.WriteLine("Server waiting for connection...");

        IsWorking = true;
        
        var client = await listener.AcceptTcpClientAsync();
        var stream = client.GetStream();
        Console.WriteLine("Connection established");
        
        Writer(stream);
        Reader(stream);
        
        await Task.WhenAny(tasks);
        
        listener.Stop();
        
        IsWorking = false;
    }
    
    public void Stop()
    {
        cancellationTokenSource.Cancel();
    }
}