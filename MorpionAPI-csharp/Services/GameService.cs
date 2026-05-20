using System.Collections.Concurrent;
using MorpionAPI_csharp.Models;

namespace MorpionAPI_csharp.Services;

public class GameService
{
    private readonly ConcurrentDictionary<Guid, Game> _games = new();
    private readonly IGameHistoryService _historyService;

    public GameService(IGameHistoryService historyService)
    {
        _historyService = historyService;
    }

    public Game CreateGame(GameMode mode)
    {
        var game = new Game { Mode = mode };
        _games[game.Id] = game;
        return game;
    }

    public Game? GetGame(Guid id) => _games.TryGetValue(id, out var g) ? g : null;

    public (Game? Game, string? Error) PlayMove(Guid id, int position)
    {
        if (!_games.TryGetValue(id, out var game))
            return (null, "Game not found");

        if (game.Status != GameStatus.InProgress)
            return (game, "Game is already finished");

        var move = game.Board.IsValidMove(position);
        if (move == null)
            return (game, "Invalid move (position must be 1-9 and cell empty)");

        game.Board.PlayMove(move.Value.line, move.Value.column, game.CurrentPlayer);

        if (CheckEndOfGame(game)) return (game, null);

        game.CurrentPlayer = game.CurrentPlayer == 'X' ? 'O' : 'X';

        if (game.Mode == GameMode.HumanVsBot && game.CurrentPlayer == 'O')
        {
            PlayBotMove(game);
        }

        return (game, null);
    }

    private void PlayBotMove(Game game)
    {
        int botIndex = game.Board.GetBotMove();
        int position = botIndex + 1;

        var move = game.Board.IsValidMove(position);
        if (move == null) return;

        game.Board.PlayMove(move.Value.line, move.Value.column, game.CurrentPlayer);

        if (CheckEndOfGame(game)) return;

        game.CurrentPlayer = game.CurrentPlayer == 'X' ? 'O' : 'X';
    }

    private bool CheckEndOfGame(Game game)
    {
        if (game.Board.CheckWin(game.CurrentPlayer))
        {
            game.Status = game.CurrentPlayer == 'X' ? GameStatus.XWon : GameStatus.OWon;
            _historyService.Add(ResultLabel(game), game.Id);
            return true;
        }

        if (game.Board.IsFull())
        {
            game.Status = GameStatus.Draw;
            _historyService.Add("Nul", game.Id);
            return true;
        }

        return false;
    }

    private static string ResultLabel(Game game)
    {
        if (game.Mode == GameMode.HumanVsBot)
            return game.Status == GameStatus.XWon ? "Victoire" : "Défaite";

        return game.Status == GameStatus.XWon ? "X gagne" : "O gagne";
    }
}
