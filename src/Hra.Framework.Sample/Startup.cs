using Hra.Framework.Sample.Background;
using Hra.Framework.Sample.Models;
using Hra.Framework.Utils;
using Hra.Framework.Web.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hra.Framework.Sample
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            Ensure.NotNull(configuration, nameof(configuration));

            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<PollyHandler>();

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    var jsonSerializerOptions = options.JsonSerializerOptions;

                    jsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

                    jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.ConfigureEndpoints(_configuration);

            services.AddSingleton<BoundedMessageChannel<CurrencyRequest>>();

            services.AddHostedService<CurrencyLimitsBackgroundService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
