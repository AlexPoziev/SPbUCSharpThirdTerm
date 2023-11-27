using System.Diagnostics;
using System.Runtime.InteropServices;
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

    private const string testFileName = "Test.txt";

    [OneTimeSetUp]
    public void Setup()
    { 
        Task.Run(() => server.Start());
    }

    [Test]
    public async Task ListShouldReturnExpectedValue()
    {
        const string path = "./TestFiles";
        var expectedResult = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "DirectoryElement { ElementName = ./TestFiles\\HelloWorld.exe, IsDirectory = False } DirectoryElement { ElementName = ./TestFiles\\TestDirectory, IsDirectory = True } DirectoryElement { ElementName = ./TestFiles\\TextDoc.txt, IsDirectory = False }"
            : "DirectoryElement { ElementName = ./TestFiles/HelloWorld.exe, IsDirectory = False } DirectoryElement { ElementName = ./TestFiles/TestDirectory, IsDirectory = True } DirectoryElement { ElementName = ./TestFiles/TextDoc.txt, IsDirectory = False }";
        
        var result = string.Join(' ', (await client.ListAsync(path)).Select(element => element.ToString()));
        
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task GetShouldReturnExpectedValue()
    {
        const string path = "./TestFiles/HelloWorld.exe";
        var expectedResult = await File.ReadAllBytesAsync(path);
        
        var result = await GetResultAsync(path);
        
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
        const int clientsNumber = 5;
        const int millisecondsWait = 2000;
        
        var listResults = new List<DirectoryElement>[clientsNumber];
        var getResults = new byte[clientsNumber][];
        var tasks = new Task[clientsNumber];
        var manualResetEvent = new ManualResetEvent(false);
        
        for (var i = 0; i < clientsNumber; ++i)
        {
            var locali = i;
            tasks[i] = Task.Run(async () =>
            {
                manualResetEvent.WaitOne();
                
                await Task.Delay(millisecondsWait);
                
                var newClient = new Client(port, host);
                listResults[locali] = await newClient.ListAsync(listPath);
                getResults[locali] = await GetResultAsync(getPath);
            });
        }
        
        var stopwatch = new Stopwatch();
        
        manualResetEvent.Set();
        stopwatch.Start();
        
        Task.WaitAll(tasks);
        stopwatch.Stop();
        
        Console.WriteLine(stopwatch.ElapsedMilliseconds);
        
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(millisecondsWait * clientsNumber));
        
        for (var i = 1; i < clientsNumber; ++i)
        {
            Assert.Multiple(() =>
            {
                Assert.That(listResults[i - 1], Is.EqualTo(listResults[i]));
                Assert.That(getResults[i - 1], Is.EqualTo(getResults[i]));
            });
        }
    }
    
    private async Task<byte[]> GetResultAsync(string path)
    {
        using var stream = new MemoryStream();
        
        await client.GetAsync(path, stream);
        
        return stream.ToArray();
    }
}