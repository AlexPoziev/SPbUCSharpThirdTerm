namespace Lazy;

/// <summary>
/// Interface for providing multi thread lazy initialization.
/// </summary>
public class MultiThreadLazy<T> : ILazy<T>
{
    private Exception? thrownException;
    
    private readonly object lockObject = new();
    
    private volatile bool flag;

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
    /// <exception cref="Exception">exception of supplied function.</exception>
    public T? Get()
    {
        if (flag)
        {
            if (thrownException != default)
            {
                throw thrownException;
            }
            
            return result;
        }

        lock (lockObject)
        {
            if (!flag)
            {
                try
                {
                    result = supplier!();
                }
                catch (Exception e)
                { 
                    thrownException = e;
                    throw;
                }
                finally
                {
                    supplier = default;
                    flag = true;
                }
            }
            
            if (thrownException != default)
            {
                throw thrownException;
            }

            return result;
        }
    }
}