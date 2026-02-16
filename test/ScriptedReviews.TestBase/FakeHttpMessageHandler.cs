using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ScriptedReviews;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly ConcurrentDictionary<string, HttpResponseMessage> _responses = new();

    public void AddResponse(string urlFragment, HttpResponseMessage response)
    {
        _responses[urlFragment] = response;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var url = request.RequestUri?.ToString() ?? string.Empty;

        // Check exact match first
        if (_responses.ContainsKey(url))
        {
            return Task.FromResult(_responses[url]);
        }

        // Check partial match
        foreach (var key in _responses.Keys)
        {
            if (url.Contains(key))
            {
                return Task.FromResult(_responses[key]);
            }
        }

        return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));
    }
}
