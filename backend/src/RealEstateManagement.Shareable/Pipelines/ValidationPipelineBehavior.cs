using FluentValidation;
using MediatR;
using OperationResult;
using RealEstateManagement.Shareable.Exceptions;

namespace RealEstateManagement.Shareable.Pipelines
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IResult<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
            => _validators = validators;        

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
                var errors = failures.GroupBy(e => e.PropertyName, v => v.ErrorMessage).ToDictionary(z=>z.Key, z=>z.Select(y=>y));
                if (failures.Count != 0)
                    return TResponse.Error(new DataInvalidException(errors));
            }
            return await next();
        }
    }
}
