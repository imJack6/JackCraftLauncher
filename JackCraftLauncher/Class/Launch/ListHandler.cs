using System;
using System.Collections.Generic;
using System.Linq;
using JackCraftLauncher.Views.MainMenus;
using ProjBobcat.Class.Model;

namespace JackCraftLauncher.Class.Launch;

public class ListHandler
{
    public static void RefreshLocalGameList()
    {
        try
        {
            List<VersionInfo> gameList = GetLocalGameList();
            if (StartMenu.Instance != null)
            {
                if (gameList == null)
                {
                    GlobalVariable.LocalGameList = new();
                    StartMenu.Instance.LocalGameListBox.ItemsSource = 
                        GlobalVariable.LocalGameList.Select(x => x.Name ?? x.Id).ToList();
                }
                else
                {
                    if (GlobalVariable.LocalGameList != gameList)
                    {
                        GlobalVariable.LocalGameList = gameList;
                        StartMenu.Instance.LocalGameListBox.ItemsSource = 
                            GlobalVariable.LocalGameList.Select(x => x.Name ?? x.Id).ToList();
                    }
                }
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
    private static List<VersionInfo> GetLocalGameList()
    {
        List<VersionInfo> gameList = null!;
        try
        {
            gameList = App.Core.VersionLocator.GetAllGames().ToList();
        }
        catch (Exception)
        {
            // ignored
        }
        return gameList;
    }
}