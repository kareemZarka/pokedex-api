using System.Net;
using System.Text;
using Pokedex.Api.Clients.PokeApi;

namespace Pokedex.Api.UnitTests.Clients.PokeApi;

public class PokeApiClientTests
{
    private const string MewtwoJson = """
      {
        "name": "mewtwo",
        "is_legendary": true,
        "habitat": { "name": "rare" },
        "flavor_text_entries": [                                                                                                                                                                                                                                              
          {
            "flavor_text": "Japanese description here.",                                                                                                                                                                                                                      
            "language": { "name": "ja" }                                                                                                                                                                                                                                      
          },
          {                                                                                                                                                                                                                                                                   
            "flavor_text": "It was created by\na scientist after\fyears of  horrific gene splicing.",
            "language": { "name": "en" }                                                                                                                                                                                                                                      
          }
        ]                                                                                                                                                                                                                                                                     
      }                                                            
      """;

    [Fact]
    public async Task GetSpeciesAsync_WhenSpeciesExists_ReturnsMappedSpeciesWithCleanedDescription()
    {
        var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(MewtwoJson, Encoding.UTF8, "application/json"),
        });
        var http = new HttpClient(handler) { BaseAddress = new Uri("https://pokeapi.co/api/v2/") };
        var sut = new PokeApiClient(http);

        var result = await sut.GetSpeciesAsync("Mewtwo");

        Assert.NotNull(result);
        Assert.Equal("mewtwo", result!.Name);
        Assert.Equal("rare", result.Habitat);
        Assert.True(result.IsLegendary);
        Assert.Equal(
            "It was created by a scientist after years of horrific gene splicing.",
            result.Description);
        Assert.Equal(
            "https://pokeapi.co/api/v2/pokemon-species/mewtwo",
            handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetSpeciesAsync_WhenNotFound_ReturnsNull()
    {
        var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var http = new HttpClient(handler) { BaseAddress = new Uri("https://pokeapi.co/api/v2/") };
        var sut = new PokeApiClient(http);

        var result = await sut.GetSpeciesAsync("unknown");

        Assert.Null(result);
    }
}