using FluentValidation;
using MediatR;

namespace PaymentAPI.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that validates requests using FluentValidation.
/// </summary>
/// <typeparam name="TRequest">The type of the request being validated.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <remarks>
/// This behavior intercepts all MediatR requests and runs FluentValidation validators before the handler executes.
/// If validation fails, it returns an error response without executing the handler.
/// </remarks>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validators">Collection of FluentValidation validators for the request type.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Handles the request by running all validators before passing to the next handler.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="next">The next handler in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// If validation succeeds, returns the result from the next handler.
    /// If validation fails, returns an error response with all validation messages.
    /// </returns>
    /// <exception cref="ValidationException">Thrown if validation fails and the response type doesn't have a Fail method.</exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                // Build readable error message
                var errorMessage = string.Join(", ", failures.Select(f => f.ErrorMessage));

                // ApiResponse.Fail() expects generic type → we need reflection
                var failMethod = typeof(TResponse).GetMethod("Fail");
                if (failMethod != null)
                {
                    return (TResponse)failMethod.Invoke(null, new object[] { errorMessage });
                }

                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}
