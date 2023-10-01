namespace Lazy;

/// <summary>
/// Interface for providing lazy initialization.
/// </summary>
/// <typeparam name="T">result type.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Gets the lazily initialized value.
    /// </summary>
    public T? Get();
}