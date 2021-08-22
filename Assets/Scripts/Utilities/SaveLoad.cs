using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    public static PlayerInfo LoadedPlayerInfo;
    public static string NewPlayerInfoName = "tempDevGame";

    public static void Save(PlayerInfo playerInfo)
    {
        CheckSavesPath();

        string jsonContent = JsonUtility.ToJson(playerInfo, true);
        File.WriteAllText(Application.persistentDataPath + "/Saves/" + playerInfo.gamesaveid + ".scp", jsonContent);
    }

    public static PlayerInfo Load(string playerInfoId)
    {
        CheckSavesPath();

        try
        {
            string jsonContent = File.ReadAllText(Application.persistentDataPath + "/Saves/" + playerInfoId + ".scp");
            LoadedPlayerInfo = JsonUtility.FromJson<PlayerInfo>(jsonContent);
            return LoadedPlayerInfo;
        }
        catch
        {
            NewPlayerInfoName = playerInfoId;
            return null;
        }

    }

    public static void Delete(string playerInfoId)
    {
        CheckSavesPath();

        if (File.Exists(Application.persistentDataPath + "/Saves/" + playerInfoId + ".scp"))
            File.Delete(Application.persistentDataPath + "/Saves/" + playerInfoId + ".scp");
    }

    public static void CheckSavesPath()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves/");
        }
    }
}
