using Pokedex.Api.Models;

namespace Pokedex.Api.Services;

public interface IPokemonService
{
    Task<PokemonResponse?> GetAsync(string name, CancellationToken cancellationToken = default);
    Task<PokemonResponse?> GetTranslatedAsync(string name, CancellationToken cancellationToken = default);
}