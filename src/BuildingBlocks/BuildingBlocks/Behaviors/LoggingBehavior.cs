using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            logger.LogInformation($"[START] Handle Request={typeof(TRequest).Name} - Response={typeof(TResponse).Name} - RequestData ={request}");
            var timer = new Stopwatch();
            timer.Start();
            var response = await next();

            timer.Stop();
            var timeTaken = timer.Elapsed;
            if (timeTaken.Seconds > 3) // if the request is greater than 3 secods, then log the warning
                logger.LogWarning($"[PERFORMANCE] The Request {typeof(TRequest).Name} took {timeTaken} seconds");

            logger.LogInformation($"[END] Handled {typeof(TRequest).Name} with {typeof(TResponse)}");
            return response;

        }
    }
}
