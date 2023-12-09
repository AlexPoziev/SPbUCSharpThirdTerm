using System.Text;
using System.Security.Cryptography;

namespace CheckSum;

/// <inheritdoc cref="ICheckSumCalculator"/>
public class CheckSumMultiThread : ICheckSumCalculator
{
    /// <summary>
    /// Calculates check sum of the directory in multi-thread
    /// </summary>
    /// <param name="path">path to directory</param>
    /// <returns>Hash value in bytes</returns>
    /// <exception cref="DirectoryNotFoundException">Throws is directory not exists</exception>
    public IEnumerable<byte> CalculateDirectoryCheckSum(string path)
    {
        if (!Path.Exists(path))
        {
            throw new DirectoryNotFoundException();
        }
        
        var files = Directory.GetFiles(path).OrderBy(p => p).ToArray();
        var directories = Directory.GetDirectories(path).OrderBy(p => p).ToArray();

        var partitionCheckSums = new byte[files.Length + directories.Length][];

        Parallel.ForEach(files, (file, state, index) =>
        {
            using var fileStream = File.Open(file, FileMode.Open);
            partitionCheckSums[index] = MD5.HashData(fileStream);
        });

        Parallel.ForEach(directories, (directory, state, index) =>
        {
            partitionCheckSums[index + files.Length] = CalculateDirectoryCheckSum(directory).ToArray();
        });

        var result = Encoding.UTF8.GetBytes(Path.GetDirectoryName(path) ?? string.Empty)
            .Concat(partitionCheckSums.SelectMany(x => x)).ToArray();

        return MD5.HashData(result);
    }
}