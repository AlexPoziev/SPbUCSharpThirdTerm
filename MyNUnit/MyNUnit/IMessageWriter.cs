namespace MyNUnit;

public interface IMessageWriter
{
    public void WriteReport(Assembly assembly);
    
    public void WriteFullReport(IEnumerable<Assembly> assemblies);
}