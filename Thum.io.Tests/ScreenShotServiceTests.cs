using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

using Moq;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.Extensions.Options;

using Thum.io.Services;
using Thum.io.Interfaces;
using Thum.io.Tests.Mocks;

namespace Thum.io.Tests
{
    public class ScreenShotServiceTests
    {
        private IScreenShotService _service;

        private IOptions<Settings> _settings;

        private HttpClient _httpClient;

        private MockFileSystem _fileSystem;

        private HttpMessageHandlerMock _httpMessageHandler;

        private string _screenShot = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "Screenshot.png");

        [SetUp]
        public void Setup()
        {
            _httpMessageHandler = new HttpMessageHandlerMock();
            _httpMessageHandler.Setup(new ReadOnlyMemoryContent(File.ReadAllBytes(_screenShot)));
            _settings = Options.Create(new Settings { Url = "https://image.thum.io", ApiKey = "ApiKey" });
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            _fileSystem.AddDirectory(@"/home/temp");
            _httpClient = new HttpClient(_httpMessageHandler.Instance);
            _service = new ScreenShotService(_settings, _fileSystem, _httpClient);
        }

        [Test]
        public void Instance_Without_Api_Key_Should_Throw_Exception()
        {
            var settings = Options.Create(new Settings());

            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                var service = new ScreenShotService(settings, _fileSystem, _httpClient);
                await service.ToMemory($"https://google.com");
            });
        }

        [Test]
        public async Task Instance_Without_Url_Should_Use_Default_Url()
        {
            var settings = Options.Create(new Settings { ApiKey = "ApiKey" });
            var service = new ScreenShotService(settings, _fileSystem, _httpClient);
            var screenShotUrl = "https://google.com";
            var screenShot = await service.ToMemory(screenShotUrl);
            var parameters = Constants.ScreenShotParameters.FormatWith(new
            {
                ApiKey = settings.Value.ApiKey,
                Parameters = new ImageModifierOptions { NoAnimate = true }.ToString(),
                Url = screenShotUrl
            });
            var targetUri = $"{Constants.Url}/{parameters}";

            _httpMessageHandler.Verify(Times.Exactly(1), targetUri);

            screenShot.Should().NotBeNull();
        }

        [Test]
        public async Task Instance_With_Url_Should_Use_Provided_Url()
        {
            var settings = Options.Create(new Settings { Url = "http://custom.url", ApiKey = "ApiKey" });
            var service = new ScreenShotService(settings, _fileSystem, _httpClient);
            var screenShotUrl = "https://google.com";
            var screenShot = await service.ToMemory(screenShotUrl);
            var parameters = Constants.ScreenShotParameters.FormatWith(new
            {
                ApiKey =  settings.Value.ApiKey,
                Parameters = new ImageModifierOptions { NoAnimate = true }.ToString(),
                Url = screenShotUrl
            });
            var targetUri = $"{settings.Value.Url}/{parameters}";

            _httpMessageHandler.Verify(Times.Exactly(1), targetUri);

            screenShot.Should().NotBeNull();
        }

        [Test]
        public async Task To_Memory_Should_Return_Image()
        {
            var screenShotUrl = "https://google.com";
            var screenShot = await _service.ToMemory(screenShotUrl);
            var parameters = Constants.ScreenShotParameters.FormatWith(new
            {
                ApiKey = _settings.Value.ApiKey,
                Parameters = new ImageModifierOptions { NoAnimate = true }.ToString(),
                Url = screenShotUrl
            });
            var targetUri = $"{_settings.Value.Url}/{parameters}";

            _httpMessageHandler.Verify(Times.Exactly(1), targetUri);

            screenShot.Should().NotBeNull();
        }

        [Test]
        public async Task To_Memory_with_Options_Should_Return_Image()
        {
            var options = new ImageModifierOptions {NoAnimate = true, Width = 1000};
            var screenShotUrl = "https://google.com";
            var screenShot = await _service.ToMemory(screenShotUrl, options);
            var parameters = Constants.ScreenShotParameters.FormatWith(new
            {
                ApiKey = _settings.Value.ApiKey,
                Parameters = options.ToString(),
                Url = screenShotUrl
            });
            var targetUri = $"{_settings.Value.Url}/{parameters}";

            _httpMessageHandler.Verify(Times.Exactly(1), targetUri);

            screenShot.Should().NotBeNull();
        }

        [Test]
        public async Task To_Disk_Should_Save_to_File_System()
        {
            var screenShotUrl = "https://google.com";
            var path = "/home/temp/image.png";

            await _service.ToDisk(screenShotUrl, path);

            var contents = _fileSystem.GetFile(path).Contents;

            contents.Should().NotBeNull();
            File.ReadAllBytes(_screenShot).Should().BeEquivalentTo(contents);
        }
    }
}