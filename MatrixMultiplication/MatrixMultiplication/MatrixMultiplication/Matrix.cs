using System.Linq;

namespace MatrixMultiplication.Models;

public class Matrix
{
    public int[,] MatrixArray { get; }

    public (int row, int column) Size => (MatrixArray.GetLength(0), MatrixArray.GetLength(1));

    public void WriteInFile(string filePath)
    {
        using var writer = new StreamWriter(filePath);

        for (int i = 0; i < Size.row; ++i)
        {
            for (int j = 0; j < Size.column; ++j)
            {
                writer.Write($"{MatrixArray[i, j]} ");
            }

            writer.Write(Environment.NewLine);
        }
    }

    public Matrix(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        string[] lines;

        try
        {
            lines = File.ReadAllLines(filePath);
        }
        catch (FileNotFoundException)
        {
            throw;
        }
        

        MatrixArray = new int[lines.Length, lines[0].Split(' ').Length];

        for (int i = 0; i < lines.Length; ++i)
        {
            var line = lines[i].Split(' ').Select(int.Parse).ToArray();

            if (line.Length != MatrixArray.GetLength(1))
            {
                throw new InvalidDataException();
            }

            for (int j = 0; i < line.Length; ++j)
            {
                MatrixArray[i,j] = line[j];
            }
        }
    }

    public Matrix(int[,] matrix)
    {
        MatrixArray = matrix;
    }

    public Matrix((int rowSize, int columnSize) size)
    {
        var random = new Random();

        MatrixArray = new int[size.rowSize, size.columnSize];

        for (int i = 0; i < size.rowSize; ++i)
        {
            for (int j = 0; j < size.columnSize; ++j)
            {
                MatrixArray[i, j] = random.Next(1024);
            }
        }
    }
}

