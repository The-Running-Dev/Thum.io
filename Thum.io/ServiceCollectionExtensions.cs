using System.IO.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Thum.io.Services;
using Thum.io.Interfaces;

namespace Thum.io
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddThumIoScreenshots(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = "ScreenShotService")
        {
            var config = configuration.GetSection(sectionName);
            
            services.Configure<Settings>(config);
            services.AddTransient<IFileSystem, FileSystem>();
            services.AddHttpClient<IScreenShotService, ScreenShotService>();
            
            return services;
        }
    }
}