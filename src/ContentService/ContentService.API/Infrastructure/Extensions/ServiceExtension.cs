using ContentService.Application.Services;
using ContentService.Domain.ApiServices;
using ContentService.Domain.Configs;
using ContentService.Domain.Repositories;
using ContentService.Infrastructure.ApiServices;
using ContentService.Infrastructure.ApiServices.Refits;
using ContentService.Infrastructure.Repositories;
using Microsoft.AspNetCore.ResponseCompression;
using Refit;
using System.IO.Compression;
using Polly;
using System.Net.Http;

namespace ContentService.API.Infrastructure.Extensions
{
    public static class ServiceExtension
    {
        public static void ConfigureServices(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            services.Configure<ApplicationConfig>(configurationManager.GetSection("Application"));
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
        }

        public static void AddDependencyInjections(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("Application").Get<ApplicationConfig>();

            services.AddScoped<IContentRepository, ContentRepository>();
            services.AddScoped<IContentService, Application.Services.ContentService>();
            services.AddScoped<IUserApiService, UserApiService>();

            // To improve consistency we can add retry policies and circuit breakers
            services.AddRefitClient<IUserApiRefitClient>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(config!.UserApiConfig.BaseUrl);
                })
                .SetHandlerLifetime(TimeSpan.FromSeconds(300))
                .AddPolicyHandler(Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .OrResult(msg => (int)msg.StatusCode >= 500)
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
        }
    }
}
