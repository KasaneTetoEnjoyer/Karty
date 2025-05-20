using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Karty
{
    public partial class WojnaWindow : Window
    {
        private readonly string folderPath;
        private Queue<Card> playerDeck = new();
        private Queue<Card> computerDeck = new();
        private Random random = new();
        private int turnCount = 0;
        private const int MaxTurns = 1000; 

        public WojnaWindow()
        {
            InitializeComponent();
            folderPath = Path.Combine(AppContext.BaseDirectory, "kartypng");

            var placeholder = Path.Combine(folderPath, "traszka.png");
            if (File.Exists(placeholder))
            {
                PlayerCardImage.Source = new Bitmap(placeholder);
                ComputerCardImage.Source = new Bitmap(placeholder);
                ComputerCardImage.RenderTransform = new ScaleTransform(1, -1);
            }

            InitializeDecks();
            UpdateCardCountText();
        }

        private void InitializeDecks()
        {
            List<Card> fullDeck = new();
            
            foreach (int value in Enumerable.Range(2, 13)) 
            {
               
                for (int i = 0; i < 4; i++)
                {
                    fullDeck.Add(new Card(value));
                }
            }

            Shuffle(fullDeck);

            for (int i = 0; i < fullDeck.Count; i++)
            {
                if (i % 2 == 0)
                    playerDeck.Enqueue(fullDeck[i]);
                else
                    computerDeck.Enqueue(fullDeck[i]);
            }
        }

        private void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private void PlayTurn_Click(object? sender, RoutedEventArgs e)
        {
            turnCount++;
            if (turnCount >= MaxTurns)
            {
                ScoreHistory.AddScore(AppState.ActivePlayerLogin ?? "Nieznany", "Remis", "Wojna");
                ShowMessage("REMIS");
                Graj.IsEnabled = false;


            }
            if (playerDeck.Count == 0)
            {
                ShowMessage("PRZEGRANA");
                ScoreHistory.AddScore(AppState.ActivePlayerLogin ?? "Nieznany", "Przegrana", "Wojna");
                Graj.IsEnabled = false;


            }
            if (computerDeck.Count == 0)
            {
                ShowMessage("WYGRANA");

                ScoreHistory.AddScore(AppState.ActivePlayerLogin ?? "Nieznany", "Wygrana", "Wojna");
                Graj.IsEnabled = false;

            }

                List<Card> warPile = new();
            bool resolved = false;
            int warDepth = 0;
            const int maxWarDepth = 3;

            while (!resolved && warDepth < maxWarDepth)
            {
                if (playerDeck.Count == 0 || computerDeck.Count == 0)
                    break;

                var playerCard = playerDeck.Dequeue();
                var computerCard = computerDeck.Dequeue();
                warPile.Add(playerCard);
                warPile.Add(computerCard);

                ShowCards(playerCard, computerCard);

                if (playerCard.Value > computerCard.Value)
                {
                    ResultText.Text = warDepth == 0 ? "Wygrałeś turę!" : "Wygrałeś wojnę!"  ;
                    Shuffle(warPile);
                    foreach (var c in warPile)
                        playerDeck.Enqueue(c);
                    resolved = true;
                }
                else if (playerCard.Value < computerCard.Value)
                {
                    ResultText.Text = warDepth == 0 ? "Przegrałeś turę!" : "Przegrałeś wojnę!";
                    Shuffle(warPile);
                    foreach (var c in warPile)
                        computerDeck.Enqueue(c);
                    resolved = true;
                }
                else
                {
                    ResultText.Text = "Wojna!";
                    warDepth++;
                    if (playerDeck.Count < 1 || computerDeck.Count < 1)
                    {
                        ResultText.Text = "Nie można kontynuować wojny. Koniec gry.";
                        resolved = true;
                        break;
                    }

                    
                    warPile.Add(playerDeck.Dequeue());
                    warPile.Add(computerDeck.Dequeue());
                }
            }

            if (!resolved)
            {
                ResultText.Text = "Zbyt długa wojna! Losowy podział kart.";
                Shuffle(warPile);
                foreach (var c in warPile)
                {
                    if (random.Next(2) == 0)
                        playerDeck.Enqueue(c);
                    else
                        computerDeck.Enqueue(c);
                }
            }

            UpdateCardCountText();
        }

        private void ShowCards(Card playerCard, Card computerCard)
        {
            var playerPath = Path.Combine(folderPath, playerCard.FileName);
            var computerPath = Path.Combine(folderPath, computerCard.FileName);

            PlayerCardImage.Source = File.Exists(playerPath) ? new Bitmap(playerPath) : null;
            ComputerCardImage.Source = File.Exists(computerPath) ? new Bitmap(computerPath) : null;
            ComputerCardImage.RenderTransform = new ScaleTransform(1, -1);
        }

        private void UpdateCardCountText()
        {
            CardCountText.Text = $"Twoje karty: {playerDeck.Count}   Przeciwnik: {computerDeck.Count}";
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
}

    public class Card
    {
        public int Value { get; }
        public Card(int value) => Value = value;
        public string FileName => $"{ValueToName(Value)}_of_clubs.png";

        private string ValueToName(int v) => v switch
        {
            11 => "jack",
            12 => "queen",
            13 => "king",
            14 => "ace",
            _ => v.ToString()
        };
    }
