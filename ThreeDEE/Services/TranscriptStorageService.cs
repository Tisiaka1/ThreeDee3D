using SQLite;
using ThreeDEE.Models;

namespace ThreeDEE.Services;

public class TranscriptStorageService
{
    private readonly SQLiteAsyncConnection _db;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private bool _initialized;

    public TranscriptStorageService()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "notes_v1.db3");
        _db = new SQLiteAsyncConnection(dbPath);
    }

    private async Task InitAsync()
    {
        if (_initialized) return;
        await _initializationLock.WaitAsync();
        try
        {
            if (_initialized) return;
            await _db.CreateTableAsync<TranscriptEntry>();
            _initialized = true;
        }
        finally { _initializationLock.Release(); }
    }

    public async Task<List<TranscriptEntry>> LoadAsync(Guid noteId)
    {
        await InitAsync();
        return await _db.Table<TranscriptEntry>()
            .Where(entry => entry.NoteId == noteId)
            .OrderBy(entry => entry.Timestamp)
            .ToListAsync();
    }

    public async Task AddAsync(TranscriptEntry entry)
    {
        await InitAsync();
        await _db.InsertAsync(entry);
    }

    public async Task SaveAsync(Guid noteId, IEnumerable<TranscriptEntry> entries)
    {
        await InitAsync();
        await _db.RunInTransactionAsync(conn =>
        {
            foreach (var entry in entries)
            {
                entry.NoteId = noteId;
                conn.InsertOrReplace(entry);
            }
        });
    }
}
