using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class ObjectsManager : MonoBehaviour
{
    public static ObjectsManager Instance;

    private readonly static string gameSceneName = "GameGlobe";
    private readonly static string gameObjectsPath = @"\Game\Objects\";
    private readonly static string modsObjectsPath = @"\Mods\Objects\";
    private readonly static string cacheObjectsPath = @"\Cache\";
    private readonly static string gameObjectsPrefix = "game_object_";
    private readonly static string modsObjectsPrefix = "mod_";

    public static string GameObjectsFullPath { get => Application.streamingAssetsPath + gameObjectsPath; }
    public static string ModsObjectsFullPath { get => Application.streamingAssetsPath + modsObjectsPath; }
    public static string CacheObjectsFullPath { get => Application.streamingAssetsPath + cacheObjectsPath; }


    public List<ObjectData> objects = new List<ObjectData>();
    public List<string> disabledObjects = new List<string>(); //TODO zapisywać listę wyłączonych modów do pliku 

    void Awake()
    {
        Instance = this; //poprawic to!!

        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == gameSceneName)
        {
            ClearObjectsList();
            LoadObjectsFromPath(GameObjectsFullPath);
            LoadObjectsFromPath(ModsObjectsFullPath);
            RemoveDisabledObjects();
        }
    }
    /// <summary> 
    /// Returns object's id when new directory was created or null when was not. 
    /// </summary>
    public string CreateObject(string id, bool devMode)
    {
        string fullId = id;
        string dirPath;

        if (devMode)
        {
            fullId = gameObjectsPrefix + fullId;
            dirPath = GameObjectsFullPath + fullId;
        }
        else
        {
            fullId = modsObjectsPrefix + UnityEngine.Random.Range(10000, 99999) + "_" + fullId;
            dirPath = ModsObjectsFullPath + fullId;
        }

        if (Directory.Exists(dirPath))
        {
            UnityEngine.Debug.LogWarning("Directory like yours exists.");
            return null;
        }

        try
        {
            Directory.CreateDirectory(dirPath);
            ObjectData tempObjectData = new ObjectData(fullId);
            string json = JsonUtility.ToJson(tempObjectData.json);
            File.WriteAllText(dirPath + "/" + fullId + ".json", json);
            objects.Add(tempObjectData);
            return fullId;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning("I cannot create new dir. Sth is wrong bro!.\n" + e);
            return null;
        }
    }

    public void SaveObjectToFile(ObjectJSONModel jsonModel, bool devMode, bool cache = false)
    {
        string fullId = jsonModel.id;

        int objectIndex = GetObjectIndexById(fullId);

        if(objectIndex == -1)
        {
            UnityEngine.Debug.LogError("File like that one does not exist.");
            return;
        }

        objects[objectIndex].json = jsonModel;

        string dirPath;

        if (cache)
            dirPath = CacheObjectsFullPath;
        else if (devMode)
            dirPath = GameObjectsFullPath + fullId;
        else
            dirPath = ModsObjectsFullPath + fullId;


        try
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            string json = JsonUtility.ToJson(jsonModel, true);
            File.WriteAllText(dirPath + "/" + fullId + ".json", json);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning("I cannot create new dir. Sth is wrong bro!.\n" + e);
        }

    }

    public void LoadObjectsFromPath(string _objectsPath)
    {
        if (Directory.Exists(_objectsPath) == false)
            Directory.CreateDirectory(_objectsPath);

        string[] dirs = Directory.GetDirectories(_objectsPath, "*", SearchOption.TopDirectoryOnly);

        for (int i = 0; i < dirs.Length; i++)
        {
            string tempDir = new DirectoryInfo(dirs[i]).Name;

            string tempJsonPath = _objectsPath + "/" + tempDir + "/" + tempDir + ".json";

            try
            {
                string tempJsonContent = File.ReadAllText(tempJsonPath);
                ObjectData tempData = new ObjectData();
                ObjectJSONModel tempJSON = JsonUtility.FromJson<ObjectJSONModel>(tempJsonContent);
                tempData.json = tempJSON;
                objects.Add(tempData);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning("Error while opening json file in " + tempDir + "\n" + e);
                continue;
            }

            //objects[objects.Count - 1].icon = LoadIconById(objects[objects.Count - 1].json.id);

            SetObjectsStatusFromList();
        }
    }

    public Texture2D LoadIconById(string _id)
    {
        Texture2D tempTexture;
        try
        {
            string tempPath = null;
            if (_id.Contains(gameObjectsPrefix) && Directory.Exists(GameObjectsFullPath + _id))
                tempPath = GameObjectsFullPath + _id + "/" + _id + ".png";
            else if (_id.Contains(modsObjectsPrefix) && Directory.Exists(ModsObjectsFullPath + _id))
                tempPath = ModsObjectsFullPath + _id + "/" + _id + ".png";


            byte[] fileData;
            fileData = File.ReadAllBytes(tempPath);
            tempTexture = new Texture2D(2, 2);
            tempTexture.LoadImage(fileData);
            objects[objects.Count - 1].icon = tempTexture;
            return tempTexture;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning("Error while opening png file in " + _id + "\n" + e);
            return null;
        }
    }

    public static void DeleteObjectById(string _id)
    {
        for (int i = 0; i < ObjectsManager.Instance.objects.Count; i++)
            if (Instance.objects[i].json.id == _id)
            {
                if (_id.Contains(gameObjectsPrefix) && Directory.Exists(GameObjectsFullPath + _id))
                    Directory.Delete(GameObjectsFullPath + _id, true);
                else if (_id.Contains(modsObjectsPrefix) && Directory.Exists(ModsObjectsFullPath + _id))
                    Directory.Delete(ModsObjectsFullPath + _id, true);

                Instance.objects.RemoveAt(i);
                return;
            }
    }

    public void OpenObjectPath(string _id)
    {
        for (int i = 0; i < ObjectsManager.Instance.objects.Count; i++)
            if (Instance.objects[i].json.id == _id)
            {
                string tempPath = null;
                if (_id.Contains(gameObjectsPrefix) && Directory.Exists(GameObjectsFullPath + _id))
                    tempPath = GameObjectsFullPath + _id;
                else if (_id.Contains(modsObjectsPrefix) && Directory.Exists(ModsObjectsFullPath + _id))
                    tempPath = ModsObjectsFullPath + _id;

                if(tempPath != null)
                {
                    Process.Start(tempPath);
                }

                return;
            }
    }

    public static void ClearObjectsList()
    {
        Instance.objects.Clear();
    }

    private void RemoveDisabledObjects()
    {
        foreach (ObjectData tempObject in objects)
        {
            if (tempObject.disabled)
                objects.Remove(tempObject);
        }
    }

    private void SetObjectsStatusFromList()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (disabledObjects.Contains(Instance.objects[i].json.id))
                objects[i].disabled = true;
            else
                objects[i].disabled = false;
        }
    }

    public static ObjectData GetObjectDataById(string _id)
    {
        for (int i = 0; i < ObjectsManager.Instance.objects.Count; i++)
            if (Instance.objects[i].json.id == _id)
                return Instance.objects[i];

        return null;
    }

    public static int GetObjectIndexById(string _id)
    {
        for (int i = 0; i < ObjectsManager.Instance.objects.Count; i++)
            if (Instance.objects[i].json.id == _id)
                return i;

        return -1;
    }

    [System.Serializable]
    public class ObjectData
    {
        public ObjectJSONModel json;
        public Texture2D icon;
        public bool disabled = false;

        public ObjectData()
        {
            json = new ObjectJSONModel();
            json.researchTexts = new string[5];
        }

        public ObjectData(string _id)
        {
            json = new ObjectJSONModel();
            json.id = _id;
            json.researchTexts = new string[5];
        }

        public int ResearchesCount
        {
            get
            {
                for (int i = 0; i < json.researchTexts.Length; i++)
                {
                    if (json.researchTexts[i] == "")
                        return i;
                }
                return json.researchTexts.Length;
            }
        }
    }

    [System.Serializable]
    public class ObjectJSONModel
    {
        public string id;
        public string steamId;
        public string steamName;
        public string steamDescription;
        public MinigameType minigame;
        public ObjectType type;
        public string alert;
        public string[] researchTexts;
    }

    public enum ObjectType
    {
        Safe,
        Euclid,
        Keter,
        Thaumiel,
        Neutralized
    }

    public enum MinigameType
    {
        None,
        Waves,
        Defense
    }
}
