namespace MorpionAPI_csharp.Models;

public class GameHistoryEntry
{
    public string Result { get; set; } = "";
    public string Date { get; set; } = "";
    public Guid GameId { get; set; }
    public string DisplayText => $"{Date} - {Result}";
}
