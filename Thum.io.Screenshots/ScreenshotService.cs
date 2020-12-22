using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

namespace Thum.io.Screenshots
{
    public class ScreenshotService : IScreenshotService
    {
        private readonly HttpClient _httpClient;

        private readonly Settings _settings;

        public ScreenshotService(IOptions<Settings> settings, HttpClient httpClient)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_settings.Url);
        }

        public async Task<MemoryStream> ToMemory(string url)
        {
            var response = await _httpClient.GetAsync(
                $"/get/auth/{_settings.ApiKey}/{_settings.Parameters}/{url}");
            var content = await response.Content.ReadAsByteArrayAsync();
            var memoryStream = new MemoryStream(content, 0, content.Length);

            return memoryStream;
        }
        
        public async Task ToDisk(string url, string path)
        {
            var screehshot = await ToMemory(url);

            using (var file = new FileStream(path, FileMode.Create, FileAccess.Write))
                screehshot.CopyTo(file);
        }
    }
}