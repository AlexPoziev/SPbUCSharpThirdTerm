using MatrixMultiplication;

namespace MatrixMultiplicationTests;

public class MatrixMultiplicationTests
{
    [Test]
    public void ConsistentAndParallelMethodShouldToGetSameAndExpectedResult()
    {
        var firstMatrix = new Matrix("../../../TestsFiles/FirstMatrix.txt");
        var secondMatrix = new Matrix("../../../TestsFiles/SecondMatrix.txt");
        var expectedMatrix = new Matrix("../../../TestsFiles/ResultMatrix.txt");
        
        var firstResult = MatrixOperations.MultiplyMatrices(firstMatrix, secondMatrix);
        var secondResult = MatrixOperations.MultiplyMatricesParallel(firstMatrix, secondMatrix);

        Assert.That(firstResult.IsEqual(secondResult) && firstResult.IsEqual(expectedMatrix), Is.True);
    }

    [Test]
    public void NotRectangleMatrixShouldThrowInvalidDataException()
    {
        Assert.Throws<InvalidDataException>(() => new Matrix("../../../TestsFiles/FailMatrix.txt"));
    }
}
