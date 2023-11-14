using System.Reflection;

namespace MyNUnit;

public static class Utils
{
    public static List<MethodInfo> GetMethodsWithAttributes(Type attributeType, IEnumerable<MethodInfo> methods)
        => methods.Where(method => Attribute.IsDefined(method, attributeType)).ToList();
}