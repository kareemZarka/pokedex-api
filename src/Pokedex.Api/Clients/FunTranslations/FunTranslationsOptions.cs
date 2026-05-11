namespace Pokedex.Api.Configuration;

public sealed class FunTranslationsOptions
{
    public const string SectionName = "FunTranslations";

    public required string BaseUrl { get; init; }
    public int TimeoutSeconds { get; init; } = 5;
}