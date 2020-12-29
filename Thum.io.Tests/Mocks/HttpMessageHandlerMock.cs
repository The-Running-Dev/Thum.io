using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Moq;
using Moq.Protected;

namespace Thum.io.Tests.Mocks
{
    public class HttpMessageHandlerMock
    {
        public HttpMessageHandler Instance => Mock.Object;

        public Mock<HttpMessageHandler> Mock;

        public HttpMessageHandlerMock()
        {
            Mock = new Mock<HttpMessageHandler>();
        }

        public void Setup(HttpContent content)
        {
            Mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                }).Verifiable();
        }

        public void Verify(Times timesCalled, string requestUri)
        {
            Mock.Protected().Verify(
                "SendAsync",
                timesCalled,
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri.ToString() == requestUri
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}