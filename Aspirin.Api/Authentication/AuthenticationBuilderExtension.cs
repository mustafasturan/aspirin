using System;
using Microsoft.AspNetCore.Authentication;

namespace Aspirin.Api.Authentication
{
    public static class AuthenticationBuilderExtension
    {
        public static AuthenticationBuilder AddAuthScheme(this AuthenticationBuilder builder, Action<AuthOptions> configureOptions)
        {
            return builder.AddScheme<AuthOptions, AuthHandler>(AuthOptions.DefaultScheme, configureOptions);
        }
    }
}
