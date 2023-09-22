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
    /// <param name="filePath">path to text file with matrix</param>
    /// <exception cref="InvalidDataException">Not matrix in file.</exception>
    /// <exception cref="ArgumentNullException">Throws if <see cref="filePath"/> is null</exception>
    public Matrix(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        var lines = File.ReadAllLines(filePath);
        
        matrixArray = new int[lines.Length, lines[0].Split(' ').Length];

        for (var i = 0; i < lines.Length; ++i)
        {
            if (lines[i] == string.Empty)
            {
                throw new InvalidDataException("Empty line");
            }
            
            var line = lines[i].Trim().Split(' ').Select(int.Parse).ToArray();

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
    /// <param name="matrix">matrix in rectangle array form</param>
    /// <exception cref="ArgumentNullException">Throws if <see cref="matrix"/> is null</exception>
    public Matrix(int[,] matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        matrixArray = matrix;
    }

    /// <summary>
    /// Constructor for creating random matrices.
    /// </summary>
    /// <param name="rowSize">row size of matrix</param>
    /// <param name="columnSize">column size of matrix</param>
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
    /// <param name="row">row number of element</param>
    /// <param name="columns">column number of element</param>
    public int this [int row, int columns]
    {
        set => matrixArray[row, columns] = value;
        get => matrixArray[row, columns];
    }

    /// <summary>
    /// Get size of the matrix.
    /// </summary>
    public (int row, int column) Size 
            => (matrixArray.GetLength(0), matrixArray.GetLength(1));

    /// <summary>
    /// Method to write matrix to the file.
    /// </summary>
    /// <param name="filePath">file path to text file which contains matrix</param>
    /// <exception cref="ArgumentNullException">Throws if <see cref="filePath"/> is null</exception>
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

    /// <summary>
    /// Method to check is two matrices equal.
    /// <returns>true -- matrices are equal, else -- false</returns>
    /// <param name="matrix">Matrix that need to be checked for equality with current one.</param>
    ///  <exception cref="ArgumentNullException">Throws if <see cref="matrix"/> is null</exception>
    /// </summary>
    public bool IsEqual(Matrix matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        if (Size.row != matrix.Size.row || Size.column != matrix.Size.column)
        {
            return false;
        }

        for (var i = 0; i < Size.row; ++i)
        {
            for (var j = 0; j < Size.column; ++j)
            {
                if (matrixArray[i, j] != matrix[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }
}