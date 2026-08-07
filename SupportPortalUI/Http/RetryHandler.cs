using System.Net;

namespace SupportPortalUI.Http;

/// <summary>
/// Simple transient-fault retry handler with exponential backoff.
/// Avoids adding Polly to keep the project light; suitable for UI-level resiliency.
/// </summary>
public sealed class RetryHandler : DelegatingHandler
{
    private readonly ILogger<RetryHandler>? _logger;
    private readonly int _maxRetries;
    private readonly TimeSpan _baseDelay;

    public RetryHandler(ILogger<RetryHandler>? logger = null, int maxRetries = 3, TimeSpan? baseDelay = null)
    {
        _logger = logger;
        _maxRetries = maxRetries;
        _baseDelay = baseDelay ?? TimeSpan.FromMilliseconds(200);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage? response = null;

        for (var attempt = 1; attempt <= _maxRetries; attempt++)
        {
            try
            {
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

                // If success or client error (4xx) return immediately.
                if (response.IsSuccessStatusCode || ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500))
                {
                    return response;
                }

                // For server errors (5xx) we'll retry.
                _logger?.LogWarning("Transient server error {Status} on {Method} {Uri}; attempt {Attempt}", response.StatusCode, request.Method, request.RequestUri, attempt);
            }
            catch (HttpRequestException ex) when (attempt < _maxRetries)
            {
                _logger?.LogWarning(ex, "HttpRequestException on attempt {Attempt} for {Method} {Uri}", attempt, request.Method, request.RequestUri);
            }

            // Dispose the response before retrying to avoid leaked resources.
            if (response is not null && attempt < _maxRetries)
            {
                try { response.Dispose(); } catch { }
            }

            if (attempt == _maxRetries)
            {
                // If final attempt, break and either return last response or rethrow
                break;
            }

            var delay = TimeSpan.FromMilliseconds(_baseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
            try
            {
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        // If we have a response (possibly non-success) return it, otherwise throw
        if (response != null)
        {
            return response;
        }

        throw new HttpRequestException("Request failed after retries and no response was received.");
    }
}
