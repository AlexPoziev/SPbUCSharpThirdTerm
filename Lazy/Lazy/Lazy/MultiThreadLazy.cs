namespace Lazy;

/// <summary>
/// Interface for providing multi thread lazy initialization.
/// </summary>
public class MultiThreadLazy<T> : ILazy<T>
{
    private Exception? thrownException;
    
    private readonly object lockObject = new();
    
    private volatile bool isResultCalculated;

    private Func<T?>? supplier;

    private T? result;
    
    /// <summary>
    /// Constructor for multi thread lazy class.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <param name="supplier">method for value initializing</param>
    public MultiThreadLazy(Func<T> supplier)
    {
        ArgumentNullException.ThrowIfNull(supplier);
        
        this.supplier = supplier;
    }
    
    /// <summary>
    /// Gets the lazily initialized value.
    /// </summary>
    /// <exception cref="AggregateException">exception of supplied function.</exception>
    public T? Get()
    {
        if (!isResultCalculated)
        {
            lock (lockObject)
            {
                if (!isResultCalculated)
                {
                    try
                    {
                        result = supplier!();
                    }
                    catch (Exception e)
                    {
                        thrownException = e;
                    }
                    finally
                    {
                        supplier = default;
                        isResultCalculated = true;
                    }
                }
            }
        }
        
        if (thrownException != default)
        {
            throw new AggregateException(thrownException);
        }

        return result;
    }
}