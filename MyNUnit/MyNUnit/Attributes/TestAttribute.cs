namespace MyNUnit.Attributes;

public class TestAttribute : Attribute
{
    public TestAttribute(string? ignoreMessage = null, Exception? expected = null)
    {
        IgnoreMessage = ignoreMessage;
        ExpectedException = expected;    
    }
    
    public Exception? ExpectedException { get; private set; }
    
    public string? IgnoreMessage { get; private set; }
}
