namespace Lazy;

public class Lazy<T>: ILazy<T>
{
    private bool flag;

    private Func<T?>? supplier;

    private T? result;
    
    public Lazy(Func<T> supplier)
    {
        ArgumentNullException.ThrowIfNull(this.supplier);
        
        this.supplier = supplier;
    }
    
    public T? Get()
    {
        if (!flag)
        {
            result = supplier!();
            supplier = null;
            flag = true;
        }

        return result;
    }
}