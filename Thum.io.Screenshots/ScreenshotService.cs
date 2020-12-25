using System;
using System.IO;
using System.Net.Http;
using System.IO.Abstractions;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using Thum.io.Screenshots.Interfaces;

namespace Thum.io.Screenshots
{
    public class ScreenShotService : IScreenShotService
    {
        private readonly Settings _settings;
        
        private readonly IFileSystem _fileSystem;

        private readonly HttpClient _httpClient;
        
        public ScreenShotService(IOptions<Settings> settings, IFileSystem fileSystem, HttpClient httpClient)
        {
            _settings = settings.Value;
            _fileSystem = fileSystem;
            _httpClient = httpClient;

            if (_settings.ApiKey.IsEmpty())
            {
                throw new ArgumentException($"Please Provide Your API Key");
            }

            _httpClient.BaseAddress = new Uri(_settings.Url ?? Constants.Url);
        }

        public async Task<MemoryStream> ToMemory(string url, ImageModifierOptions options = null)
        {
            options = options ?? new ImageModifierOptions { NoAnimate = true };
            options.NoAnimate = true;

            var response = await _httpClient.GetAsync(
                Constants.ScreenShotParameters.FormatWith(new
                {
                    _settings.ApiKey,
                    Parameters = options.ToString(),
                    url
                }));
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