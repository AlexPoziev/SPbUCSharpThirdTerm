namespace MyNUnit.Assert;

public class FailedAssertion : Exception
{
    public FailedAssertion()
    {
        
    }

    public FailedAssertion(string message) : base(message)
    {
        
    }
}