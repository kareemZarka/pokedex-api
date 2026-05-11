namespace Pokedex.Api.Models;

public record PokemonResponse(
    string Name,
    string Description,
    string? Habitat,
    bool IsLegendary);