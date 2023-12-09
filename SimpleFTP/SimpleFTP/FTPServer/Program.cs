using SimpleFTP.Server;

var server = new Server(7777);

var taskServer = Task.Run(async () => await server.Start());

while (Console.ReadKey().Key != ConsoleKey.Escape)
{
}

server.Stop();

await taskServer;