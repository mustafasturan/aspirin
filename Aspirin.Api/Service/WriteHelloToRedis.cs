using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Aspirin.Api.Service
{
    public class WriteHelloToRedis : IRequest
    {
        public WriteHelloToRedis(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }

    public class WriteHelloToRedisValidator : AbstractValidator<WriteHelloToRedis>
    {
        public WriteHelloToRedisValidator()
        {
            RuleFor(x => x.Key).NotEmpty();
        }
    }

    public class WriteHelloToRedisHandler : AsyncRequestHandler<WriteHelloToRedis>
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<WriteHelloToRedisHandler> _logger;

        public WriteHelloToRedisHandler(IDistributedCache distributedCache, ILoggerFactory loggerFactory)
        {
            _distributedCache = distributedCache;
            _logger = loggerFactory.CreateLogger<WriteHelloToRedisHandler>();
        }

        protected override async Task Handle(WriteHelloToRedis request, CancellationToken cancellationToken)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                };
                await _distributedCache.SetAsync(request.Key, Encoding.UTF8.GetBytes("Hello"), options, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Redis exception");
            }
        }
    }
}
