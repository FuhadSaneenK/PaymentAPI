using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

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
}
