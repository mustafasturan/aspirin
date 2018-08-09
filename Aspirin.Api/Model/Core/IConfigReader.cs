using System;
using System.Threading.Tasks;
using Aspirin.Api.Data.Setting;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Aspirin.Api.Model.Core
{
    public interface IConfigReader
    {
        Task<T> Read<T>(string key);
    }

    public class ConfigReader : IConfigReader
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        public ConfigReader(IMediator mediator, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _mediator = mediator;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public async Task<T> Read<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key), "Config name can not be empty!");
            }

            //Read config from memory cache.
            bool isExist = _memoryCache.TryGetValue($"Aspirin:config:cache:{key}", out T response);
            if (isExist)
            {
                return response;
            }

            //Read config from database.
            var setting = await _mediator.Send(new GetSetting(key));
            if (!string.IsNullOrWhiteSpace(setting))
            {
                response = JsonConvert.DeserializeObject<T>(setting);
                _memoryCache.Set(key, response, TimeSpan.FromMinutes(30));
                return response;
            }

            //Read config from configuration sources.
            var featureValue = _configuration[key];
            if (string.IsNullOrWhiteSpace(featureValue))
            {
                throw new AspirinException($"Config value for {key} can not be found!", 500);
            }

            return JsonConvert.DeserializeObject<T>(featureValue);
        }
    }

    public static class ConfigReaderExtensions
    {
        public static void AddConfigReader(this IServiceCollection services)
        {
            services.AddSingleton<IConfigReader, ConfigReader>();
        }
    }
}
