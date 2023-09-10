using MatrixMultiplication.Models;
using System.Diagnostics;

namespace MatrixMultiplication.Experiment;

public static class ExperimentEvaluations
{
    public static (double average, double standartDeviation) EvaluateMatrixMultiplication((int row, int column) firstMatrixSize, (int row, int column) secondMatrixSize, bool isParallel)
    {
        var launchCount = 10;
        var fractionalNumbersRound = 2;

        var firstMatrix = new Matrix(firstMatrixSize.row, firstMatrixSize.column);
        var secondMatrix = new Matrix(secondMatrixSize.row, secondMatrixSize.column);

        var results = new double[launchCount];

        var stopwatch = new Stopwatch();

        for (int i = 0; i < launchCount; ++i)
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

        var average = Math.Round(EvaluateAverage(results), fractionalNumbersRound);
        var confidenceInterval = Math.Round(EvaluateStandartDeviationOfTheMean(results), fractionalNumbersRound) * 2;

        return (average, confidenceInterval);
    }

    public static double EvaluateAverage(double[] times)
    {
        var result = 0.0;

        foreach (var time in times)
        {
            result += time;
        }

        return result / times.Length;
    }


    public static double EvaluateStandartDeviationOfTheMean(double[] times)
    {
        var result = 0.0;
        var average = EvaluateAverage(times);

        foreach (var time in times)
        {
            result += (average - time) * (average - time);
        }

        return Math.Sqrt(result / (times.Length - 1)) * (1 / Math.Sqrt(times.Length));
    
    }
}
