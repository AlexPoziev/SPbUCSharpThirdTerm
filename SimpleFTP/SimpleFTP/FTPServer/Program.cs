using SimpleFTP.Server;

var server = new Server(7777);

await server.Start();