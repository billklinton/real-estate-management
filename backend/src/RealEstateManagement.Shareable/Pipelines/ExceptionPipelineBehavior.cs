using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace RealEstateManagement.Shareable.Pipelines
{
    public class ExceptionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IResult<TResponse>
    {
        private readonly ILogger<ExceptionPipelineBehavior<TRequest, TResponse>> _logger;

        public ExceptionPipelineBehavior(ILogger<ExceptionPipelineBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling {RequestName}", typeof(TRequest).Name);
                return TResponse.Error(ex);
            }
        }
    }
}