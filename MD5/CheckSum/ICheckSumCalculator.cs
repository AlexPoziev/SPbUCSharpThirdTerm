namespace CheckSum;

/// <summary>
/// Provides calculation of check sum of the directory
/// </summary>
public interface ICheckSumCalculator
{
    /// <summary>
    /// Calculates check sum of the directory
    /// </summary>
    /// <param name="path">path to directory</param>
    /// <returns>Hash value in bytes</returns>
    public IEnumerable<byte> CalculateDirectoryCheckSum(string path);
}