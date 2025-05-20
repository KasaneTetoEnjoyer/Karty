using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.Text;

namespace Karty;

public partial class Historia : Window
{
    private readonly List<ScoreEntry> scoreEntries = new();

    public Historia()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    public void AddScore(int score)
    {
        var player = AppState.ActivePlayerLogin ?? "Nieznany";
        scoreEntries.Add(new ScoreEntry { PlayerName = player, Score = score });
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        var scoreTextBlock = this.FindControl<TextBlock>("ScoreTextBlock");

        var sb = new StringBuilder();
        sb.AppendLine($"{"Gracz",-20}{"Wynik",10}");
        sb.AppendLine(new string('-', 30));

        foreach (var entry in scoreEntries)
        {
            sb.AppendLine($"{entry.PlayerName,-20}{entry.Score,10}");
        }

        scoreTextBlock.Text = sb.ToString();
    }

    private void Close_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    public class ScoreEntry
    {
        public string PlayerName { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}
