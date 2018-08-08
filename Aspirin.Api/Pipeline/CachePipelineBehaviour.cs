using System.Threading;
using System.Threading.Tasks;
using Aspirin.Api.Model.Core;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Aspirin.Api.Pipeline
{
    public class CachePipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IMemoryCache _cache;

        public CachePipelineBehaviour(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!(request is ICacheable cacheable))
            {
                return await next();
            }

            bool isExist = _cache.TryGetValue(cacheable.CacheSettings.Key, out TResponse response);
            if (isExist)
            {
                return response;
            }
            response = await next();
            _cache.Set(cacheable.CacheSettings.Key, response, cacheable.CacheSettings.Value);
            return response;
        }
    }
}
