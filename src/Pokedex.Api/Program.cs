using Microsoft.Extensions.Options;
using Pokedex.Api.Clients.PokeApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services
    .AddOptions<PokeApiOptions>()
    .Bind(builder.Configuration.GetSection(PokeApiOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddHttpClient<IPokeApiClient, PokeApiClient>((sp, http) =>
{
    var opts = sp.GetRequiredService<IOptions<PokeApiOptions>>().Value;
    http.BaseAddress = new Uri(opts.BaseUrl);
    http.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();

    app.UseAuthorization();
}



app.MapControllers();

app.Run();
