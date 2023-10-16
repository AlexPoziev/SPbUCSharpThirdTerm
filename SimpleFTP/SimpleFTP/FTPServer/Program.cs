using SimpleFTP.Server;

var server = new Server(8888);

await server.Start();