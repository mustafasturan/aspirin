using System;
using System.Threading.Tasks;
using Aspirin.Api.Data.Setting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Aspirin.Api.Model.Core
{
    public interface IConfigReader
    {
        Task<T> ReadJson<T>(string key);
        Task<string> ReadString(string key);
    }

    public class ConfigReader : IConfigReader
    {
        private readonly ISettingRepository _settingRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        public ConfigReader(ISettingRepository settingRepository, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _settingRepository = settingRepository;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public async Task<T> ReadJson<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key), "Config name can not be empty!");
            }
            string cacheKey = $"Aspirin:cache:config:json:{key}";
            //Read config from memory cache.
            bool isExist = _memoryCache.TryGetValue(cacheKey, out T response);
            if (isExist)
            {
                return response;
            }

            //Read config from database.
            var setting = await _settingRepository.GetSetting(key);
            if (!string.IsNullOrWhiteSpace(setting))
            {
                response = JsonConvert.DeserializeObject<T>(setting);
                _memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(30));
                return response;
            }

            //Read config from configuration sources.
            var config = _configuration.GetSection(key).Get<T>();
            if (config == null)
            {
                throw new AspirinException($"Config value for {key} can not be found!", 500);
            }

            _memoryCache.Set(cacheKey, config, TimeSpan.FromMinutes(30));

            return config;
        }

        public async Task<string> ReadString(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key), "Config name can not be empty!");
            }

            string cacheKey = $"Aspirin:cache:config:str:{key}";

            //Read config from memory cache.
            bool isExist = _memoryCache.TryGetValue(cacheKey, out string response);
            if (isExist)
            {
                return response;
            }

            //Read config from database.
            var setting = await _settingRepository.GetSetting(key);
            if (!string.IsNullOrWhiteSpace(setting))
            {
                _memoryCache.Set(cacheKey, setting, TimeSpan.FromMinutes(30));
                return setting;
            }

            //Read config from configuration sources.
            var config = _configuration[key];
            if (string.IsNullOrWhiteSpace(config))
            {
                throw new AspirinException($"Config value for {key} can not be found!", 500);
            }

            _memoryCache.Set(cacheKey, config, TimeSpan.FromMinutes(30));

            return config;
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
