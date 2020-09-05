using Polly;
using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Hra.Framework.Web.Handlers
{
    public class PollyHandler : DelegatingHandler
    {
        private readonly TimeSpan[] RetryTimeSpans = new[]
        {
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(200),
            TimeSpan.FromMilliseconds(300),
            TimeSpan.FromMilliseconds(400),
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromMilliseconds(600),
            TimeSpan.FromMilliseconds(700),
            TimeSpan.FromMilliseconds(800),
            TimeSpan.FromMilliseconds(900),
            TimeSpan.FromSeconds(1)
        };

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Policy
            .Handle<TimeoutException>()
            .Or<TaskCanceledException>()
            .Or<SocketException>()
            .Or<AggregateException>()
            .Or<HttpRequestException>()
            .OrResult<HttpResponseMessage>(_ =>
            {
                int statusCode = (int)_.StatusCode;
                return (statusCode >= 500 && statusCode < 600) || statusCode == 429;
            })
            .WaitAndRetryAsync(RetryTimeSpans)
            .ExecuteAsync(token => base.SendAsync(request, token), cancellationToken);
    }
}
