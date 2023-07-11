using System.ComponentModel;

namespace JackCraftLauncher.Class.Models.ListTemplate;

public class FabricDownloadList : INotifyPropertyChanged
{
    public string version;
    public string versionType;

    public FabricDownloadList()
    {
    }

    public FabricDownloadList(string version, string versionType)
    {
        this.version = version;
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

    public string VersionType
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