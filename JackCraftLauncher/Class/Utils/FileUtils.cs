using System.IO;

namespace JackCraftLauncher.Class.Utils;

public class FileUtils
{
    /// <summary>
    ///     拷贝文件到另一个文件夹下
    /// </summary>
    /// <param name="sourceName">源文件路径</param>
    /// <param name="folderPath">目标路径（目标文件夹）</param>
    public static void CopyToFile(string sourceName, string folderPath)
    {
        //例子：
        //源文件路径
        //string sourceName = @"D:\Source\Test.txt";
        //目标路径:项目下的NewTest文件夹,(如果没有就创建该文件夹)
        //string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NewTest");

        /*if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }*/
        DirectoryUtils.CreateDirectory(folderPath);
        //当前文件如果不用新的文件名，那么就用原文件文件名
        var fileName = Path.GetFileName(sourceName);
        //这里可以给文件换个新名字，如下：
        //string fileName = string.Format("{0}.{1}", "newFileText", "txt");

        //目标整体路径
        var targetPath = Path.Combine(folderPath, fileName);

        //Copy到新文件下
        var file = new FileInfo(sourceName);
        if (file.Exists)
            //true 为覆盖已存在的同名文件，false 为不覆盖
            file.CopyTo(targetPath, true);
    }
}