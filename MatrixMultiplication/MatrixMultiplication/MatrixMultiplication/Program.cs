using MatrixMultiplication;

if (args.Length < 2)
{
    Console.WriteLine("Not enough matrix files");
    return;
}

Matrix result;

try
{
    var firstMatrix = new Matrix(args[0]);
    var secondMatrix = new Matrix(args[1]);

    result = MatrixOperations.MultiplyMatrices(firstMatrix, secondMatrix);
}
catch (InvalidDataException e)
{
    Console.WriteLine(e.Message);
    return;
}
catch (FileNotFoundException)
{
    Console.WriteLine("File not found");
    return;
}

result.WriteInFile("result.txt");

Console.WriteLine("Multiplication Completed");