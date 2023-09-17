namespace Lazy;

/// <summary>
/// Class for providing one thread lazy initialization.
/// </summary>
public class Lazy<T>: ILazy<T>
{
    private Exception? thrownException;
    
    private bool flag;

    private Func<T?>? supplier;

    private T? result;

    /// <summary>
    /// Constructor for one thread lazy class.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <param name="supplier">method for value initializing</param>
    public Lazy(Func<T> supplier)
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
        if (thrownException != null)
        {
            throw thrownException;
        }
        
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
        
        return result;
    }
}