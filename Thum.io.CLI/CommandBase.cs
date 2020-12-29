using System;
using System.Security;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;
using Serilog;
using Thum.io.CLI.Models;
using Thum.io.CLI.Interfaces;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Thum.io.CLI
{
    [HelpOption("--help")]
    public abstract class CommandBase
    {
        [Option(
            CommandOptionType.SingleValue,
            ShortName = "k",
            LongName = "key",
            Description = "API Key in the format {Id}-{Url Key}",
            ValueName = "API Key",
            ShowInHelpText = true
        )]
        public string ApiKey { get; set; }

        protected ILogger Logger { get; set; }

        protected IConsole Console { get; set; }

        protected Profile Profile
        {
            get => ProfileService.Get();
            set => ProfileService.Update(value);
        }

        protected IProfileService ProfileService;

        protected CommandBase(IProfileService profileService)
        {
            ProfileService = profileService;
        }

        protected virtual async Task<int> OnExecute(CommandLineApplication app)
        {
            if (Profile is null)
            {
                OutputToConsole("Please configure your API key...");
                OutputToConsole(string.Empty);

                await ConfigureApiKey();
            }

            return 0;
        }

        protected async Task<int> ConfigureApiKey()
        {
            if (ApiKey.IsEmpty())
            {
                // Prompt the user to enter their API Key
                ApiKey = SecureStringToString(Prompt.GetPasswordAsSecureString("API Key:"));
            }
            
            try
            {
                if (ApiKey.IsNotEmpty())
                {
                    await ProfileService.Create(ApiKey);

                    return 0;
                }

                OutputError("API key not specified, profile not created...");
            }
            catch (Exception ex)
            {
                OnException(ex);
            }

            return 1;
        }

        protected string SecureStringToString(SecureString value)
        {
            var valuePtr = IntPtr.Zero;

            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);

                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        protected void OnException(Exception ex)
        {
            OutputError(ex.Message);

            Logger.LogError(ex.Message);
            Logger.LogDebug(ex, ex.Message);
        }

        protected void OutputToConsole(string data)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Out.WriteLine(data);
            Console.ResetColor();
        }

        protected void OutputError(string message)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Error.WriteLine(message);
            Console.ResetColor();
        }
    }
}