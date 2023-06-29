using Avalonia.Controls;

namespace JackCraftLauncher.Views.MainMenus;

public partial class StartGameMenu : UserControl
{
    public StartGameMenu()
    {
        InitializeComponent();
        Localizer.Localizer.Instance.LoadLanguage("zh-CN");
    }

    public void ClearStartGameLog()
    {
        StartGameLogListBox.Items.Clear();
    }

    public void AddStartGameLog(string log)
    {
        StartGameLogListBox.Items.Add(log);
        if (StartGameLogListBox.Scroll != null)
        {
            var listboxScroll = (ScrollViewer)StartGameLogListBox.Scroll;
            listboxScroll.ScrollToEnd();
        }
    }

    private void StartGameLogListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        StartGameLogListBox.SelectedIndex = -1;
    }
}