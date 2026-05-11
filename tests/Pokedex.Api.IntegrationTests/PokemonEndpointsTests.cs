using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pokedex.Api.Clients.FunTranslations;
using Pokedex.Api.Clients.PokeApi;
using Pokedex.Api.Models;
using Xunit;

namespace Pokedex.Api.IntegrationTests;

public class PokemonEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PokemonEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Pokemon_ReturnsExpectedJson()
    {
        var species = new PokemonSpecies(
            Name: "mewtwo",
            Description: "It was created by a scientist.",
            Habitat: "rare",
            IsLegendary: true);

        var client = CreateClient(pokeApi: new StubPokeApiClient(species));

        var response = await client.GetAsync("/pokemon/mewtwo");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PokemonResponse>();
        Assert.NotNull(body);
        Assert.Equal("mewtwo", body!.Name);
        Assert.Equal("It was created by a scientist.", body.Description);
        Assert.Equal("rare", body.Habitat);
        Assert.True(body.IsLegendary);
    }

    [Fact]
    public async Task Get_TranslatedPokemon_ReturnsTranslatedDescription()
    {
        var species = new PokemonSpecies(
            Name: "mewtwo",
            Description: "It was created by a scientist.",
            Habitat: "rare",
            IsLegendary: true);

        var client = CreateClient(
            pokeApi: new StubPokeApiClient(species),
            translator: new StubTranslationClient("Created by a scientist, it was."));

        var response = await client.GetAsync("/pokemon/translated/mewtwo");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PokemonResponse>();
        Assert.NotNull(body);
        Assert.Equal("Created by a scientist, it was.", body!.Description);
    }

    [Fact]
    public async Task Get_UnknownPokemon_Returns404()
    {
        var client = CreateClient(pokeApi: new StubPokeApiClient(null));

        var response = await client.GetAsync("/pokemon/not-a-real-pokemon");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private HttpClient CreateClient(IPokeApiClient? pokeApi = null, ITranslationClient? translator = null) =>
        _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                if (pokeApi is not null)
                {
                    services.RemoveAll<IPokeApiClient>();
                    services.AddSingleton(pokeApi);
                }
                if (translator is not null)
                {
                    services.RemoveAll<ITranslationClient>();
                    services.AddSingleton(translator);
                }
            });
        }).CreateClient();

    private sealed class StubPokeApiClient(PokemonSpecies? result) : IPokeApiClient
    {
        public Task<PokemonSpecies?> GetSpeciesAsync(string name, CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    private sealed class StubTranslationClient(string? result) : ITranslationClient
    {
        public Task<string?> TranslateAsync(string text, TranslationType type, CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }
}
