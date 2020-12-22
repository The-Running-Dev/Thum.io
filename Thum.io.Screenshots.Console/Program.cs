using System.IO;
using System.Reflection;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Thum.io.Screenshots.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var services = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddConsole();
                })
                .AddThumIoScreenshots(config)
                .AddSingleton<App>()
                .BuildServiceProvider();

            services.GetService<App>().Main(args);
        }
    }
}