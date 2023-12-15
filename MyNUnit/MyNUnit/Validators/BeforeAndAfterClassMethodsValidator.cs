using System.Reflection;
using FluentValidation;

namespace MyNUnit.Validators;

public class BeforeAndAfterClassMethodsValidator : AbstractValidator<MethodInfo>
{
    public BeforeAndAfterClassMethodsValidator(TestMethodsValidator testMethodsValidator)
    {
        RuleFor(tm => tm.IsStatic)
            .Equal(true)
            .WithMessage(tm => $"Method: '{tm.Name}' should be static");
        
        Include(testMethodsValidator);
    }
}