namespace Lazy;

public class MultiThreadLazy<T> : ILazy<T>
{
    private readonly object lockObject = new();
    
    private volatile bool flag;

    private Func<T?>? supplier;

    private T? result;
    
    public MultiThreadLazy(Func<T> supplier)
    {
        ArgumentNullException.ThrowIfNull(this.supplier);
        
        this.supplier = supplier;
    }
    
    public T? Get()
    {
        if (flag)
        {
            return result;
        }

        lock (lockObject)
        {
            if (!flag)
            {
                flag = true;
                result = supplier!();
                supplier = default;
            }

            return result;
        }
    }
}