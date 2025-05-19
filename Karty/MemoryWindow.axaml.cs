using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Karty;

public partial class MemoryWindow : Window
{
    private List<Button> cards;
    private List<string> cardImages;
    private Button? firstCard;
    private Button? secondCard;
    private int matchesFound;
    private int moves;
    private readonly string cardFolder;

    public MemoryWindow()
    {
        InitializeComponent();
        cardFolder = Path.Combine(Environment.CurrentDirectory, "Assets", "Cards");
        cards = new List<Button>();
        cardImages = new List<string>();
        InitializeGame();
    }

    private void InitializeGame()
    {
        if (CardGrid == null || MovesText == null || ResultText == null)
        {
        }

        CardGrid.Children.Clear();
        cards.Clear();
        cardImages.Clear();
        matchesFound = 0;
        moves = 0;
        MovesText.Text = "0";
        ResultText.Text = "";

        if (!Directory.Exists(cardFolder))
        {
            Directory.CreateDirectory(cardFolder);
            return;
        }

        var allCards = Directory.GetFiles(cardFolder)
            .Where(f => !f.EndsWith("back.png"))
            .Select(f => Path.GetFileName(f))
            .ToList();

        cardImages.AddRange(allCards.Take(8)); 
        cardImages.AddRange(cardImages); 

        var random = new Random();
        cardImages = cardImages.OrderBy(_ => random.Next()).ToList(); 

        foreach (var image in cardImages)
        {
            var cardButton = new Button
            {
                Width = 100,
                Height = 150,
                Tag = image,
                Content = new Image
                {
                    Source = LoadCardImage("back.png"),
                    Width = 100,
                    Height = 150,
                    Stretch = Avalonia.Media.Stretch.UniformToFill
                }
            };
            cardButton.Click += Card_Click;
            cards.Add(cardButton);
            CardGrid.Children.Add(cardButton);
        }
    }

    private async void Card_Click(object? sender, RoutedEventArgs e)
    {
        if (firstCard != null && secondCard != null) return;
        if (sender is not Button button) return;

        ShowCard(button);

        if (firstCard == null)
        {
            firstCard = button;
        }
        else if (secondCard == null && button != firstCard)
        {
            secondCard = button;
            moves++;
            MovesText.Text = moves.ToString();
            await Task.Delay(500);
            CheckMatch();
        }
    }

    private void ShowCard(Button card)
    {
        if (card.Content is Image img)
        {
            img.Source = LoadCardImage(card.Tag?.ToString() ?? "");
        }
    }

    private void CheckMatch()
    {
        if (firstCard == null || secondCard == null) return;

        if (firstCard.Tag?.ToString() == secondCard.Tag?.ToString())
        {
            firstCard.IsEnabled = false;
            secondCard.IsEnabled = false;
            matchesFound++;

            if (matchesFound == 8)
            {
                ResultText.Text = $"Wygrales, ruchy: {moves}";
            }
        }
        else
        {
            ResetCardImage(firstCard);
            ResetCardImage(secondCard);
        }

        firstCard = null;
        secondCard = null;
    }

    private void ResetCardImage(Button card)
    {
        if (card.Content is Image img)
        {
            img.Source = LoadCardImage("back.png");
        }
    }

    private Bitmap? LoadCardImage(string fileName)
    {
        var imagePath = Path.Combine(cardFolder, fileName);
        if (File.Exists(imagePath))
            return new Bitmap(imagePath);

    }

    private void Restart_Click(object? sender, RoutedEventArgs e)
    {
        InitializeGame();
    }
}
