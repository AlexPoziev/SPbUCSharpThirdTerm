using System.Net;
using NetworkChat;

namespace ChatTests;

public class Tests
{
    private StreamReader clientReader;
    private StreamWriter clientWriter;
    private Stream clientStream;
    private StreamReader serverReader;
    private StreamWriter serverWriter;
    private Stream serverStream;
    private Server server;
    private Client client;
    private IPAddress ip = IPAddress.Parse("127.0.0.1");
    private int port = 8080;
    
    [OneTimeSetUp]
    public void SetupOnceTime()
    {
        clientStream = new MemoryStream();
        clientReader = new StreamReader(clientStream);
        clientWriter = new StreamWriter(clientStream);
        
        serverStream = new MemoryStream();
        serverReader = new StreamReader(clientStream);
        serverWriter = new StreamWriter(clientStream);

        server = new Server(port, serverStream);
        client = new Client(ip, port, clientStream);
        
        Task.Run(() => client.StartAsync());
        Task.Run(() => server.StartAsync());
    }

    public void Setup()
    {
        serverStream.Seek(0, SeekOrigin.Begin);
        clientStream.Seek(0, SeekOrigin.Begin);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        clientStream.Close();
        serverStream.Close();
        
        client.Stop();
        server.Stop();
    }

    [Test]
    public async Task MessageFromClientToServerShouldBeSame()
    {
        const string message = "Hello World!";
        
        await clientWriter.WriteLineAsync(message);
        await clientWriter.FlushAsync();
        
        Task.Delay(3000).Wait();
        
        serverStream.Seek(0, SeekOrigin.Begin);

        var result = await serverReader.ReadLineAsync();
        
        Assert.That(result, Is.EqualTo(message));
        
        serverStream.Seek(0, SeekOrigin.Begin);
    }
    
    [Test]
    public async Task ExitShouldStopWorkingOfServerAndClient()
    {
        var stream = new MemoryStream();
        var exitWriter = new StreamWriter(stream);
        var exitServer = new Server(port, stream);
        var exitClient = new Client(ip, port);
        
        const string message = "exit";
        
        await exitWriter.WriteLineAsync(message);
        await exitWriter.FlushAsync();
        serverStream.Seek(0, SeekOrigin.Begin);
        
        Task.Delay(3000).Wait();
        
        Assert.Multiple(() =>
        {
            Assert.That(!exitServer.IsWorking);
            Assert.That(!exitClient.IsWorking);
        });
        
        serverStream.Seek(0, SeekOrigin.Begin);
    }
}