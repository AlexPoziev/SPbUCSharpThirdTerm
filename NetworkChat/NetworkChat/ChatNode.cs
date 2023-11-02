using System.Collections.Concurrent;
using System.Net.Sockets;

namespace NetworkChat;

public class ChatNode
{
    protected CancellationTokenSource cancellationTokenSource = new();
    
    protected readonly ConcurrentBag<Task> tasks = new();

    private TextReader reader;
    private TextWriter writer;

    protected ChatNode(TextReader reader, TextWriter writer)
    {
        this.reader = reader;
        this.writer = writer;
    }
    
    protected void Writer(NetworkStream stream)
    {
        tasks.Add(Task.Run(async () =>
        {
            var streamWriter = new StreamWriter(stream);
            
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                var messageToSend = await reader.ReadLineAsync(cancellationTokenSource.Token);
                await streamWriter.WriteLineAsync(messageToSend);
                await streamWriter.FlushAsync();
                
                if (messageToSend == "exit")
                {
                    Stop();
                }
            }

            await streamWriter.WriteLineAsync("exit");
            await streamWriter.FlushAsync();
            
            streamWriter.Close();
        }, cancellationTokenSource.Token));
    }
    
    protected void Reader(NetworkStream stream)
    {
        tasks.Add(Task.Run(async () =>
        {
            var streamReader = new StreamReader(stream);
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                var receivedMessage = await streamReader.ReadLineAsync();
                
                if (receivedMessage == "exit")
                {
                    Stop();
                    break;
                }
                
                await writer.WriteLineAsync(receivedMessage);
            }

            streamReader.Close();
        }, cancellationTokenSource.Token));
    }

    private void Stop()
    {
        cancellationTokenSource.Cancel();
    }
}