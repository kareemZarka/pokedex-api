using Pokedex.Api.Clients.FunTranslations;
using Pokedex.Api.Clients.PokeApi;
using Pokedex.Api.Models;

namespace Pokedex.Api.Services;

public sealed class PokemonService : IPokemonService
{
    private const string CaveHabitat = "cave";

    private readonly IPokeApiClient _pokeApi;
    private readonly ITranslationClient _translator;

    public PokemonService(IPokeApiClient pokeApi, ITranslationClient translator)
    {
        _pokeApi = pokeApi;
        _translator = translator;
    }

    public async Task<PokemonResponse?> GetAsync(string name, CancellationToken cancellationToken = default)
    {
        var species = await _pokeApi.GetSpeciesAsync(name, cancellationToken);
        return species is null ? null : ToResponse(species, species.Description);
    }

    public async Task<PokemonResponse?> GetTranslatedAsync(string name, CancellationToken cancellationToken = default)
    {
        var species = await _pokeApi.GetSpeciesAsync(name, cancellationToken);
        if (species is null)
        {
            return null;
        }

        var translationType = ChooseTranslation(species);
        var translated = await _translator.TranslateAsync(species.Description, translationType, cancellationToken);

        // Fall back to the standard description if translation failed (brief rule #3).                                                                                                                                                                                     
        return ToResponse(species, translated ?? species.Description);
    }

    private static TranslationType ChooseTranslation(PokemonSpecies species) =>
        species.IsLegendary || string.Equals(species.Habitat, CaveHabitat, StringComparison.OrdinalIgnoreCase)
            ? TranslationType.Yoda
            : TranslationType.Shakespeare;

    private static PokemonResponse ToResponse(PokemonSpecies species, string description) =>
        new(species.Name, description, species.Habitat, species.IsLegendary);
}