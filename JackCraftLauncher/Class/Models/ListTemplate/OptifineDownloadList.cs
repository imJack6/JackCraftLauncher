using System.ComponentModel;

namespace JackCraftLauncher.Class.Models.ListTemplate;

public class OptifineDownloadList : INotifyPropertyChanged
{
    public string version;
    public string versionTypeAndForge;

    public OptifineDownloadList()
    {
    }

    public OptifineDownloadList(string version, string versionTypeAndForge)
    {
        this.version = version;
        this.versionTypeAndForge = versionTypeAndForge;
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

    public string VersionTypeAndForge
    {
        get => versionTypeAndForge;
        set
        {
            versionTypeAndForge = value;
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Age"));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}