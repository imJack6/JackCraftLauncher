using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using Avalonia;
using Avalonia.Platform;
using Newtonsoft.Json;

namespace JackCraftLauncher.Localizer;

public class Localizer : INotifyPropertyChanged
{
    private const string IndexerName = "Item";
    private const string IndexerArrayName = "Item[]";
    private Dictionary<string, string> m_Strings;

    public string Language { get; private set; }

    public string this[string key]
    {
        get
        {
            string res;
            if (m_Strings != null && m_Strings.TryGetValue(key, out res))
                return res.Replace("\\n", "\n");

            return $"{Language}:{key}";
        }
    }

    public static Localizer Instance { get; set; } = new();
    public event PropertyChangedEventHandler PropertyChanged;

    public bool LoadLanguage(string language)
    {
        Language = language;

        var uri = new Uri($"avares://JackCraftLauncher/Assets/i18n/{language}.json");
        if (AssetLoader.Exists(uri))
        {
            using (var sr = new StreamReader(AssetLoader.Open(uri), Encoding.UTF8))
            {
                m_Strings = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
            }

            Invalidate();

            return true;
        }

        return false;
    } // LoadLanguage

    public void Invalidate()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerArrayName));
    }
}