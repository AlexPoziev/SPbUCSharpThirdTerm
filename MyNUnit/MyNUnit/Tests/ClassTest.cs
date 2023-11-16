using System.Collections.Concurrent;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using MyNUnit.Attributes;
using MyNUnit.Tests.TestResults;
using MyNUnit.Validators;

namespace MyNUnit.Tests;

public class ClassTest
{
    private readonly string className;

    private readonly string assemblyName;

    private readonly List<Test> testMethods = new();

    private readonly List<MethodInfo> afterClassMethods ;

    private readonly List<ValidationFailure> errors = new();

    public ClassTest(string className, TypeInfo classInfo, string assemblyName)
    {
        this.className = className;
        this.assemblyName = assemblyName;

        var generalValidator = new TestMethodsValidator();
        var beforeAndAfterClassValidator = new BeforeAndAfterClassMethodsValidator(generalValidator);

        var methods = classInfo.GetMethods().ToList();
        
        List<MethodInfo> ValidateAndGet(IValidator<MethodInfo> validator, Type type)
        {
            var neededMethods = Utils.GetMethodsWithAttributes(type, methods);
            neededMethods.ForEach(method => errors.AddRange(validator.Validate(method).Errors));
            
            return neededMethods;
        }
        
        afterClassMethods = ValidateAndGet(beforeAndAfterClassValidator, typeof(AfterClassAttribute));
        var beforeMethods = ValidateAndGet(generalValidator, typeof(BeforeAttribute));
        var afterMethods = ValidateAndGet(generalValidator, typeof(AfterAttribute));
        var tests = ValidateAndGet(generalValidator, typeof(TestAttribute));
        var beforeClassMethods = ValidateAndGet(beforeAndAfterClassValidator, typeof(BeforeClassAttribute));
        
        if (errors.Count == 0)
        {
            Parallel.ForEach(beforeClassMethods, method =>
                method.Invoke(null, null));

            var classInstance = Activator.CreateInstance(classInfo) ?? throw new ArgumentNullException(nameof(classInfo));
            
            testMethods = tests.Select(method => new Test(classInstance, method, beforeMethods, afterMethods)).ToList();
        }
    }

    public ClassTestResult Run()
    {
        if (errors.Count != 0)
        {
            return new ClassTestResult(new List<TestResult>(), className, assemblyName, 0, errors);
        }
        
        var testResults = new ConcurrentBag<TestResult>();

        Parallel.ForEach(testMethods, testMethod => testResults.Add(testMethod.Run()));

        var result = new ClassTestResult(testResults.ToList(), className, assemblyName, testResults.Sum(r => r.TestDuration));
        
        Parallel.ForEach(afterClassMethods, method =>
            method.Invoke(null, null));

        return result;
    }
}