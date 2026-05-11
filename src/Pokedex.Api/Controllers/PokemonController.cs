using Microsoft.AspNetCore.Mvc;
using Pokedex.Api.Models;
using Pokedex.Api.Services;

namespace Pokedex.Api.Controllers;

[ApiController]
[Route("pokemon")]
public sealed class PokemonController : ControllerBase
{
    private readonly IPokemonService _service;

    public PokemonController(IPokemonService service)
    {
        _service = service;
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<PokemonResponse>> Get(string name, CancellationToken cancellationToken)
    {
        var pokemon = await _service.GetAsync(name, cancellationToken);
        return pokemon is null ? NotFound() : Ok(pokemon);
    }

    [HttpGet("translated/{name}")]
    public async Task<ActionResult<PokemonResponse>> GetTranslated(string name, CancellationToken cancellationToken)
    {
        var pokemon = await _service.GetTranslatedAsync(name, cancellationToken);
        return pokemon is null ? NotFound() : Ok(pokemon);
    }
}