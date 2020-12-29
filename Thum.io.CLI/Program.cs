using System;
using System.Threading.Tasks;

using Serilog;
using PeanutButter.INIFile;
using Serilog.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Thum.io.CLI.Models;
using Thum.io.CLI.Commands;
using Thum.io.CLI.Services;
using Thum.io.CLI.Interfaces;

namespace Thum.io.CLI
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    var env = hostContext.HostingEnvironment;

                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile($"appsettings.json", true, true)
                        .AddIniFile(ProfileService.ConfigFile, true, true)
                        .AddEnvironmentVariables()
                        .Build();

                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .Enrich.FromLogContext()
                        .CreateLogger();

                    services.AddLogging(config =>
                    {
                        config.ClearProviders();
                        config.AddProvider(new SerilogLoggerProvider(Log.Logger));

                        var minimumLevel = configuration.GetSection("Serilog:MinimumLevel")?.Value;
                        
                        if (minimumLevel.IsNotEmpty())
                        {
                            config.SetMinimumLevel(Enum.Parse<LogLevel>(minimumLevel));
                        }
                    });

                    services.Configure<Profile>(configuration);
                    services.AddTransient<IINIFile>(_ => new INIFile(ProfileService.ConfigFile));
                    services.AddTransient<IProfileService, ProfileService>();
                    services.AddThumIoScreenshots(configuration);
                });

            try
            {
                return await builder.RunCommandLineApplicationAsync<App>(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException != null ? ex.InnerException.Message : ex.Message);

                return 1;
            }
        }
    }
}