using MorpionAPI_csharp.Models;

namespace MorpionAPI_csharp.Services;

public interface IGameHistoryService
{
    IReadOnlyList<GameHistoryEntry> History { get; }
    void Add(string result, Guid gameId);
}
