using System.ComponentModel;

namespace JackCraftLauncher.Class.Models.ListTemplate;

public class DefaultDownloadList : INotifyPropertyChanged
{
    public string time;

    public string version;

    public VersionType versionType;

    public DefaultDownloadList()
    {
    }

    public DefaultDownloadList(string version, string time, VersionType versionType)
    {
        this.version = version;
        this.time = time;
        this.versionType = versionType;
    }

    public string Version
    {
        get => version;
        set
        {
            version = value;
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Age"));
        }
    }

    public string Time
    {
        get => time;
        set
        {
            time = value;
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Age"));
        }
    }

    public VersionType VersionType
    {
        get => versionType;
        set
        {
            versionType = value;
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Age"));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}