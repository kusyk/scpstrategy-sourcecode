using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModsLoader : MonoBehaviour
{
    private readonly static string gameSteamPath = @"common\SCP_Strategy\SCP_Strategy_Data";
    private readonly static string modsSteamPath = @"workshop\content\1403020";

    public static string GetFullModsSteamPath
    {
        get
        {
            string tempString = Application.dataPath;
            tempString = tempString.Replace(gameSteamPath, "");
            tempString = tempString.Replace(gameSteamPath.Replace(@"\", "/"), "");
            tempString += modsSteamPath;

            return tempString;
        }
    }

    private void Start()
    {
        string steamPath = GetFullModsSteamPath;
        Debug.Log("Steam path: " + steamPath);

        if (Directory.Exists(steamPath) == false)
        {
            Debug.Log("Steam path not found.");
            return;
        }

        string[] subdirs = Directory.GetDirectories(steamPath);


        for (int i = 0; i < subdirs.Length; i++)
        {
            string[] scp_mod_files = Directory.GetFiles(subdirs[i], "mod_*.json", SearchOption.TopDirectoryOnly);

            if (scp_mod_files.Length == 1)
            {
                string modName = Path.GetFileNameWithoutExtension(scp_mod_files[0]);
                Directory.CreateDirectory(ObjectsManager.ModsObjectsFullPath + modName);
                File.Copy(scp_mod_files[0], ObjectsManager.ModsObjectsFullPath + modName + @"\" + modName + ".json", true);
            }
        }
    }
}
