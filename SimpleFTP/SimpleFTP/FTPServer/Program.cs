using SimpleFTP.Server;

var server = new Server(8888);

Task.Run(() => server.Start());

while (Console.ReadKey().Key != ConsoleKey.Enter)
{
}

server.Stop();