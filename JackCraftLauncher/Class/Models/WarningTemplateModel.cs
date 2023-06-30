namespace JackCraftLauncher.Class.Models;

public class WarningTemplateModel
{
    public WarningTemplateModel(string header1,string content1)
    {
        Header1 = header1;
        Content1 = content1;
    }

    public string Header1 { get; set; }
    public string Content1 { get; set; }
}