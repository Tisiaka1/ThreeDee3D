using ThreeDEE.Models;
using ThreeDEE.Services;

namespace ThreeDEE;

public partial class MainPage : ContentPage
{
    private const double DividerWidth = 12;
    private const double MinimumPaneWidth = 120;
    private double _startingLeftWidth;
    private readonly NoteStorageService _noteStorage = new();
    private NoteDocument _currentNote = new();

    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var notes = await _noteStorage.GetNotesAsync();
        _currentNote = notes.FirstOrDefault() ?? new NoteDocument();
        NotesEditor.Text = _currentNote.MarkdownContent;
        NoteStatusLabel.Text = notes.Count > 0 ? "Loaded saved note" : "New note";
    }

    private void OnNotesTextChanged(object? sender, TextChangedEventArgs e)
    {
        _currentNote.MarkdownContent = e.NewTextValue ?? string.Empty;
        NoteStatusLabel.Text = "Unsaved changes";
    }

    private async void OnSaveNoteClicked(object? sender, EventArgs e)
    {
        await _noteStorage.SaveNoteAsync(_currentNote);
        NoteStatusLabel.Text = $"Saved {DateTime.Now:HH:mm:ss}";
    }

    private void OnDividerPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _startingLeftWidth = LeftColumn.Width.Value /
                    (LeftColumn.Width.Value + RightColumn.Width.Value) *
                    Math.Max(0, SplitLayout.Width - DividerWidth);
                break;

            case GestureStatus.Running:
                var availableWidth = SplitLayout.Width - DividerWidth;
                if (availableWidth <= MinimumPaneWidth * 2)
                    return;

                var leftWidth = Math.Clamp(
                    _startingLeftWidth + e.TotalX,
                    MinimumPaneWidth,
                    availableWidth - MinimumPaneWidth);
                var leftWeight = leftWidth / availableWidth;
                LeftColumn.Width = new GridLength(leftWeight, GridUnitType.Star);
                RightColumn.Width = new GridLength(1 - leftWeight, GridUnitType.Star);
                break;
        }
    }
}
