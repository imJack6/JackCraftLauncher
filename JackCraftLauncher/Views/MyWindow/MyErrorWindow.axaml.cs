using Avalonia.Controls;
using Avalonia.Interactivity;
using JackCraftLauncher.Class.Models.ErrorModels;

namespace JackCraftLauncher.Views.MyWindow;

public partial class MyErrorWindow : Window
{
    public MyErrorWindow(ErrorResult errorResult)
    {
        InitializeComponent();
        ErrorMessageTextBox.Text =
            $"{Localizer.Localizer.Instance["Error"]}: {errorResult.ErrorMessage.Error}{Environment.NewLine}";
        ErrorMessageTextBox.Text +=
            $"{Localizer.Localizer.Instance["ErrorType"]}: {errorResult.ErrorType}{Environment.NewLine}";
        ErrorMessageTextBox.Text +=
            $"{Localizer.Localizer.Instance["ErrorMessage"]}: {errorResult.ErrorMessage.ErrorMsg}{Environment.NewLine}";
        ErrorMessageTextBox.Text +=
            $"{Localizer.Localizer.Instance["RepairMeasures"]}: {errorResult.ErrorMessage.Fix}{Environment.NewLine}";
        ErrorMessageTextBox.Text += $"----------------------------------{Environment.NewLine}";
        ErrorMessageTextBox.Text +=
            $"{Localizer.Localizer.Instance["ErrorDetails"]}: {Environment.NewLine}{FormatException(errorResult.ErrorMessage.Exception)}";
        Title =
            $"{Localizer.Localizer.Instance["JackCraftLauncherEncounteredError"]} - {errorResult.ErrorMessage.Error} - {errorResult.ErrorType}";
    }

    public static string FormatException(Exception e)
    {
        var output = "";
        output += $"{Localizer.Localizer.Instance["Application"]}: " + AppDomain.CurrentDomain.FriendlyName +
                  Environment.NewLine;
        output += $"{Localizer.Localizer.Instance["Time"]}: " + DateTime.Now + Environment.NewLine +
                  Environment.NewLine;
        output += $"{Localizer.Localizer.Instance["Message"]}: " + e.Message + Environment.NewLine;
        output += e.ToString();
        return output;
    }

    public static MyErrorWindow CreateErrorWindow(ErrorType errorType, string error, string errorMsg, string fix,
        Exception ex)
    {
        var errorWindow = new MyErrorWindow(new ErrorResult
            {
                ErrorType = errorType,
                ErrorMessage = new ErrorMessage
                {
                    Error = error,
                    ErrorMsg = errorMsg,
                    Fix = fix,
                    Exception = ex
                }
            }
        );
        errorWindow.Show();
        return errorWindow;
    }

    private async void CopyButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await Clipboard!.SetTextAsync(ErrorMessageTextBox.Text);
    }

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}