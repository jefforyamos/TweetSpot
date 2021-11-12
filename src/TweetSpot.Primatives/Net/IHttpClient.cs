using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace TweetSpot.Net
{
    /// <summary>
    /// Clients to http services should use this as a dependency rather than instancing <see cref="HttpClient"/>.
    /// Calling through this interface allows the caller to be unit testable via mocks of this interface.
    /// </summary>
    /// <remarks>
    /// It's intentional this interface is identical as possible with the underlying <see cref="HttpClient"/>.
    /// It's also intentional that only properties and methods necessary for our usage are exposed.
    /// </remarks>
    public interface IHttpClient : IDisposable
    {
        /// <summary>
        /// Gets the headers that should be sent with each request.
        /// </summary>
        HttpRequestHeaders DefaultRequestHeaders { get; }

        /// <summary>
        /// Gets or sets the timespan to await before the request times out
        /// </summary>
        TimeSpan Timeout { get; set; }

        /// <summary>
        /// Send a GET request to the specified Uri and return the response body as a stream in an asynchronous operation.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="TaskCanceledException"></exception>
        Task<Stream> GetStreamAsync(Uri requestUri, CancellationToken cancellationToken);
    }
}