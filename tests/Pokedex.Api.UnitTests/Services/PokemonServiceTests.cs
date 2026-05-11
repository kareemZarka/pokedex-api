using Pokedex.Api.Clients.FunTranslations;
using Pokedex.Api.Clients.PokeApi;
using Pokedex.Api.Services;
using Xunit;

namespace Pokedex.Api.UnitTests.Services;

public class PokemonServiceTests
{
    [Fact]
    public async Task GetAsync_WhenPokemonExists_ReturnsStandardDescription()
    {
        var pokeApi = new StubPokeApiClient(MewtwoSpecies);
        var translator = new StubTranslationClient(null); // should NOT be called                                                                                                                                                                                           
        var sut = new PokemonService(pokeApi, translator);

        var result = await sut.GetAsync("mewtwo");

        Assert.NotNull(result);
        Assert.Equal("mewtwo", result!.Name);
        Assert.Equal(MewtwoSpecies.Description, result.Description);
        Assert.False(translator.WasCalled);
    }

    [Fact]
    public async Task GetAsync_WhenPokemonNotFound_ReturnsNull()
    {
        var sut = new PokemonService(new StubPokeApiClient(null), new StubTranslationClient(null));

        var result = await sut.GetAsync("unknown");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetTranslatedAsync_WhenLegendary_UsesYodaTranslation()
    {
        var translator = new StubTranslationClient("yoda-version");
        var sut = new PokemonService(new StubPokeApiClient(MewtwoSpecies), translator);

        var result = await sut.GetTranslatedAsync("mewtwo");

        Assert.Equal("yoda-version", result!.Description);
        Assert.Equal(TranslationType.Yoda, translator.LastType);
    }

    [Fact]
    public async Task GetTranslatedAsync_WhenHabitatIsCave_UsesYodaTranslation()
    {
        var caveSpecies = MewtwoSpecies with { IsLegendary = false, Habitat = "cave" };
        var translator = new StubTranslationClient("yoda-version");
        var sut = new PokemonService(new StubPokeApiClient(caveSpecies), translator);

        var result = await sut.GetTranslatedAsync("zubat");

        Assert.Equal("yoda-version", result!.Description);
        Assert.Equal(TranslationType.Yoda, translator.LastType);
    }

    [Fact]
    public async Task GetTranslatedAsync_WhenOrdinaryPokemon_UsesShakespeareTranslation()
    {
        var ordinarySpecies = MewtwoSpecies with { IsLegendary = false, Habitat = "grassland" };
        var translator = new StubTranslationClient("shakespeare-version");
        var sut = new PokemonService(new StubPokeApiClient(ordinarySpecies), translator);

        var result = await sut.GetTranslatedAsync("pikachu");

        Assert.Equal("shakespeare-version", result!.Description);
        Assert.Equal(TranslationType.Shakespeare, translator.LastType);
    }

    [Fact]
    public async Task GetTranslatedAsync_WhenTranslationFails_FallsBackToStandardDescription()
    {
        var translator = new StubTranslationClient(null); // simulates failure
        var sut = new PokemonService(new StubPokeApiClient(MewtwoSpecies), translator);

        var result = await sut.GetTranslatedAsync("mewtwo");

        Assert.Equal(MewtwoSpecies.Description, result!.Description);
    }

    [Fact]
    public async Task GetTranslatedAsync_WhenPokemonNotFound_DoesNotCallTranslator()
    {
        var translator = new StubTranslationClient("should-not-be-used");
        var sut = new PokemonService(new StubPokeApiClient(null), translator);

        var result = await sut.GetTranslatedAsync("unknown");

        Assert.Null(result);
        Assert.False(translator.WasCalled);
    }

    private static readonly PokemonSpecies MewtwoSpecies = new(
        Name: "mewtwo",
        Description: "It was created by a scientist.",
        Habitat: "rare",
        IsLegendary: true);

    private sealed class StubPokeApiClient : IPokeApiClient
    {
        private readonly PokemonSpecies? _result;
        public StubPokeApiClient(PokemonSpecies? result) => _result = result;
        public Task<PokemonSpecies?> GetSpeciesAsync(string name, CancellationToken cancellationToken = default) =>
            Task.FromResult(_result);
    }

    private sealed class StubTranslationClient : ITranslationClient
    {
        private readonly string? _result;
        public StubTranslationClient(string? result) => _result = result;
        public bool WasCalled { get; private set; }
        public TranslationType? LastType { get; private set; }

        public Task<string?> TranslateAsync(string text, TranslationType type, CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            LastType = type;
            return Task.FromResult(_result);
        }
    }
}