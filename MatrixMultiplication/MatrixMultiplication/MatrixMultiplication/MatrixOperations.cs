using MatrixMultiplication.Models;

namespace MatrixMultiplication;

public static class MatrixOperations
{
    public static Matrix MatricesMultiplication(Matrix firstMatrix, Matrix secondMatrix)
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
                for (int k = 0; k < firstMatrix.Size.row; ++k)
                {
                    matrix[i, j] += firstMatrix.MatrixArray[i, k] * secondMatrix.MatrixArray[k, j];
                }
            }
        }

        return new Matrix(matrix);
    }
}

