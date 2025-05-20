using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;


namespace Karty
{
    public partial class Historia : Window
    {
        public Historia()
        {
            InitializeComponent();
            UpdateScoreDisplay();
        }

        private void UpdateScoreDisplay()
        {
            var scoreTextBlock = this.FindControl<TextBlock>("ScoreTextBlock");

            var sb = new StringBuilder();
            sb.AppendLine($"{"Gracz",-20}{"Wynik",10}{"Gra",15}");
            sb.AppendLine(new string('-', 50));

            foreach (var entry in ScoreHistory.GetAllScores())
            {
                sb.AppendLine($"{entry.PlayerName,-20}{entry.Score,10}{entry.GameName,15}");
            }

            scoreTextBlock.Text = sb.ToString();
        }

        private void Close_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

public static class ScoreHistory
{
    private static readonly string SavePath = Path.Combine(Environment.CurrentDirectory, "score_history.json");
    private static List<ScoreEntry> entries = new();

    static ScoreHistory()
    {
        LoadFromFile();
    }

    public static void AddScore(string player, string score, string game)
    {
        entries.Add(new ScoreEntry
        {
            PlayerName = player,
            Score = score,
            GameName = game
        });

        SaveToFile();
    }

    public static List<ScoreEntry> GetAllScores() => entries;

    private static void SaveToFile()
    {
        try
        {
            var json = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SavePath, json);
        }
        catch
        {
            // Mo¿na zalogowaæ b³¹d, jeœli potrzeba
        }
    }

    private static void LoadFromFile()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                var json = File.ReadAllText(SavePath);
                var loaded = JsonSerializer.Deserialize<List<ScoreEntry>>(json);
                if (loaded != null)
                {
                    entries = loaded;
                }
            }
            catch
            {
                // Mo¿na zalogowaæ b³¹d
                entries = new List<ScoreEntry>();
            }
        }
    }

    public class ScoreEntry
    {
        public string PlayerName { get; set; } = string.Empty;
        public string Score { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
    }
}


