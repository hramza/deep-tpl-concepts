using Hra.Framework.Sample.Repositories;
using Hra.Framework.Web.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hra.Framework.Sample
{
    internal static class ServiceCollectionExtensions
    {
        public static void ConfigureEndpoints(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureHttpClient<ICurrenyApiRepository, CurrenyApiRepository>(new Uri(configuration["Endpoints:Cex"]));
        }

        private static void ConfigureHttpClient<TInterface, TImplementation>(this IServiceCollection services, Uri uri, int timeoutSeconds = 100)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.AddHttpClient<TInterface, TImplementation>(config =>
            {
                config.BaseAddress = uri;
                config.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                config.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            })
            .AddHttpMessageHandler<PollyHandler>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(_handlerLifetime));
        }

        private const int _handlerLifetime = 5;
    }
}
