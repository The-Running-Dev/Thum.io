using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;

using Thum.io.CLI.Interfaces;

namespace Thum.io.CLI.Commands
{
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    [Subcommand(typeof(Configure), typeof(ScreenShot))]
    public class App : CommandBase
    {
        public App(IConsole console, ILogger<App> logger, IProfileService profileService): base(profileService)
        {
            Console = console;
            Logger = logger;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            // Configure the profile, if needed
            await base.OnExecute(app);

            app.ShowHelp();

            return 0;
        }

        private static string GetVersion()
            => typeof(App).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}