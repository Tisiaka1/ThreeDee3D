using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ThreeDEE.Models;
using ThreeDEE.Services;
using System.Collections.ObjectModel;

namespace ThreeDEE.ViewModels;

public partial class NoteEditorViewModel : ObservableObject
{
    private readonly NoteStorageService _storage;
    private readonly TranscriptStorageService _transcriptStorage;

    public NoteEditorViewModel(NoteStorageService storage, TranscriptStorageService transcriptStorage)
    {
        _storage = storage;
        _transcriptStorage = transcriptStorage;
    }

    public ObservableCollection<TranscriptEntry> TranscriptEntries { get; } = new();

    [ObservableProperty]
    private NoteDocument _currentNote = new();

    [ObservableProperty]
    private bool _isToolboxOpen = true;

    [ObservableProperty]
    private double _noteFontSize = 16;

    [ObservableProperty]
    private string _noteFontFamily = "OpenSansRegular";

    [ObservableProperty]
    private string _statusText = "Unsaved";

    [RelayCommand]
    private void ToggleToolbox() => IsToolboxOpen = !IsToolboxOpen;

    public async Task OpenNoteAsync(Guid noteId)
    {
        var note = (await _storage.GetNotesAsync()).FirstOrDefault(item => item.Id == noteId);
        if (note is null) return;

        CurrentNote = note;
        TranscriptEntries.Clear();
        foreach (var entry in await _transcriptStorage.LoadAsync(noteId))
            TranscriptEntries.Add(entry);
        StatusText = "Loaded";
    }

    public async Task AddTranscriptAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        // Ensure the parent note exists before recording transcript rows for it.
        await _storage.SaveNoteAsync(CurrentNote);

        var entry = new TranscriptEntry
        {
            NoteId = CurrentNote.Id,
            Text = text.Trim(),
            Timestamp = DateTime.UtcNow
        };
        await _transcriptStorage.AddAsync(entry);
        TranscriptEntries.Add(entry);
        StatusText = "Modified";
    }

    [RelayCommand]
    private void SetHeaderPrefix()
    {
        string current = CurrentNote.MarkdownContent ?? string.Empty;
        CurrentNote.MarkdownContent = current.StartsWith("# ") ? current : $"# {current}";
        OnPropertyChanged(nameof(CurrentNote));
        StatusText = "Modified";
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await _storage.SaveNoteAsync(CurrentNote);
        await _transcriptStorage.SaveAsync(CurrentNote.Id, TranscriptEntries);
        StatusText = $"Saved {DateTime.Now:HH:mm:ss}";
    }
}
