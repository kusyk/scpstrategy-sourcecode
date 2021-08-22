using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using LapinerTools.Steam.Data;
using LapinerTools.Steam;
using System;
using LapinerTools.Steam.UI;
using LapinerTools.uMyGUI;

public class UploadManager : MonoBehaviour
{
    public static UploadManager Instance;

    public Texture2D objectTesture;

    private bool devAccess = false;
    private ObjectsManager.ObjectJSONModel lastSavedObject;

    private void Awake()
    {
        Instance = this;
    }

    public void UploadObject(ObjectsManager.ObjectJSONModel objectToShare, bool _devAccess)
    {
        devAccess = _devAccess;

        if (!CheckObject(objectToShare))
        {
            ((uMyGUI_PopupText)uMyGUI_PopupManager.Instance.ShowPopup(uMyGUI_PopupManager.POPUP_TEXT))
                .SetText("Uploading Error", "<size=18>Fill in all required fields.</size>")
                .ShowButton(uMyGUI_PopupManager.BTN_OK);
            return;
        }

        PrepareDirectory();

        ObjectsManager.Instance.SaveObjectToFile(objectToShare, false, true);

        PrepareIcon();
        SendObject(objectToShare);
        //ClearDirectory();
    }

    public void SetItemId(ulong m_PublishedFileId)
    {
        lastSavedObject.steamId = m_PublishedFileId.ToString();
        ObjectsManager.Instance.SaveObjectToFile(lastSavedObject, devAccess, true);
        ObjectsManager.Instance.SaveObjectToFile(lastSavedObject, devAccess, false);
        ModsUIController.Instance.ShowObjectEditorScreen(lastSavedObject.id);
    }

    private bool CheckObject(ObjectsManager.ObjectJSONModel obj)
    {
        if (string.IsNullOrEmpty(obj.id))
            return false;

        if (string.IsNullOrEmpty(obj.steamName))
            return false;

        if (string.IsNullOrEmpty(obj.steamDescription))
            return false;

        if (string.IsNullOrEmpty(obj.alert))
            return false;

        if (obj.researchTexts.Length < 2)
            return false;

        return true;
    }

    private void PrepareDirectory()
    {
        string cachePath = ObjectsManager.CacheObjectsFullPath;

        if (Directory.Exists(cachePath))
            Directory.Delete(cachePath, true);

        Directory.CreateDirectory(cachePath);
    }

    private void PrepareIcon()
    {
        string iconPath = ObjectsManager.CacheObjectsFullPath + "icon.png";

        File.WriteAllBytes(iconPath, objectTesture.EncodeToPNG());
    }

    private void SendObject(ObjectsManager.ObjectJSONModel objectToShare)
    {
        Debug.Log(SteamUtils.GetAppID().m_AppId.ToString());

        WorkshopItemUpdate mod = new WorkshopItemUpdate();
        mod.Name = objectToShare.steamName;
        mod.Description = objectToShare.steamDescription;
        mod.IconPath = ObjectsManager.CacheObjectsFullPath + "icon.png";
        mod.ContentPath = ObjectsManager.CacheObjectsFullPath;
        mod.Tags = new List<string> { "Objects" };
        mod.ChangeNote = "-"; 

        if (string.IsNullOrEmpty(objectToShare.steamId) == false)
            mod.SteamNative.m_nPublishedFileId = new PublishedFileId_t(ulong.Parse(objectToShare.steamId));

        lastSavedObject = objectToShare;

        SteamWorkshopUIUpload.Instance.UploadWithUI(mod);
    }

    private void ClearDirectory()
    {
        string cachePath = ObjectsManager.CacheObjectsFullPath;

        if (Directory.Exists(cachePath))
            Directory.Delete(cachePath, true);
    }
}
