using MyNUnit;

const string path = "./";

var result = TestRunner.RunTests(path);

foreach (var element in result)
{
    await Utils.PrintReportOfAssemblyTestAsync(element, Console.Out);
}
