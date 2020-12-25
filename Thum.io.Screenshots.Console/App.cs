using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Thum.io.Screenshots.Interfaces;
using Microsoft.Extensions.CommandLineUtils;

namespace Thum.io.Screenshots.Console
{
    public class App
    {
        private readonly IScreenShotService _screenShotService;

        private readonly ILogger<App> _logger;

        private CommandLineApplication _app;

        public App(IScreenShotService screenShotService, ILogger<App> logger)
        {
            _screenShotService = screenShotService ?? throw new ArgumentNullException(nameof(screenShotService));
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
                Description = "Takes a screenshot of a URL and saves it to a local file"
            };
            _app.HelpOption("-h|--help");

            var args = Environment.GetCommandLineArgs().ToList().Skip(1).ToArray();
            var screenShotCommand = _app.Command("screenshot", config =>
            {
                config.Description = "Take a Screenshot";

                var width = config.Option("-w|--width <WIDTH>", "Thumbnail width in pixels.", CommandOptionType.SingleValue);
                var crop = config.Option("-c|--crop <CROP>", "Height of original screen shot in pixels.", CommandOptionType.SingleValue);
                var url = config.Argument("url", "The URL to take a screenshot of.");
                var path = config.Argument("path", "The path to save the screenshot to.");

                config.OnExecute(async () =>
                {
                    var parameters = new ImageModifierOptions();
                
                    if (int.TryParse(width.Value(), out var parametersWidth)) {
                        parameters.Width = parametersWidth;
                    }

                    if (int.TryParse(crop.Value(), out var parametersCrop)) {
                        parameters.Crop = parametersCrop;
                    }

                    return await TakeScreenshot(config, url.Value, path.Value, parameters);
                });
                config.HelpOption("-h|--help");
            });

            _app.Command("help", config =>
            {
                config.Description = "Get Help";
                config.OnExecute(() =>
                {
                    screenShotCommand.ShowHelp("screenshot");

                    return 1;
                });
            });

            _app.OnExecute(() =>
            {
                _app.ShowHelp();

                return 1;
            });
        }

        public async Task<int> TakeScreenshot(CommandLineApplication app, string url, string path, ImageModifierOptions parameters)
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(path))
            {
                _logger.LogInformation($"Saving Screenshot of \"{url}\" to \"{path}\"...");

                await _screenShotService.ToDisk(url, path, parameters);

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