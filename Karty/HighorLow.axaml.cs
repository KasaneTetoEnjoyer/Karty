using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;

namespace Karty
{
    public partial class HighorLow : Window
    {
        private List<PlayingCard> deck;
        private PlayingCard currentCard;
        private int score = 0;
        private string cardFolder;

        public HighorLow()
        {
            InitializeComponent();

            cardFolder = Path.Combine(Environment.CurrentDirectory, "Assets", "Cards");

            HigherButton.Click += (_, _) => MakeGuess(true);
            LowerButton.Click += (_, _) => MakeGuess(false);

            // Start gry po za³adowaniu UI (po otwarciu okna)
            this.Opened += (_, _) => StartGame();
        }

        private void StartGame()
        {
            deck = GenerateDeck();
            Shuffle(deck);
            DrawFirstCard();
            DeckImage.Source = LoadImage("back.png");
        }

        private void DrawFirstCard()
        {
            if (deck.Count > 0)
            {
                currentCard = deck[0];
                deck.RemoveAt(0);
                ShowCard(currentCard);
            }
        }

        private void MakeGuess(bool guessHigher)
        {
            if (deck.Count == 0)
            {
                ShowMessage("Koniec gry!");
                return;
            }

            var nextCard = deck[0];
            deck.RemoveAt(0);
            ShowCard(nextCard);

            if ((guessHigher && nextCard.Value > currentCard.Value) ||
                (!guessHigher && nextCard.Value < currentCard.Value) ||
                (nextCard.Value == currentCard.Value))
            {
                score++;
            }

            ScoreText.Text = $"Wynik: {score}";
            currentCard = nextCard;

            if (deck.Count == 0)
            {
                ShowMessage("Koniec talii! Twój wynik: " + score);
            }
        }

        private void ShowCard(PlayingCard card)
        {
            string fileName = $"{card.Suit}_{card.Value}.png";
            CurrentCardImage.Source = LoadImage(fileName);
        }

        private Bitmap? LoadImage(string fileName)
        {
            var path = Path.Combine(cardFolder, fileName);
            if (File.Exists(path))
                return new Bitmap(path);
            return null;
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

        private List<PlayingCard> GenerateDeck()
        {
            var suits = new[] { "Hearts", "Diamonds", "Clubs", "Spades" };
            var deck = new List<PlayingCard>();
            foreach (var suit in suits)
            {
                for (int value = 2; value <= 14; value++) // 11–14 = JQKA
                {
                    deck.Add(new PlayingCard { Suit = suit, Value = value });
                }
            }
            return deck;
        }

        private void Shuffle<T>(IList<T> list)
        {
            var rng = new Random();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }

    public class PlayingCard
    {
        public string Suit { get; set; } = "";
        public int Value { get; set; }
    }
}
