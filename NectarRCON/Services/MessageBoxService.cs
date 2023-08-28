using NectarRCON.Interfaces;
using System;
using System.Windows;

namespace NectarRCON.Services;
public class MessageBoxService : IMessageBoxService
{
    private readonly ILanguageService _languageService;

    public MessageBoxService(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    public void Show(string message)
        => MessageBox.Show(message, "NectarRcon", MessageBoxButton.OK, MessageBoxImage.Information);

    public void Show(string message, string title)
        => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

    public MessageBoxResult Show(string message, string title, MessageBoxButton button)
        => MessageBox.Show(message, title, button, MessageBoxImage.Information);

    public void Show(string message, string title, MessageBoxImage image)
        => MessageBox.Show(message, title, MessageBoxButton.OK, image);

    public MessageBoxResult Show(string message, string title, MessageBoxButton button, MessageBoxImage image)
        => MessageBox.Show(message, title, button, image);

    public void Show(Exception exception, string? information = null)
        => MessageBox.Show($"{_languageService.GetKey("text.went_wrong")}\n{information}\n{exception}", "error", MessageBoxButton.OK, MessageBoxImage.Error);
}
