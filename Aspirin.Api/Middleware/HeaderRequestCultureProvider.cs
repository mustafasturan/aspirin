using System.Threading.Tasks;
using Aspirin.Api.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Aspirin.Api.Middleware
{
    public class HeaderRequestCultureProvider : RequestCultureProvider
    {
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext?.Request == null)
            {
                return Task.FromResult(new ProviderCultureResult(LocalizationHelper.GetDefaultCulture()));
            }

            var requestCulture = httpContext.Request.Headers["Culture"];
            var culture = LocalizationHelper.CheckCulture(requestCulture);

            return Task.FromResult(new ProviderCultureResult(culture));
        }
    }
}
