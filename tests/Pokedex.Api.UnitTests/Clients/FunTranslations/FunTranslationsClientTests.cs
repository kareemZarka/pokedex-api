using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Pokedex.Api.Clients.FunTranslations;

namespace Pokedex.Api.UnitTests.Clients.FunTranslations;

public class FunTranslationsClientTests
{
    private const string YodaResponseJson = """
      {                                                                                                                                                                                                                                                                       
        "success": { "total": 1 },                                        
        "contents": {                                                                                                                                                                                                                                                         
          "translated": "Created by a scientist, it was.",         
          "text": "It was created by a scientist.",
          "translation": "yoda"                                                                                                                                                                                                                                               
        }                                                                   
      }                                                                                                                                                                                                                                                                       
      """;

    [Fact]
    public async Task TranslateAsync_WhenApiSucceeds_ReturnsTranslatedText()
    {
        var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(YodaResponseJson, Encoding.UTF8, "application/json"),
        });
        var sut = BuildSut(handler);

        var result = await sut.TranslateAsync("It was created by a scientist.", TranslationType.Yoda);

        Assert.Equal("Created by a scientist, it was.", result);
        Assert.Equal(
            "https://funtranslations.mercxry.me/translate/yoda.json",
            handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
    }

    [Fact]
    public async Task TranslateAsync_WhenRateLimited_ReturnsNull()
    {
        var handler = new TestHttpMessageHandler(_ => new HttpResponseMessage((HttpStatusCode)429));
        var sut = BuildSut(handler);

        var result = await sut.TranslateAsync("anything", TranslationType.Shakespeare);

        Assert.Null(result);
    }

    [Fact]
    public async Task TranslateAsync_WhenNetworkFails_ReturnsNull()
    {
        var handler = new TestHttpMessageHandler(_ => throw new HttpRequestException("connection refused"));
        var sut = BuildSut(handler);

        var result = await sut.TranslateAsync("anything", TranslationType.Yoda);

        Assert.Null(result);
    }

    private static FunTranslationsClient BuildSut(TestHttpMessageHandler handler) =>
        new(
            new HttpClient(handler) { BaseAddress = new Uri("https://funtranslations.mercxry.me/") },
            NullLogger<FunTranslationsClient>.Instance);
}