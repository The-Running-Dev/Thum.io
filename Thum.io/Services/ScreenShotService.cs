using System;
using System.IO;
using System.Net.Http;
using System.IO.Abstractions;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Thum.io.Interfaces;

namespace Thum.io.Services
{
    public class ScreenShotService : IScreenShotService
    {
        public Settings Settings { get; set; }
        
        private readonly IFileSystem _fileSystem;

        private readonly HttpClient _httpClient;

        public ScreenShotService(IOptions<Settings> settings, IFileSystem fileSystem, HttpClient httpClient)
        {
            Settings = settings.Value;
            _fileSystem = fileSystem;
            _httpClient = httpClient;
        }

        public async Task<MemoryStream> ToMemory(string url, ImageModifierOptions options = null)
        {
            if (Settings.ApiKey.IsEmpty())
            {
                throw new ArgumentException("Please Provide Your API Key");
            }

            _httpClient.BaseAddress = new Uri(Settings.Url ?? Constants.Url);

            options = options ?? new ImageModifierOptions { NoAnimate = true };
            options.NoAnimate = true;

            var parameters = Constants.ScreenShotParameters.FormatWith(new
            {
                Settings.ApiKey,
                Parameters = options.ToString(),
                url
            });
            var response = await _httpClient.GetAsync(parameters);
            var content = await response.Content.ReadAsByteArrayAsync();
            var memoryStream = new MemoryStream(content, 0, content.Length);

            return memoryStream;
        }

        public async Task ToDisk(string url, string path, ImageModifierOptions options = null)
        {
            var screenShot = await ToMemory(url, options);

            using (var file = _fileSystem.FileStream.Create(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                await screenShot.CopyToAsync(file);
            }
        }
    }
}