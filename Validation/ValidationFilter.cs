using FluentValidation;
using ToolShare.Domain.Exceptions;

namespace ToolShare.Validation;

public class ValidationFilter<TRequest> : IEndpointFilter where TRequest : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();

        if (request is not null)
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<TRequest>>();
            if (validator is not null)
            {
                var result = await validator.ValidateAsync(request);
                if (!result.IsValid)
                {
                    var errors = result.Errors
                        .GroupBy(e => ToCamelCase(e.PropertyName))
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                    throw new RequestValidationException("One or more validation errors occurred.", errors);
                }
            }
        }

        return await next(context);
    }

    private static string ToCamelCase(string name)
        => string.IsNullOrEmpty(name) ? name : char.ToLowerInvariant(name[0]) + name[1..];
}