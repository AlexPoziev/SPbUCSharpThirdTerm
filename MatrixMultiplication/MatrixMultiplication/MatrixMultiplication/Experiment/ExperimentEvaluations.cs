using System.Diagnostics;

namespace MatrixMultiplication.Experiment;

/// <summary>
/// Class that perform evaluation of experiment result
/// </summary>
public static class ExperimentEvaluations
{
    private static Stopwatch stopwatch = new Stopwatch();
    
    /// <summary>
    /// Method to find expectation and standard deviation of the mean
    /// </summary>
    /// <param name="firstMatrixSize">Dimensions of the first matrix</param>
    /// <param name="secondMatrixSize">Dimensions of the second matrix</param>
    /// <param name="isParallel">Which method need to check consistent -- false or parallel -- true.</param>
    /// <returns>Expectation and standard deviation of the mean</returns>
    public static (double average, double standardDeviation) EvaluateMatrixMultiplication(
        (int row, int column) firstMatrixSize, (int row, int column) secondMatrixSize, bool isParallel)
    {
        stopwatch.Reset();
        
        const int launchCount = 10;
        const int fractionalNumbersRound = 3;

        var firstMatrix = new Matrix(firstMatrixSize.row, firstMatrixSize.column);
        var secondMatrix = new Matrix(secondMatrixSize.row, secondMatrixSize.column);

        var results = new double[launchCount];

        for (var i = 0; i < launchCount; ++i)
        {
            if (isParallel)
            {
                stopwatch.Start();
                MatrixOperations.MultiplyMatricesParallel(firstMatrix, secondMatrix);
                stopwatch.Stop();
            }
            else
            {
                stopwatch.Start();
                MatrixOperations.MultiplyMatrices(firstMatrix, secondMatrix);
                stopwatch.Stop();
            }

            results[i] = stopwatch.ElapsedMilliseconds;
            stopwatch.Reset();
        }

        var average = Math.Round(EvaluateAverage(results) / 1000, fractionalNumbersRound);
        var confidenceInterval =
            Math.Round(EvaluateStandartDeviationOfTheMean(results) / 1000, fractionalNumbersRound) * 2;

        return (average, confidenceInterval);
    }

    private static double EvaluateAverage(double[] times)
    {
        var result = times.Sum();

        return result / times.Length;
    }


    private static double EvaluateStandartDeviationOfTheMean(double[] times)
    {
        var average = EvaluateAverage(times);

        var result = times.Sum(time => (average - time) * (average - time));

        return Math.Sqrt(result / (times.Length - 1)) * (1 / Math.Sqrt(times.Length));
    }
}