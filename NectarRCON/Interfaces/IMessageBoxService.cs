using System;
using System.Security.Cryptography;
using System.Windows;

namespace NectarRCON.Interfaces
{
    public interface IMessageBoxService
    {
        void Show(string message);
        void Show(string message, string title);
        MessageBoxResult Show(string message, string title, MessageBoxButton button);
        void Show(string message, string title, MessageBoxImage image);
        MessageBoxResult Show(string message, string title, MessageBoxButton button, MessageBoxImage image);
        void Show(Exception exception, string? information);
    }
}
