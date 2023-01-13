using System.Collections.Generic;
using System.Windows;

namespace NectarRCON.Interfaces;
public interface ILanguageService
{
    public void Refresh();
    public ResourceDictionary GetSelectedLanguage();
    public Dictionary<string, ResourceDictionary> GetLanguages();
    public void SelectLanguage();
    public void SelectLanguage(string languageName, bool name);
    public string GetKey(string key);
}