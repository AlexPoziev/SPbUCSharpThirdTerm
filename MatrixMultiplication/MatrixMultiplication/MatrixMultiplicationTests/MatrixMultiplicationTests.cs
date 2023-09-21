using MatrixMultiplication;

namespace MatrixMultiplicationTests;

public class MatrixMultiplicationTests
{
    [Test]
    public void ConsistentAndParallelMethodShouldToGetSameAndExpectedResult()
    {
        var firstMatrix = new Matrix("FirstMatrix.txt");
        var secondMatrix = new Matrix("SecondMatrix.txt");
        var expectedMatrix = new Matrix("ResultMatrix.txt");
        
        var firstResult = MatrixOperations.MultiplyMatrices(firstMatrix, secondMatrix);
        var secondResult = MatrixOperations.MultiplyMatricesParallel(firstMatrix, secondMatrix);

        Assert.That(firstResult.IsEqual(secondResult) && firstResult.IsEqual(expectedMatrix), Is.True);
    }

    [Test]
    public void NotRectangleMatrixShouldThrowInvalidDataException()
    {
        Assert.Throws<InvalidDataException>(() => new Matrix("FailMatrix.txt"));
    }
}
