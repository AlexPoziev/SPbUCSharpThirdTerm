using System.Net;
using System.Net.Sockets;

namespace NetworkChat;

public class Client: ChatNode
{
    private readonly TcpClient client;
    private readonly IPAddress ip;
    private readonly int port;

    public bool IsWorking = false;
    
    public Client(IPAddress ip, int port) : base(Console.In, Console.Out)
    {
        this.port = port;
        this.ip = ip;
        client = new TcpClient();
    }
    
    public Client(IPAddress ip, int port, TextReader reader, TextWriter writer) : base(reader, writer)
    {
        this.port = port;
        this.ip = ip;
        client = new TcpClient();
    }
    
    public async Task Start()
    {
        Console.WriteLine("Connecting to server...");
        
        await client.ConnectAsync(ip, port);
        
        IsWorking = true;
        
        Console.WriteLine("Connected to server");
        
        var stream = client.GetStream();
        
        Writer(stream);
        Reader(stream);

        await Task.WhenAny(tasks);
        
        client.Close();
        
        IsWorking = false;
    }
    
    public void Stop()
    {
        cancellationTokenSource.Cancel();
    }
}