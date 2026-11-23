using SQLite;

namespace ThreeDEE.Models;

public class NoteDocument
{
    [PrimaryKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FolderName { get; set; } = "General";

    public string Title { get; set; } = "Untitled Note";

    public string MarkdownContent { get; set; } = string.Empty;

    public string AudioFilePath { get; set; } = string.Empty;

    public string LiveTranscript { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
