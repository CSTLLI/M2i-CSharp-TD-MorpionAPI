namespace MorpionAPI_csharp.Models;

public enum GameMode
{
    HumanVsHuman,
    HumanVsBot
}

public enum GameStatus
{
    InProgress,
    XWon,
    OWon,
    Draw
}

public class Game
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public GameMode Mode { get; init; }
    public Board Board { get; } = new();
    public char CurrentPlayer { get; set; } = 'X';
    public GameStatus Status { get; set; } = GameStatus.InProgress;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public record CreateGameRequest(GameMode Mode);

public record MoveRequest(int Position);

public record GameResponse(
    Guid Id,
    GameMode Mode,
    string Board,
    char CurrentPlayer,
    GameStatus Status)
{
    public static GameResponse From(Game g) => new(
        g.Id,
        g.Mode,
        g.Board.GetSnapshot(),
        g.CurrentPlayer,
        g.Status);
}
