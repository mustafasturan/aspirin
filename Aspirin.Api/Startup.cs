using Aspirin.Api.Authentication;
using Aspirin.Api.Localization;
using Aspirin.Api.Middleware;
using Aspirin.Api.Model.Core;
using Aspirin.Api.Pipeline;
using FluentValidation.AspNetCore;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json.Serialization;

namespace Aspirin.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureServicesForMediatr(services);
            ConfigureServicesForAspirin(services);

            services.AddLocalization();

            services.AddCors();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = AuthOptions.DefaultScheme;
                options.DefaultAuthenticateScheme = AuthOptions.DefaultScheme;
                options.DefaultChallengeScheme = AuthOptions.DefaultScheme;
            })
            .AddAuthScheme(options => { });

            services.AddMvc()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("JwtTokenPolicy", policy => policy.RequireClaim("ApiKey").RequireClaim("Culture"));
            });

            services.AddMemoryCache();
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = "172.18.0.2";
                options.InstanceName = "redisInstance";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            var supportedCultures = LocalizationHelper.GetSupportedCultures();
            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(LocalizationHelper.GetDefaultCulture()),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };
            requestLocalizationOptions.RequestCultureProviders.Clear();
            requestLocalizationOptions.RequestCultureProviders.Add(new HeaderRequestCultureProvider());
            app.UseRequestLocalization(requestLocalizationOptions);

            app.UseAuthentication();

            app.UseMvc();
        }

        private void ConfigureServicesForMediatr(IServiceCollection services)
        {
            services.AddMediatR();
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachePipelineBehaviour<,>));
        }

        private void ConfigureServicesForAspirin(IServiceCollection services)
        {
            //Application specific service configurations should be added here.
            services.AddSingleton<ILocalizationHelper, LocalizationHelper>();
            services.AddScoped<RequestContext>();
        }
    }
}
