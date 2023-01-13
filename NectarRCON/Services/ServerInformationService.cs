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
public class ServerInformationService : IServerInformationService
{
    private readonly ILanguageService _languageService;
    private List<ServerInformation> _servers = new();
    public List<ServerInformation> Servers 
        => _servers;
    public ServerInformationService(ILanguageService languageService)
    {
        _languageService = languageService;
        if (!File.Exists("./servers.json"))
            File.WriteAllText("./servers.json", "[]");
        try
        {
            string jsonString = File.ReadAllText("./servers.json");
            _servers = JsonSerializer.Deserialize<List<ServerInformation>>(jsonString)
                ?? new List<ServerInformation>();
        }
        catch(JsonException ex)
        {
            MessageBoxResult result = MessageBox.Show(_languageService.GetKey("text.server_information.error.json_error")
                .Replace("%s", ex.Message)
                .Replace("\\n","\n"), _languageService.GetKey("text.error"), MessageBoxButton.OKCancel, MessageBoxImage.Error);
            if (result == MessageBoxResult.OK)
                File.WriteAllText("./servers.json", "[]");
            else
                Process.GetCurrentProcess().Kill();
        }catch(Exception ex)
        {
            MessageBox.Show(_languageService.GetKey("text.server_information.error.text")
                .Replace("%s", ex.Message)
                .Replace("\\n", "\n")
                , _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);

            Process.GetCurrentProcess().Kill();
        }
    }
    public void AddServer(ServerInformation server)
    {
        if (ServerIsExist(server.Name))
            return;
        _servers.Add(server);
    }
    public ServerInformation? GetServer(string name)
        => _servers.Where(s => s.Name == name).FirstOrDefault();
    public List<ServerInformation> GetServers()
    {
        return _servers;
    }
    public void RemoveServer(string name)
    {
        ServerInformation? server = GetServer(name);
        if (null == server) return;
        _servers.Remove(server);
    }
    public void Save()
    {
        try
        {
            File.WriteAllText("./servers.json", JsonSerializer.Serialize(_servers));
        }catch(Exception ex)
        {
            MessageBox.Show(_languageService.GetKey("text.server_information.error.save")
                .Replace("%s", ex.Message)
                .Replace("\\n", "\n")
                , _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    public bool ServerIsExist(string name)
        => GetServer(name) != null;
    public void Update(string name, ServerInformation newInfo)
    {
        var oldInfo = GetServer(name);
        if (null == oldInfo) return;
        for (int i = 0; i < _servers.Count; i++)
        {
            if (_servers[i] == oldInfo)
            {
                _servers[i] = newInfo;
                return;
            }
        }
    }
}