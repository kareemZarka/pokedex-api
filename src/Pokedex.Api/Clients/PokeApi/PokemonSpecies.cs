namespace Pokedex.Api.Clients.PokeApi;

public sealed record PokemonSpecies(
    string Name,
    string Description,
    string? Habitat,
    bool IsLegendary);