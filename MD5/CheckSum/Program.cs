using System.Diagnostics;
using CheckSum;

if (args.Length != 1)
{
    Console.WriteLine("Not only argument");
    return;
}

Console.WriteLine("MultiThread:");
var multiThreadResult = GetCheckSumResultAndWriteAverage(new CheckSumMultiThread(), new Stopwatch(), args[0]);
Console.WriteLine(BitConverter.ToString(multiThreadResult.ToArray()));

Console.WriteLine("================================================================================================");

Console.WriteLine("SingleThread:");
var singleThreadResult = GetCheckSumResultAndWriteAverage(new CheckSumSingleThread(), new Stopwatch(), args[0]);
Console.WriteLine(BitConverter.ToString(singleThreadResult.ToArray()));

Console.WriteLine("================================================================================================");

IEnumerable<byte> GetCheckSumResultAndWriteAverage(ICheckSumCalculator checkSumCalculator, Stopwatch stopwatch, string path)
{
    const int invokesCount = 10;
    var experimentTimes = new long[invokesCount];

    var result = Enumerable.Empty<byte>();
    for (var i = 0; i < invokesCount; ++i)
    {
        stopwatch.Start();
        result = checkSumCalculator.CalculateDirectoryCheckSum(path);
        stopwatch.Stop();

        experimentTimes[i] = stopwatch.ElapsedMilliseconds;
    }

    Console.WriteLine($"Mathematical Expectation: {experimentTimes.Sum() / invokesCount} ms");

    return result;
}
