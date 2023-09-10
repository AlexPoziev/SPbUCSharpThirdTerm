using MatrixMultiplication.Models;
using MatrixMultiplication;
using MatrixMultiplication.Experiment;

var test = new ((int, int), (int, int))[3] { ((256, 256), (256, 256)), ((256, 256), (256, 256)), ((256, 256), (256, 256)) };
ExperimentWriter.CreateTable(test);

//if (args.Length < 2)
//{
//    Console.WriteLine("Not enough matrix files");
//    return;
//}

//Matrix firstMatrix;
//Matrix secondMatrix;
//Matrix result;

//try
//{
//    firstMatrix = new Matrix(args[0]);
//    secondMatrix = new Matrix(args[1]);

//    result = MatrixOperations.MultiplyMatrices(firstMatrix, secondMatrix);
//}
//catch (InvalidDataException e)
//{
//    Console.WriteLine(e.Message);
//    return;
//}
//catch (FileNotFoundException)
//{
//    Console.WriteLine("File not found");
//    return;
//}

//result.WriteInFile("result.txt");

//Console.WriteLine("Multiplication Completed");