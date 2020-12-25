using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

using Moq;
using Moq.Protected;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.Extensions.Options;

using Thum.io.Screenshots;
using Thum.io.Screenshots.Interfaces;

namespace Thum.io.Tests
{
    public class ScreenShotServiceTests
    {
        private IScreenShotService _service;

        private IOptions<Settings> _settings;

        private HttpClient _httpClient;

        private MockFileSystem _fileSystem;

        private Mock<HttpMessageHandler> _httpMessageHandler;

        private string _screenShot = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "Screenshot.png");

        [SetUp]
        public void Setup()
        {
            _httpMessageHandler = new Mock<HttpMessageHandler>();
            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ReadOnlyMemoryContent(File.ReadAllBytes(_screenShot))
                }).Verifiable();

            _settings = Options.Create(new Settings { Url = "https://image.thum.io", ApiKey = "ApiKey" });
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            _fileSystem.AddDirectory(@"C:\Temp");
            _httpClient = new HttpClient(_httpMessageHandler.Object);
            _service = new ScreenShotService(_settings, _fileSystem, _httpClient);
        }

        [Test]
        public async Task To_Memory_Should_Return_Image()
        {
            var screenShotUrl = "https://google.com";
            var screenShot = await _service.ToMemory(screenShotUrl);
            var targetUri = $"{_settings.Value.Url}/get/auth/{_settings.Value.ApiKey}/{nameof(ImageModifierOptions.NoAnimate)}/{screenShotUrl}";

            _httpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri.ToString() == targetUri
                ),
                ItExpr.IsAny<CancellationToken>()
            );

            screenShot.Should().NotBeNull();
        }

        [Test]
        public async Task To_Memory_with_Options_Should_Return_Image()
        {
            var parameters = new ImageModifierOptions { NoAnimate = true, Width = 1000 };
            var screenShotUrl = "https://google.com";
            var screenShot = await _service.ToMemory(screenShotUrl, parameters);
            var targetUri = $"{_settings.Value.Url}/get/auth/{_settings.Value.ApiKey}/" +
                            $"{nameof(ImageModifierOptions.Width)}/{parameters.Width}/" +
                            $"{nameof(ImageModifierOptions.NoAnimate)}/{screenShotUrl}";

            _httpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri.ToString() == targetUri
                ),
                ItExpr.IsAny<CancellationToken>()
            );

            screenShot.Should().NotBeNull();
        }
        
        [Test]
        public async Task To_Disk_Should_Save_to_File_System()
        {
            var screenShotUrl = "https://google.com";
            var path = "C:/Temp/Image.png";

            await _service.ToDisk(screenShotUrl, path);

            var contents = _fileSystem.GetFile(path).Contents;

            contents.Should().NotBeNull();
            File.ReadAllBytes(_screenShot).Should().BeEquivalentTo(contents);
        }
    }
}