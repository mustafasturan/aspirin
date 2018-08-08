using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Aspirin.Api.Service
{
    public class ReadFromRedis : IRequest<string>
    {
        public ReadFromRedis(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }

    public class ReadFromRedisValidator : AbstractValidator<ReadFromRedis>
    {
        public ReadFromRedisValidator()
        {
            RuleFor(x => x.Key).NotEmpty();
        }
    }

    public class ReadFromRedisHandler : IRequestHandler<ReadFromRedis,string>
    {
        private readonly IDistributedCache _distributedCache;

        public ReadFromRedisHandler(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<string> Handle(ReadFromRedis request, CancellationToken cancellationToken)
        {
            var value = await _distributedCache.GetAsync(request.Key, cancellationToken);
            if (value == null)
            {
                return "Empty";
            }
            return Encoding.UTF8.GetString(value);
        }
    }
}
