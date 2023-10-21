using FTPClient.Client;
using FTPClient.Client.Models;
using SimpleFTP.Server;

namespace SimpleFTPTests;

public class SimpleFtpTests
{
    private const int port = 7777;

    private const string host = "localhost";
    
    private readonly Server server = new(port);

    private readonly Client client = new(port, host);

    [OneTimeSetUp]
    public void Setup()
    { 
        Task.Run(() => server.Start());
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        server.Stop();
    }

    [Test]
    public async Task ListShouldReturnExpectedValue()
    {
        const string path = "./TestFiles";
        const string expectedResult = "DirectoryElement { ElementName = ./TestFiles/TestDirectory, IsDirectory = True } DirectoryElement { ElementName = ./TestFiles/TextDoc.txt, IsDirectory = False } DirectoryElement { ElementName = ./TestFiles/HelloWorld.exe, IsDirectory = False }";
        
        var result = string.Join(' ', (await client.ListAsync(path)).Select(element => element.ToString()));
        
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task GetShouldReturnExpectedValue()
    {
        const string path = "./TestFiles/HelloWorld.exe";
        var expectedResult = await File.ReadAllBytesAsync(path);
        
        var result = await client.GetAsync(path);
        
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    
    [Test]
    public void NonExistentDirectoryForListTest()
    {
        var path = "./TestFilessss";
        
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await client.ListAsync(path));
    }

    
    [Test]
    public void ManyClientsShouldReturnSameResult()
    {
        const string listPath = "./TestFiles";
        const string getPath = "./TestFiles/TextDoc.txt";
        const int clientsNumber = 10;

        var listResults = new List<DirectoryElement>[clientsNumber];
        var getResults = new byte[clientsNumber][];
        var tasks = new Task[clientsNumber];

        for (var i = 0; i < clientsNumber; ++i)
        {
            var locali = i;
            tasks[i] = Task.Run(async () =>
            {
                var newClient = new Client(port, host);
                listResults[locali] = await newClient.ListAsync(listPath);
                getResults[locali] = await newClient.GetAsync(getPath);
            });
        }

        Task.WaitAll(tasks);

        for (var i = 1; i < clientsNumber; ++i)
        {
            Assert.Multiple(() =>
            {
                Assert.That(listResults[i - 1], Is.EqualTo(listResults[i]));
                Assert.That(getResults[i - 1], Is.EqualTo(getResults[i]));
            });
        }
    }
}