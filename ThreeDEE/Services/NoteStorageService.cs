using SQLite;
using ThreeDEE.Models;

namespace ThreeDEE.Services;

public class NoteStorageService
{
    private SQLiteAsyncConnection? _db;

    private async Task InitAsync()
    {
        if (_db is not null) return;

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "notes_v1.db3");
        _db = new SQLiteAsyncConnection(dbPath);
        await _db.CreateTableAsync<NoteDocument>();
    }

    public async Task<List<NoteDocument>> GetNotesAsync()
    {
        await InitAsync();
        return await _db!.Table<NoteDocument>()
            .OrderByDescending(n => n.UpdatedAt)
            .ToListAsync();
    }

    public async Task SaveNoteAsync(NoteDocument note)
    {
        await InitAsync();
        note.UpdatedAt = DateTime.UtcNow;
        await _db!.InsertOrReplaceAsync(note);
    }

    public async Task DeleteNoteAsync(NoteDocument note)
    {
        await InitAsync();
        await _db!.DeleteAsync(note);
    }
}