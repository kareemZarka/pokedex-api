namespace Pokedex.Api.Clients.PokeApi;

public interface IPokeApiClient
{
    Task<PokemonSpecies?> GetSpeciesAsync(string name, CancellationToken cancellationToken = default);
}