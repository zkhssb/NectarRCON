using NectarRCON.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace NectarRCON.Services;
public class LanguageService : ILanguageService
{
    private readonly Dictionary<string, string> _defaultLanguages = new();
    private Dictionary<string, ResourceDictionary> _languages = new();
    private ResourceDictionary _selectedLanguage = new ResourceDictionary()
    {
        Source = new Uri("pack://application:,,,/NectarRCON;component/Resources/Languages/zh_cn.xaml", UriKind.RelativeOrAbsolute)
    };
    private readonly ResourceDictionary _defLanguage = new ResourceDictionary()
    {
        Source = new Uri("pack://application:,,,/NectarRCON;component/Resources/Languages/zh_cn.xaml", UriKind.RelativeOrAbsolute)
    };
    public LanguageService()
    {
        Refresh();
    }
    public ResourceDictionary SelectedLanguage
    {
        get => _selectedLanguage;
    }
    public Dictionary<string, ResourceDictionary> Languages
    {
        get => _languages;
    }
    public Dictionary<string, ResourceDictionary> GetLanguages()
        => Languages;
    public ResourceDictionary GetSelectedLanguage()
    {
        return _selectedLanguage;
    }
    public void Refresh()
    {
        _languages.Clear();
        // 从本地目录获取xaml
        string[] files = new string[0];
        if (Directory.Exists("./languages/"))
            files = Directory.GetFiles("./languages/");
        foreach (string file in files)
        {
            if (Path.GetExtension(file).ToLower() == ".xaml" || Path.GetExtension(file).ToLower() == ".xml")
            {
                ResourceDictionary resourceDictionary = new ResourceDictionary();
                using (FileStream fs = File.OpenRead(file))
                {
                    resourceDictionary = (ResourceDictionary)System.Windows.Markup.XamlReader.Load(fs);
                }
                _languages.Add(Path.GetFileNameWithoutExtension(file), resourceDictionary);
            }
        }
        // 从内部文件加载
        _defaultLanguages.Add("zh_cn", "pack://application:,,,/NectarRCON;component/Resources/Languages/zh_cn.xaml");
        _defaultLanguages.Add("zh_tw", "pack://application:,,,/NectarRCON;component/Resources/Languages/zh_tw.xaml");
        _defaultLanguages.Add("en_us", "pack://application:,,,/NectarRCON;component/Resources/Languages/en_us.xaml");
        foreach (KeyValuePair<string, string> language in _defaultLanguages)
        {
            if (_languages.ContainsKey(language.Key))
                _languages.Remove(language.Key);
            Languages.Add(language.Key, new ResourceDictionary()
            {
                Source = new Uri(language.Value, UriKind.RelativeOrAbsolute)
            });
        }
    }
    public void SelectLanguage()
    {
        Application.Current.Resources.Remove(SelectedLanguage);
        string name = System.Globalization.CultureInfo.CurrentCulture.Name;
        SelectLanguage(name);
    }
    public void SelectLanguage(string languageName, bool name = false)
    {
        languageName = languageName.Replace("-", "_");
        Application.Current.Resources.MergedDictionaries.Remove(SelectedLanguage);
        var language = Languages.Where(l =>
        {
            if (name)
            {
                return (l.Value["file.name"].ToString() ?? "nullname") == languageName.ToLower();
            }
            else
            {
                return l.Key.ToLower() == languageName.ToLower();
            }
        }).FirstOrDefault();
        if (null == language.Value)
        {
            _selectedLanguage = new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/NectarRCON;component/Resources/Languages/en_us.xaml", UriKind.RelativeOrAbsolute)
            };
            if (Application.Current.Resources.Contains("en_us"))
            {
                Application.Current.Resources.Remove("en_us");
            }
            Application.Current.Resources.MergedDictionaries.Add(_selectedLanguage);
        }
        else
        {
            _selectedLanguage = language.Value;
            Application.Current.Resources.MergedDictionaries.Add(language.Value);
        }
    }
    public string GetKey(string key)
    {
        string valuel;
        if (_selectedLanguage.Contains(key))
            valuel = _selectedLanguage[key].ToString() ?? string.Empty;
        else if (_defLanguage.Contains(key))
            valuel = _defLanguage[key].ToString() ?? string.Empty;
        else valuel = key;
        return valuel
            .Replace("\\n", "\n");
    }
}
