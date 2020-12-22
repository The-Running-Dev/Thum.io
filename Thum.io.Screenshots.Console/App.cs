using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.CommandLineUtils;

namespace Thum.io.Screenshots.Console
{
    public class App
    {
        private readonly IScreenshotService _screenshotService;

        private readonly ILogger<App> _logger;

        private CommandLineApplication _app;

        public App(IScreenshotService screenshotService, ILogger<App> logger)
        {
            _screenshotService = screenshotService ?? throw new ArgumentNullException(nameof(screenshotService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            RegisterCommands();
        }

        public void Main(string[] args)
        {
            Environment.Exit(_app.Execute(args));
        }

        public void RegisterCommands()
        {
            _app = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                Name = "Thum.io.Screenshots.Console",
                Description = "Takes a screenshot of a URL and saves it to a local file",
            };
            _app.HelpOption("-h | --help");

            var args = Environment.GetCommandLineArgs().ToList().Skip(1).ToArray();
            var screenshot = _app.Command("screenshot", config =>
            {
                config.Description = "Take a Screenshot";
                
                var url = config.Argument("url", "The URL to take a screenshot of.", false);
                var path = config.Argument("path", "The path to save the screenshot to.", false);

                config.OnExecute(async () => await TakeScreenshot(config, url.Value, path.Value));
                config.HelpOption("-h | --help");
            });

            _app.Command("help", config =>
            {
                config.Description = "Get Help";
                config.OnExecute(() =>
                {
                    screenshot.ShowHelp("screenshot");

                    return 1;
                });
            });

            _app.OnExecute(() =>
            {
                _app.ShowHelp();
                
                return 1;
            });
        }

        public async Task<int> TakeScreenshot(CommandLineApplication app, string url, string path)
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(path))
            {
                _logger.LogInformation($"Saving Screenshot of \"{url}\" to \"{path}\"...");

                await _screenshotService.ToDisk(url, path);

                return 0;
            }
            else
            {
                app.ShowHelp();

                return 1;
            }
        }
    }
}