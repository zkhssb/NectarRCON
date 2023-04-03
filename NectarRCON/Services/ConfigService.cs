using NectarRCON.Interfaces;
using NectarRCON.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
namespace NectarRCON.Services;
public partial class ConfigService : IConfigService
{
    private ILanguageService _languageService;
    private Config _config = new();
    public ConfigService(ILanguageService languageService)
    {
        _languageService = languageService;
        if (!File.Exists("./config.json"))
            Save();
        try
        {
            _config = JsonSerializer.Deserialize<Config>(File.ReadAllText("./config.json"))
            ?? new Config();
        }
        catch (Exception ex)
        {
            var resutl = MessageBox.Show(_languageService.GetKey(ex.GetType() == typeof(JsonException) ? "text.config.json_error" : "text.config.error")
            .Replace("%s", ex.Message), _languageService.GetKey("text.error"), MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (resutl == MessageBoxResult.Yes)
            {
                _config = new();
                Save();
            }
            else
            {
                Process.GetCurrentProcess().Kill();
            }
        }
    }
    public Config GetConfig()
        => _config;
    public void Save()
    {
        try
        {
            File.WriteAllText("./config.json", JsonSerializer.Serialize(_config));
        }
        catch (Exception ex)
        {
            MessageBox.Show(_languageService.GetKey("text.config.save_error")
                .Replace("%s", ex.Message), _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}