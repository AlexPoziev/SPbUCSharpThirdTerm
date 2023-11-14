namespace MyNUnitTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.That("1", Is.EqualTo("ewkere"));
    }
}