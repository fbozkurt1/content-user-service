using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using UserService.Application.Services;
using UserService.Domain.Configs;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Repositories;

namespace UserService.API.Infrastructure.Extensions
{
    public static class ServiceExtension
    {
        public static void ConfigureServices(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            services.Configure<ApplicationConfig>(configurationManager.GetSection("Application"));
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
        }

        public static void AddDependencyInjections(this IServiceCollection services)
        {
            services.AddScoped<IUserService, Application.Services.UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
