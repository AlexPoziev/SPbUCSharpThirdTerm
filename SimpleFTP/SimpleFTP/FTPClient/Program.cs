using System.Text;
using FTPClient.Client;

var client = new Client(7777, "localhost");
var test = await client.ListAsync("../../..");

foreach (var directory in test)
{
    Console.WriteLine(directory);
}

var test2 = await client.GetAsync("./../../../../../../README.md", Console.OpenStandardOutput());

Console.WriteLine(Encoding.UTF8.GetString(test2));
