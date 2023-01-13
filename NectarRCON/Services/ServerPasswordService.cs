using NectarRCON.Interfaces;
using NectarRCON.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace NectarRCON.Services;
public class ServerPasswordService : IServerPasswordService
{
    private readonly ILanguageService _languageService;
    private ServerInformation? _selectedServer;
    private List<ServerPassword> _serverPasswordList = new();
    public ServerPasswordService(ILanguageService languageService)
    {
        _languageService = languageService;
        if (!File.Exists("./passwords.json"))
            File.WriteAllText("./passwords.json", "[]");
        try
        {
            string jsonString = File.ReadAllText("./passwords.json");
            _serverPasswordList = JsonSerializer.Deserialize<List<ServerPassword>>(jsonString)
                ?? new List<ServerPassword>();
        }
        catch (JsonException ex)
        {
            MessageBoxResult result = MessageBox.Show(_languageService.GetKey("text.password.error.json_error")
                .Replace("%s", ex.Message)
                .Replace("\\n", "\n"), _languageService.GetKey("text.error"), MessageBoxButton.OKCancel, MessageBoxImage.Error);
            if (result == MessageBoxResult.OK)
                File.WriteAllText("./passwords.json", "[]");
            else
                Process.GetCurrentProcess().Kill();
        }
        catch (Exception ex)
        {
            MessageBox.Show(_languageService.GetKey("text.password.error.text")
                .Replace("%s", ex.Message)
                .Replace("\\n", "\n")
                , _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);

            Process.GetCurrentProcess().Kill();
        }
    }
    public ServerPassword? Get(ServerInformation server)
    {
        var password = _serverPasswordList.Where(s =>
        {
            return s.ServerName == server.Name;
        }).FirstOrDefault();
        return password;
    }

    public ServerInformation GetSelect()
    {
        return _selectedServer ?? new();
    }

    public bool IsExist(ServerInformation server)
        => Get(server) != null;

    public void Remove(ServerInformation server)
    {
        ServerPassword? password = Get(server);
        if (password == null)
            return;
        _serverPasswordList.Remove(password);
    }

    public void Save()
    {
        try
        {
            File.WriteAllText("./passwords.json", JsonSerializer.Serialize(_serverPasswordList));
        }
        catch (Exception ex)
        {
            MessageBox.Show(_languageService.GetKey("text.password.error.save")
                .Replace("%s", ex.Message)
                .Replace("\\n", "\n")
                , _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void Select(ServerInformation server)
    {
        _selectedServer = server;
    }

    public void Set(ServerInformation server, string? password, bool? isEmpty)
    {
        var oldPassword = Get(server);
        ServerPassword newPassword;
        if (oldPassword == null)
        {
            newPassword = new ServerPassword()
            {
                Password = password ?? string.Empty,
                IsEmpty = isEmpty ?? string.IsNullOrEmpty(password),
                ServerName = server.Name
            };
        }
        else
        {
            newPassword = new ServerPassword()
            {
                Password = password ?? oldPassword.Password,
                IsEmpty = isEmpty ?? oldPassword.IsEmpty,
                ServerName = server.Name
            };
        }
        if (IsExist(server))
            _serverPasswordList.Remove(Get(server)!);
        _serverPasswordList.Add(newPassword);
    }
}
