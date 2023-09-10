using MatrixMultiplication.Models;

namespace MatrixMultiplication;

public static class MatrixOperations
{
    public static Matrix MultiplyMatrices(Matrix firstMatrix, Matrix secondMatrix)
    {
        ArgumentNullException.ThrowIfNull(firstMatrix);
        ArgumentNullException.ThrowIfNull(secondMatrix);

        if (firstMatrix.Size.column != secondMatrix.Size.row)
        {
            throw new InvalidDataException();
        }

        var matrix = new int[firstMatrix.Size.row, secondMatrix.Size.column];

        for (int i = 0; i < firstMatrix.Size.row; ++i)
        {
            for (int j = 0; j < secondMatrix.Size.column; ++j)
            {
                for (int k = 0; k < firstMatrix.Size.column; ++k)
                {
                    matrix[i, j] += firstMatrix.MatrixArray[i, k] * secondMatrix.MatrixArray[k, j];
                }
            }
        }

        return new Matrix(matrix);
    }

    public static Matrix MultiplyMatricesParallel(Matrix firstMatrix, Matrix secondMatrix)
    {
        ArgumentNullException.ThrowIfNull(firstMatrix);
        ArgumentNullException.ThrowIfNull(secondMatrix);

        if (firstMatrix.Size.column != secondMatrix.Size.row)
        {
            throw new InvalidDataException("Invalid matrices sizes");
        }

        var blockSize = 16;
        var threadsCount = Environment.ProcessorCount;
        var rowsPerThread = firstMatrix.Size.row / threadsCount + 1;

        var threads = new Thread[threadsCount];
        var matrix = new int[firstMatrix.Size.row, secondMatrix.Size.column];

        for (int t = 0; t < threadsCount; ++t)
        {
            var localt = t;
            threads[t] = new Thread(() =>
            {
                for (int row = localt * rowsPerThread; row < (localt + 1) * rowsPerThread && row < firstMatrix.Size.row; ++row)
                {
                    for (int block = 0; block < secondMatrix.Size.column; block += blockSize)
                    {
                        for (int chunk = 0; chunk < firstMatrix.Size.column; chunk += blockSize)
                        {
                            for (int subChunk = 0; subChunk < blockSize && subChunk + chunk < firstMatrix.Size.column; ++subChunk)
                            {
                                for (int i = 0; i < blockSize && i + block < secondMatrix.Size.column; ++i)
                                {
                                    matrix[row, block + i] += firstMatrix.MatrixArray[row, chunk + subChunk] * secondMatrix.MatrixArray[chunk + subChunk, block + i];
                                }
                            }
                        }
                    }
                }
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return new Matrix(matrix);
    }
}

