using System.Reflection;
using FluentValidation;

namespace MyNUnit.Validators;

public class TestMethodsValidator : AbstractValidator<MethodInfo>
{
    public TestMethodsValidator()
    {
        RuleFor(tm => tm.ReturnType)
            .Equal(typeof(void))
            .WithMessage(tm => $"Method: '{tm.Name}' should not have void return type.");

        RuleFor(tm => tm.GetParameters())
            .Empty()
            .WithMessage(tm => $"Method: '{tm.Name}' should not have parameters.");
    }
}