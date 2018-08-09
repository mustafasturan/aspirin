using System;
using System.Threading;
using System.Threading.Tasks;
using Aspirin.Api.Model.Core;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Aspirin.Api.Pipeline
{
    public class CachePipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IMemoryCache _cache;
        private readonly IDistributedCache _distributedCache;
        private readonly ISerializer _serializer;

        public CachePipelineBehaviour(IMemoryCache cache, IDistributedCache distributedCache, ISerializer serializer)
        {
            _cache = cache;
            _distributedCache = distributedCache;
            _serializer = serializer;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            if (!(request is ICacheable cacheable))
            {
                return await next();
            }

            switch (cacheable.CacheOption)
            {
                case CacheOption.None:
                    return await next();
                case CacheOption.Memory:
                    return await GetFromMemoryCache(cacheable, next);
                case CacheOption.Distrubuted:
                    return await GetFromDistrubutedCache(cacheable, cancellationToken, next);
                case CacheOption.Multi:
                    throw new NotImplementedException("Bunu unutma");
            }

            return await next();
        }

        private async Task<TResponse> GetFromMemoryCache(ICacheable cacheable, RequestHandlerDelegate<TResponse> next)
        {
            bool isExist = _cache.TryGetValue(cacheable.CacheSettings.Key, out TResponse response);
            if (isExist)
            {
                return response;
            }
            response = await next();
            _cache.Set(cacheable.CacheSettings.Key, response, cacheable.CacheSettings.Value);
            return response;
        }

        private async Task<TResponse> GetFromDistrubutedCache(ICacheable cacheable, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var value = await _distributedCache.GetAsync(cacheable.CacheSettings.Key, cancellationToken);
            if (value != null)
            {
                return _serializer.Deserialize<TResponse>(value);
            }
            var response = await next();
            var option = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheable.CacheSettings.Value,
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };
            await _distributedCache.SetAsync(cacheable.CacheSettings.Key, _serializer.Serialize(response), option, cancellationToken);
            return response;
        }
    }
}
