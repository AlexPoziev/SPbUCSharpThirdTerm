namespace MyNUnit.Assert;

public static class Assert
{
    public static void That(bool condition, string? message = null)
    {
        if (!condition)
        {
            throw message is null ? new FailedAssertion() : new FailedAssertion(message);
        }
    }
}