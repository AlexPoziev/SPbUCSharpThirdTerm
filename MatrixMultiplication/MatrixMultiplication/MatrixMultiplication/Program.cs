using MatrixMultiplication.Models;
using MatrixMultiplication;
using System.Diagnostics;

if (args.Length < 2)
{
    Console.WriteLine("Not enough matrix files");
    return;
}

var firstMatrix = new Matrix(args[0]);
var secondMatrix = new Matrix(args[1]);

var stopwatch = new Stopwatch();

stopwatch.Start();

var result = MatrixOperations.MultiplyMatricesParallel(firstMatrix, secondMatrix);
result = MatrixOperations.MultiplyMatricesParallel(new Matrix(512, 512), new Matrix(512, 512));

stopwatch.Stop();
Console.WriteLine(stopwatch.ElapsedMilliseconds);

result.WriteInFile("result.txt");