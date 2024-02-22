namespace MyNUnit.Assert;

public class FailedAssertionException : Exception
{
    public FailedAssertionException()
    {
        
    }

    public FailedAssertionException(string message) : base(message)
    {
        
    }
}