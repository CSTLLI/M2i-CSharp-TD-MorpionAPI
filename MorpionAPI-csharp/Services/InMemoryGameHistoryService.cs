using MorpionAPI_csharp.Models;

namespace MorpionAPI_csharp.Services;

public class InMemoryGameHistoryService : IGameHistoryService
{
    private readonly List<GameHistoryEntry> _history = new();
    private readonly object _lock = new();

    public IReadOnlyList<GameHistoryEntry> History
    {
        get { lock (_lock) return _history.ToList(); }
    }

    public void Add(string result, Guid gameId)
    {
        lock (_lock)
        {
            _history.Insert(0, new GameHistoryEntry
            {
                Result = result,
                Date = DateTime.Now.ToString("dd/MM/yyyy"),
                GameId = gameId
            });
        }
    }
}
