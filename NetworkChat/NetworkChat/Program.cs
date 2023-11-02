using System.Net;
using NetworkChat;

switch (args.Length)
{
    case 1:
    {
        var server = new Server(int.Parse(args[0]));
        await server.Start();
        break;
    }
    case 2:
    {
        var client = new Client(IPAddress.Parse(args[0]), int.Parse(args[1]));
        await client.Start();
        break;
    }
}