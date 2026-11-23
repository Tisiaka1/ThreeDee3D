using SQLite;

namespace ThreeDEE.Models;

/// <summary>A persisted segment of live speech transcription associated with a note.</summary>
public class TranscriptEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public Guid NoteId { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
