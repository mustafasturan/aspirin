using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Aspirin.Api.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Aspirin.Api.Localization
{
    public interface ILocalizationHelper
    {
        string GetLocalizedString(string key);
    }

    public class LocalizationHelper : ILocalizationHelper
    {
        private static readonly List<string> _supportedCultures = new List<string> { "en-US", "tr-TR" };
        private readonly IStringLocalizer<SharedResource> _localizer;

        public LocalizationHelper(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public string GetLocalizedString(string key)
        {
            return _localizer[key].Value;
        }

        public static string GetDefaultCulture()
        {
            return _supportedCultures[0];
        }

        public static List<CultureInfo> GetSupportedCultures()
        {
            return _supportedCultures.Select(culture => new CultureInfo(culture)).ToList();
        }

        public static string CheckCulture(string culture)
        {
            var foundCulture = _supportedCultures.FirstOrDefault(c => c.Equals(culture, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrWhiteSpace(foundCulture) ? GetDefaultCulture() : foundCulture;
        }
    }

    public static class LocalizationHelperExtensions
    {
        public static void AddLocalizationHelper(this IServiceCollection services)
        {
            services.AddSingleton<ILocalizationHelper, LocalizationHelper>();
        }
    }
}
