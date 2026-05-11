using System.Text.Json.Serialization;

namespace Pokedex.Api.Clients.FunTranslations;

internal sealed record FunTranslationsResponse(
    [property: JsonPropertyName("contents")] FunTranslationsContents? Contents);

internal sealed record FunTranslationsContents(
    [property: JsonPropertyName("translated")] string? Translated);