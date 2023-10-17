using System.Text;
using FTPClient.Client;

var client = new Client(8888, "localhost");
var test = await client.ListAsync("../../..");

foreach (var directory in test)
{
    Console.WriteLine(directory);
}

var test2 = await client.GetAsync("./../../../../../../README.md");

Console.WriteLine(Encoding.UTF8.GetString(test2));


