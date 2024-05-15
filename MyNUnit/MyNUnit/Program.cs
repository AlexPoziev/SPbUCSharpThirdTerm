using MyNUnit;

if (args.Length != 1)
{
    Console.WriteLine("Amount of arguments should be 1");

    return;
}

var path = args[0];

try
{
    var result = TestRunner.RunTests(path);
    
    foreach (var element in result)
    {
        await Utils.PrintReportOfAssemblyTestAsync(element, Console.Out);
    }
}
catch (DirectoryNotFoundException)
{
    Console.WriteLine("Directory not found");
}
