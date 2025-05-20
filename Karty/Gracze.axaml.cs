using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.IO;

namespace Karty;

public partial class Gracze : Window
{
    private List<Player> players = new();
    private readonly string savePath;

    public Gracze()
    {
        InitializeComponent();


        savePath = Path.Combine(Environment.CurrentDirectory, "players.json");

        LoadPlayers();
        UpdatePlayerList();
    }

    private void AddPlayer_Click(object? sender, RoutedEventArgs e)
    {
        var login = LoginTextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(login))
            return;


        var newId = players.Any() ? players.Max(p => p.Id) + 1 : 1;
        players.Add(new Player { Id = newId, Login = login });

        SavePlayers();
        UpdatePlayerList();
        LoginTextBox.Text = string.Empty;
    }

    private void UpdatePlayerList()
    {
        PlayerListBox.ItemsSource = players.Select(p => $"{p.Id}: {p.Login}").ToList();
    }

    private void LoadPlayers()
    {
        if (File.Exists(savePath))
        {
            try
            {
                var json = File.ReadAllText(savePath);
                var loaded = JsonSerializer.Deserialize<List<Player>>(json);
                if (loaded != null)
                    players = loaded;
            }
            catch
            { /* Ignoruj b³êdy */ }
        }
    }

    private void SavePlayers()
    {
        try
        {
            var json = JsonSerializer.Serialize(players, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(savePath, json);
        }
        catch { /* Ignoruj b³êdy */
            }
        }

        private void Close_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SetActivePlayer_Click(object? sender, RoutedEventArgs e)
        {
            if (PlayerListBox.SelectedIndex >= 0 && PlayerListBox.SelectedIndex < players.Count)
            {
                ShowMessage("Wybrano Gracza");
            }
            else
            {
                ShowMessage("Nie wybrano gracza.");
            }
        }
        private void ShowMessage(string message)
        {
            var dialog = new Window
            {
                Title = "Info",
                Width = 300,
                Height = 100,
                Content = new TextBlock
                {
                    Text = message,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                }
            };

            dialog.ShowDialog(this);
        }

    }

    public class Player
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
    }
    public static class AppState
    {
        public static string? ActivePlayerLogin { get; set; }
    }