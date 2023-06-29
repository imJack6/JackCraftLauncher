using Avalonia.Controls;
using HarfBuzzSharp;

namespace JackCraftLauncher.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Localizer.Localizer.Instance.LoadLanguage("zh-CN");
    }
}