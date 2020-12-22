using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Thum.io.Screenshots
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddThumIoScreenshots(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = "ScreenshotService")
        {
            var config = configuration.GetSection(sectionName);
            
            services.Configure<Settings>(config);
            services.AddHttpClient<IScreenshotService, ScreenshotService>();
            
            return services;
        }
    }
}