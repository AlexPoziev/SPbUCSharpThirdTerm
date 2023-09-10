namespace MatrixMultiplication;

/// <summary>
/// Class for matrix structure.
/// </summary>
public class Matrix
{
    private readonly int[,] matrixArray;
    
    /// <summary>
    /// Constructor of matrix by text file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <exception cref="InvalidDataException">Not matrix in file.</exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Matrix(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        var lines = File.ReadAllLines(filePath);
        
        matrixArray = new int[lines.Length, lines[0].Split(' ').Length];

        for (var i = 0; i < lines.Length; ++i)
        {
            var line = lines[i].Split(' ').Select(int.Parse).ToArray();

            if (line.Length != matrixArray.GetLength(1))
            {
                throw new InvalidDataException("The matrix is not completely filled");
            }

            for (var j = 0; j < line.Length; ++j)
            {
                matrixArray[i, j] = line[j];
            }
        }
    }

    /// <summary>
    /// Constructor by rectangle array.
    /// </summary>
    /// <param name="matrix"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Matrix(int[,] matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        matrixArray = matrix;
    }

    /// <summary>
    /// Constructor for creating random matrices.
    /// </summary>
    /// <param name="rowSize"></param>
    /// <param name="columnSize"></param>
    public Matrix(int rowSize, int columnSize)
    {
        var random = new Random();

        matrixArray = new int[rowSize, columnSize];

        for (var i = 0; i < rowSize; ++i)
        {
            for (var j = 0; j < columnSize; ++j)
            {
                matrixArray[i, j] = random.Next(100);
            }
        }
    }

    /// <summary>
    /// Matrix indexer.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="columns"></param>
    public int this [int row, int columns]
    {
        set => matrixArray[row, columns] = value;
        get => matrixArray[row, columns];
    }

    /// <summary>
    /// Get size of the matrix.
    /// </summary>
    public (int row, int column) Size => (matrixArray.GetLength(0), matrixArray.GetLength(1));

    /// <summary>
    /// Method to write matrix to the file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void WriteInFile(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        using var writer = new StreamWriter(filePath);

        for (var i = 0; i < Size.row; ++i)
        {
            for (var j = 0; j < Size.column; ++j)
            {
                writer.Write($"{matrixArray[i, j]} ");
            }

            writer.Write(Environment.NewLine);
        }
    }
}