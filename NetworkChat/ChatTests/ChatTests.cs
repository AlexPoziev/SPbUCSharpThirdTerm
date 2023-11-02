using System.Net;
using NetworkChat;

namespace ChatTests;

public class Tests
{
    private TextReader reader;
    private TextWriter writer;
    private Stream stream;
    private Client client;
    private Server server;
    private IPAddress ip = IPAddress.Parse("127.0.0.1");;
    private int port = 8080;
    
    [OneTimeSetUp]
    public Task Setup()
    {
        stream = new MemoryStream();
        reader = new StreamReader(stream);
        writer = new StreamWriter(stream);
        
        client = new Client(ip, port, reader, writer);
        server = new Server(port, reader, writer);
        
        client.Start();
        server.Start();
    }
    
    [OneTimeTearDown]
    

    [Test]
    public void Test1()
    {
        
    }
}