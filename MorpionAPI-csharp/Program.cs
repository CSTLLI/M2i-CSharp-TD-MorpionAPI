using System.Text.Json.Serialization;
using MorpionAPI_csharp.Models;
using MorpionAPI_csharp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IGameHistoryService, InMemoryGameHistoryService>();
builder.Services.AddSingleton<GameService>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/games", (CreateGameRequest request, GameService service) =>
{
    var game = service.CreateGame(request.Mode);
    return Results.Created($"/games/{game.Id}", GameResponse.From(game));
});

app.MapGet("/games/{id:guid}", (Guid id, GameService service) =>
{
    var game = service.GetGame(id);
    return game is null ? Results.NotFound() : Results.Ok(GameResponse.From(game));
});

app.MapPost("/games/{id:guid}/moves", (Guid id, MoveRequest request, GameService service) =>
{
    var (game, error) = service.PlayMove(id, request.Position);
    if (game is null) return Results.NotFound();
    if (error is not null) return Results.BadRequest(new { error });
    return Results.Ok(GameResponse.From(game));
});

app.MapGet("/history", (IGameHistoryService history) => Results.Ok(history.History));

app.Run();
