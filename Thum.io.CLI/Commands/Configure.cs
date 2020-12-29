using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;

using Thum.io.CLI.Interfaces;

namespace Thum.io.CLI.Commands
{
    [Command(
        Name = "configure",
        Description = "Configure your API Key"
    )]
    class Configure : CommandBase
    {
        public Configure(IConsole console, ILogger<Configure> logger, IProfileService profileService) : base(profileService)
        {
            Console = console;
            Logger = logger;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            return await ConfigureApiKey();
        }
    }
}