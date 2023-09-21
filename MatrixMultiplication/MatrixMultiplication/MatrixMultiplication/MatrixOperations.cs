namespace MatrixMultiplication;

/// <summary>
/// Class for matrix operations.
/// </summary>
public static class MatrixOperations
{
    /// <summary>
    /// Method that perform consistent matrix multiplication.
    /// </summary>
    /// <returns>Matrix that is the result of multiplication.</returns>
    /// <exception cref="InvalidDataException">Incorrect size of matrices.</exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static Matrix MultiplyMatrices(Matrix firstMatrix, Matrix secondMatrix)
    {
        ArgumentNullException.ThrowIfNull(firstMatrix);
        ArgumentNullException.ThrowIfNull(secondMatrix);

        if (firstMatrix.Size.column != secondMatrix.Size.row)
        {
            throw new InvalidDataException();
        }

        var matrix = new int[firstMatrix.Size.row, secondMatrix.Size.column];

        for (var i = 0; i < firstMatrix.Size.row; ++i)
        {
            for (var j = 0; j < secondMatrix.Size.column; ++j)
            {
                for (var k = 0; k < firstMatrix.Size.column; ++k)
                {
                    matrix[i, j] += firstMatrix[i, k] * secondMatrix[k, j];
                }
            }
        }

        return new Matrix(matrix);
    }

    /// <summary>
    /// Method that perform parallel matrix multiplication.
    /// With using blocks to use already saved cache.
    /// </summary>
    /// <returns>Matrix that is the result of multiplication.</returns>
    /// <exception cref="InvalidDataException">Incorrect size of matrices.</exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static Matrix MultiplyMatricesParallel(Matrix firstMatrix, Matrix secondMatrix)
    {
        ArgumentNullException.ThrowIfNull(firstMatrix);
        ArgumentNullException.ThrowIfNull(secondMatrix);

        if (firstMatrix.Size.column != secondMatrix.Size.row)
        {
            throw new InvalidDataException("Invalid matrices sizes");
        }

        const int blockSize = 16;
        var threadsCount = Environment.ProcessorCount;
        var rowsPerThread = firstMatrix.Size.row / threadsCount + 1;

        var threads = new Thread[threadsCount];
        var matrix = new int[firstMatrix.Size.row, secondMatrix.Size.column];

        for (var t = 0; t < threadsCount; ++t)
        {
            var localt = t;
            threads[t] = new Thread(() =>
            {
                for (var row = localt * rowsPerThread; row < (localt + 1) * rowsPerThread && row < firstMatrix.Size.row; ++row)
                {
                    for (var block = 0; block < secondMatrix.Size.column; block += blockSize)
                    {
                        for (var chunk = 0; chunk < firstMatrix.Size.column; chunk += blockSize)
                        {
                            for (var subChunk = 0; subChunk < blockSize && subChunk + chunk < firstMatrix.Size.column; ++subChunk)
                            {
                                for (var i = 0; i < blockSize && i + block < secondMatrix.Size.column; ++i)
                                {
                                    matrix[row, block + i] += firstMatrix[row, chunk + subChunk] *
                                                              secondMatrix[chunk + subChunk, block + i];
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