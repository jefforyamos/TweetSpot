using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace TweetSpot.Net
{
    /// <summary>
    /// This is the default implementation of <see cref="IHttpClient"/>.  It defers all operations
    /// to an underlying instance of <see cref="HttpClient"/>.  
    /// </summary>
    public class HttpClientContainer : IHttpClient
    {
        private readonly HttpClient _client;
        public HttpClientContainer()
        {
            _client = new HttpClient();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client.Dispose();
            }
        }

        public HttpRequestHeaders DefaultRequestHeaders => _client.DefaultRequestHeaders;

        public TimeSpan Timeout
        {
            get => _client.Timeout;

            set => _client.Timeout = value;
        }

        public Task<Stream> GetStreamAsync(Uri requestUri, CancellationToken cancellationToken)
        {
            return _client.GetStreamAsync(requestUri, cancellationToken);
        }
    }
}