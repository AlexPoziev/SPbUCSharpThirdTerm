using System.Security.Cryptography;
using System.Text;

namespace CheckSumTests;

using CheckSum;

public class CheckSumTests
{
    [Test]
    public void SingleAndMultiThreadShouldReturnSameValues()
    {
        const string testPath = "./TestDirectory";
        var result = new CheckSumMultiThread().CalculateDirectoryCheckSum(testPath);
        var secondResult = new CheckSumSingleThread().CalculateDirectoryCheckSum(testPath);

        Assert.That(result, Is.EqualTo(secondResult));
    }

    [TestCaseSource(nameof(CheckSumCalculators))]
    public void ChecksumShouldReturnExpectedValueWithEmptyDirectory(ICheckSumCalculator checkSumCalculator)
    {
        const string testPath = "./TestDirectory/EmptyDirectory";
        const string testFileName = "./TestDirectory/EmptyDirectory/empty.txt";
        var nameBytes = Encoding.UTF8.GetBytes(Path.GetDirectoryName(testPath) ?? string.Empty);
        var fileAndNameBytes = nameBytes.Concat(MD5.HashData(File.ReadAllBytes(testFileName)));
        var expectedResult = MD5.HashData(fileAndNameBytes.ToArray());
        
        var result = checkSumCalculator.CalculateDirectoryCheckSum(testPath);
        
        Assert.That(result, Is.EqualTo(expectedResult));
    }
    
    private static IEnumerable<ICheckSumCalculator> CheckSumCalculators()
    {
        yield return new CheckSumMultiThread();
        yield return new CheckSumSingleThread();
    }
}