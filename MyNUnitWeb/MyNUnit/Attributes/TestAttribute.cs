namespace MyNUnit.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    public TestAttribute(string? ignoreMessage = null, Type? expected = null)
    {
        IgnoreMessage = ignoreMessage;
        ExpectedException = expected;    
    }
    
    public TestAttribute(Type expected)
    {
        IgnoreMessage = null;
        ExpectedException = expected;    
    }
    
    public Type? ExpectedException { get; private set; }
    
    public string? IgnoreMessage { get; private set; }
}
