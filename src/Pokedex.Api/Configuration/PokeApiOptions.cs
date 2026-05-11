public sealed class PokeApiOptions
{
    public const string SectionName = "PokeApi";

    public required string BaseUrl { get; init; }
    public int TimeoutSeconds { get; init; } = 10;
}