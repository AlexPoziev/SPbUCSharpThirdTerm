using System.Text;
using System.Security.Cryptography;

public static class MD5MultiThread
{
    public static byte[] GetDirectoryCheckSum(string path)
    {
        var files = Directory.GetFiles(path).OrderBy(p => p).ToArray();
        var directories = Directory.GetDirectories(path).OrderBy(p => p).ToArray();

        var partitionCheckSums = new byte[][files.Length + directories.Length];

        Parallel.ForEach(files, (file, state, index) =>
        {
            partitionCheckSums[index] = MD5.HashData(File.ReadAllBytes(file));
        });

        Parallel.ForEach(directories, (directory, state, index) =>
        {
            partitionCheckSums[index + files.Length] = GetDirectoryCheckSum(directory);
        });

        var result = Encoding.UTF8.GetBytes(Path.GetDirectoryName(path) ?? string.Empty)
            .Concat(partitionCheckSums.SelectMany(x => x)).ToArray();

        return MD5.HashData(result);
    }
}