using System.Collections.Concurrent;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using MyNUnit.Attributes;
using MyNUnit.Validators;

namespace MyNUnit.TestResults;

public class ClassTest
{
    private readonly string className;
    
    private readonly object classInstance;

    private readonly List<MethodInfo> testMethods = new();
    
    private readonly List<MethodInfo> beforeMethods = new();
    
    private readonly List<MethodInfo> afterMethods = new();
    
    private readonly List<MethodInfo> afterClassMethods = new();

    private readonly List<ValidationFailure> errors = new();

    public ClassTest(string className, TypeInfo classInfo)
    {
        this.className = className;

        var generalValidator = new TestMethodsValidator();
        var beforeAndAfterClassValidator = new BeforeAndAfterClassMethodsValidator(generalValidator);

        var methods = classInfo.GetMethods().ToList();
        
        Utils.GetMethodsWithAttributes(typeof(BeforeClassAttribute), methods)
            .ForEach(method => errors.AddRange(beforeAndAfterClassValidator.Validate(method).Errors));

        void ValidateAndPut(List<MethodInfo> putInto, IValidator<MethodInfo> validator, Type type)
        {
            putInto.AddRange(Utils.GetMethodsWithAttributes(type, methods));
            putInto.ForEach(method => errors.AddRange(validator.Validate(method).Errors));
        }
        
        ValidateAndPut(afterClassMethods, beforeAndAfterClassValidator, typeof(AfterClassAttribute));
        ValidateAndPut(testMethods, generalValidator, typeof(TestAttribute));
        ValidateAndPut(beforeMethods, generalValidator, typeof(BeforeAttribute));
        ValidateAndPut(afterMethods, generalValidator, typeof(AfterAttribute));

        classInstance = Activator.CreateInstance(classInfo) ?? throw new ArgumentNullException(nameof(classInfo));
    }
    
    
}