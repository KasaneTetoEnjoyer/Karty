using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Karty;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Pasjans_Click(object? sender, RoutedEventArgs e)
    {

        //new PasjansWindow().Show();
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