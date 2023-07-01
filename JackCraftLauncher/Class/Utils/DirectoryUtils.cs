﻿using System.IO;

namespace JackCraftLauncher.Class.Utils;

public abstract class DirectoryUtils
{
    public static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                throw new Exception($"创建文件夹 {path} 失败: {ex}");
            }
    }
}