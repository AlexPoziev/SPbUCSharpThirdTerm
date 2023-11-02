using System.Collections.Concurrent;
using System.Net.Sockets;

namespace NetworkChat;

/// <summary>
/// Provides chat element functionality.
/// </summary>
public class ChatNode
{
    protected CancellationTokenSource cancellationTokenSource = new();
    
    protected readonly ConcurrentBag<Task> tasks = new();

    private readonly TextReader reader;
    private readonly TextWriter writer;
    private readonly Stream? internalStream;

    protected ChatNode(TextReader reader, TextWriter writer)
    {
        this.reader = reader;
        this.writer = writer;
    }
    
    protected ChatNode(Stream stream)
    {
        reader = new StreamReader(stream);
        writer = new StreamWriter(stream);
        internalStream = stream;
    }
    
    protected void Writer(NetworkStream stream)
    {
        tasks.Add(Task.Run(async () =>
        {
            var streamWriter = new StreamWriter(stream);
            
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                internalStream?.Seek(0, SeekOrigin.Begin);
                var messageToSend = await reader.ReadLineAsync(cancellationTokenSource.Token);
                if (messageToSend == null) 
                    continue;
                
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
                
                internalStream?.Seek(0, SeekOrigin.Begin);
                await writer.WriteLineAsync(receivedMessage);
                await writer.FlushAsync();
            }

            streamReader.Close();
        }, cancellationTokenSource.Token));
    }

    private void Stop()
    {
        cancellationTokenSource.Cancel();
    }
}