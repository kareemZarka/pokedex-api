using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Pokedex.Api.Clients.PokeApi;

public sealed class PokeApiClient : IPokeApiClient
{
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    private readonly HttpClient _http;

    public PokeApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<PokemonSpecies?> GetSpeciesAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalized = Uri.EscapeDataString(name.Trim().ToLowerInvariant());
        var response = await _http.GetAsync($"pokemon-species/{normalized}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<PokeApiSpeciesResponse>(cancellationToken)
            ?? throw new InvalidOperationException("PokéAPI returned an empty response body.");

        return new PokemonSpecies(
            Name: payload.Name,
            Description: ExtractEnglishDescription(payload.FlavorTextEntries),
            Habitat: payload.Habitat?.Name,
            IsLegendary: payload.IsLegendary);
    }

    private static string ExtractEnglishDescription(IReadOnlyList<PokeApiFlavorTextEntry> entries)
    {
        var english = entries.FirstOrDefault(e => e.Language.Name == "en");
        if (english is null)
        {
            return string.Empty;
        }

        // PokéAPI flavor text contains newlines (\n) and form-feed (\f) characters
        // that exist because the original game text was formatted for fixed-width screens.
        return WhitespaceRegex.Replace(english.FlavorText, " ").Trim();
    }
}