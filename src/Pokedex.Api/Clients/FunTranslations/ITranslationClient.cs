namespace Pokedex.Api.Clients.FunTranslations;

public interface ITranslationClient
{
    /// <summary>
    /// Returns the translated text, or null if the translation failed for any reason                                                                                                                                                                                       
    /// (rate-limited, network error, malformed response, etc.).
    /// </summary>                                                                                                                                                                                                                                                          
    Task<string?> TranslateAsync(
        string text,
        TranslationType type,
        CancellationToken cancellationToken = default);
}