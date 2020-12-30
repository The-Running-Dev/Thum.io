using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;

using Thum.io.Interfaces;
using Thum.io.CLI.Interfaces;

namespace Thum.io.CLI.Commands
{
    [Command(
        Name = "screenshot",
        Description = "Take a screenshot of a URL and save it to a file",
        UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue,
        OptionsComparison = StringComparison.InvariantCultureIgnoreCase
    )]
    public class ScreenShot : CommandBase
    {
        [Option(
            CommandOptionType.SingleValue,
            ShortName = "w",
            LongName = "width",
            Description = "Thumbnail width in pixels",
            ValueName = "width",
            ShowInHelpText = true
        )]
        public int? Width { get; set; }

        [Option(
            CommandOptionType.SingleValue,
            ShortName = "c",
            LongName = "crop",
            Description = "Height of original screen shot in pixels",
            ValueName = "crop",
            ShowInHelpText = true
        )]
        public int? Crop { get; set; }

        [Option(
            CommandOptionType.SingleValue,
            ShortName = "u",
            LongName = "url",
            Description = "The URL to take a screenshot of",
            ValueName = "url",
            ShowInHelpText = true
        )]
        public string Url { get; set; }

        [Option(
            CommandOptionType.SingleValue,
            ShortName = "f",
            LongName = "file",
            Description = "The path where to save the screenshot",
            ValueName = "file",
            ShowInHelpText = true
        )]
        public string File { get; set; }

        private readonly IScreenShotService _screenShotService;

        public ScreenShot(IConsole console, ILogger<ScreenShot> logger, IProfileService profileService, IScreenShotService screenShotService): base(profileService)
        {
            Console = console;
            Logger = logger;

            _screenShotService = screenShotService;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            // Configure the profile, if needed
            await base.OnExecute(app);

            try
            {
                // Prompt for URL and File, if they were not provided
                Url ??= Prompt.GetString("Url:");
                File ??= Prompt.GetString("File:", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "screenshot.png"));
                
                // Use the API key from the command line, or the profile
                _screenShotService.Settings.ApiKey = ApiKey ?? Profile.ApiKey;

                var parameters = new ImageModifierOptions
                {
                    Width = Width ?? 0,
                    Crop = Crop ?? 0
                };

                if (Url.IsNotEmpty() && File.IsNotEmpty())
                {
                    OutputToConsole($"Taking Screenshot of \"{Url}\"...");
                    
                    await _screenShotService.ToDisk(Url, File, parameters);

                    OutputToConsole($"Screenshot Saved to {File}...");

                    return 0;
                }
                
                app.ShowHelp();

                return 1;
            }
            catch (Exception ex)
            {
                OnException(ex);

                return 1;
            }
        }
    }
}