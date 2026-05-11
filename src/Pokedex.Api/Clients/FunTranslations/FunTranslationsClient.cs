
namespace Pokedex.Api.Clients.FunTranslations;

public sealed class FunTranslationsClient : ITranslationClient
{
    private readonly HttpClient _http;
    private readonly ILogger<FunTranslationsClient> _logger;

    public FunTranslationsClient(HttpClient http, ILogger<FunTranslationsClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<string?> TranslateAsync(
        string text,
        TranslationType type,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var path = type switch
        {
            TranslationType.Shakespeare => "translate/shakespeare.json",
            TranslationType.Yoda => "translate/yoda.json",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported translation type."),
        };

        try
        {
            using var body = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("text", text) });
            using var response = await _http.PostAsync(path, body, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "FunTranslations returned {StatusCode} for {TranslationType}; falling back to original.",
                    response.StatusCode, type);
                return null;
            }

            var payload = await response.Content.ReadFromJsonAsync<FunTranslationsResponse>(cancellationToken);
            return payload?.Contents?.Translated;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogWarning(ex,
                "FunTranslations call failed for {TranslationType}; falling back to original.", type);
            return null;
        }
    }
}