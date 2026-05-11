using System.Text.Json.Serialization;

namespace Pokedex.Api.Clients.PokeApi;

internal sealed record PokeApiSpeciesResponse(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("is_legendary")] bool IsLegendary,
    [property: JsonPropertyName("habitat")] PokeApiNamedResource? Habitat,
    [property: JsonPropertyName("flavor_text_entries")] IReadOnlyList<PokeApiFlavorTextEntry> FlavorTextEntries);

internal sealed record PokeApiNamedResource(
    [property: JsonPropertyName("name")] string Name);

internal sealed record PokeApiFlavorTextEntry(
    [property: JsonPropertyName("flavor_text")] string FlavorText,
    [property: JsonPropertyName("language")] PokeApiNamedResource Language);