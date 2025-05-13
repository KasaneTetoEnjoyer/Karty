using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Karty;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void HighorLow_Click(object? sender, RoutedEventArgs e)
    {
        new HighorLow().Show();
        
    }

    private void Wojna_Click(object? sender, RoutedEventArgs e)
    {
        new WojnaWindow().Show();
    }

    private void Makao_Click(object? sender, RoutedEventArgs e)
    {
        //new MakaoWindow().Show();
    }
}