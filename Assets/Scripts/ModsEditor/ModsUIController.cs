using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ModsUIController : MonoBehaviour
{
    public static ModsUIController Instance;
    public static bool devAccess = false;

    public string devAccessFilePath = "luxo.dev";
    public string guideLink = "https://steamcommunity.com/sharedfiles/filedetails/?id=2566680227";

    [Space]
    public GameObject mainScreen;
    public GameObject objectsListScreen;
    public GameObject objectEditorScreen;

    [Space]
    public GameObject devAccessInfoObject;

    [Header("Objects List")]
    public GameObject objectButtonPrefab;
    public RectTransform objectsListContent;
    public TMP_InputField newObjectInputField;

    [Header("Objects Editor")]
    public Texture2D standardIcon;
    public GameObject steamIdField;
    public TextMeshProUGUI editorId;
    public ScrollRect editorScrollRect;
    public RawImage editorIcon;
    public TMP_InputField editorName;
    public TMP_InputField editorDescription;
    public TMP_InputField editorSteamId;
    public TMP_InputField editorType;
    public TMP_InputField editorMinigame;
    public TMP_InputField editorAlert;
    public TMP_InputField editorResearch0;
    public TMP_InputField editorResearch1;
    public TMP_InputField editorResearch2;
    public TMP_InputField editorResearch3;
    public TMP_InputField editorResearch4;

    public void CreateNewObject()
    {
        if (newObjectInputField.text.Length < 4)
            return;

        string newObjectId = ObjectsManager.Instance.CreateObject(newObjectInputField.text, devAccess);

        if (newObjectId == null)
            return;

        ShowObjectEditorScreen(newObjectId);
    }

    public void OpenPatch()
    {
        ObjectsManager.Instance.OpenObjectPath(editorId.text);
    }

    public void BackToMainMenu()
    {
        Michsky.LSS.LoadingScreen.LoadSceneFuturistic("MainMenu");
    }

    private void Awake()
    {
        Instance = this;

        string devFilePath = Application.streamingAssetsPath + "/" + devAccessFilePath;

        if (File.Exists(devFilePath) || Application.isEditor)
            devAccess = true;
        else
            devAccess = false;

    }

    private void Start()
    {
        devAccessInfoObject.SetActive(devAccess);

        ShowMainScreen();
    }

    public void ShowMainScreen()
    {
        HideAllScreens();
        mainScreen.SetActive(true);
    }

    public void ShowObjectsListScreen()
    {
        HideAllScreens();
        objectsListScreen.SetActive(true);

        RefreshObjectsList();
    }

    public void ShowObjectEditorScreen(string _id)
    {
        HideAllScreens();
        objectEditorScreen.SetActive(true);
        steamIdField.SetActive(devAccess);
        editorScrollRect.verticalNormalizedPosition = 1;

        ObjectsManager.ObjectData tempObject = ObjectsManager.GetObjectDataById(_id);
        RefreshIcon(_id);
        editorId.text = _id;
        editorName.text = tempObject.json.steamName;
        editorDescription.text = tempObject.json.steamDescription;
        editorSteamId.text = tempObject.json.steamId;
        editorType.text = tempObject.json.type.ToString();
        editorMinigame.text = tempObject.json.minigame.ToString();
        editorAlert.text = tempObject.json.alert;
        editorResearch0.text = tempObject.json.researchTexts.Length >= 1 ? tempObject.json.researchTexts[0] : "";
        editorResearch1.text = tempObject.json.researchTexts.Length >= 2 ? tempObject.json.researchTexts[1] : "";
        editorResearch2.text = tempObject.json.researchTexts.Length >= 3 ? tempObject.json.researchTexts[2] : "";
        editorResearch3.text = tempObject.json.researchTexts.Length >= 4 ? tempObject.json.researchTexts[3] : "";
        editorResearch4.text = tempObject.json.researchTexts.Length >= 5 ? tempObject.json.researchTexts[4] : "";
    }

    public void RefreshIcon()
    {
        RefreshIcon(null);
    }

    public void RefreshIcon(string _id)
    {
        if (_id == null)
            _id = editorId.text;

        return;

        Texture2D tempIcon = ObjectsManager.Instance.LoadIconById(_id);
        editorIcon.texture = tempIcon != null ? tempIcon : standardIcon;
    }

    public void SaveObjectToFile()
    {
        ObjectsManager.ObjectJSONModel tempJson = GenerateObjectJSON();

        ObjectsManager.Instance.SaveObjectToFile(tempJson, devAccess);
        ShowObjectEditorScreen(tempJson.id);
    }

    public void SaveAndShare()
    {
        SaveObjectToFile();
        UploadManager.Instance.UploadObject(GenerateObjectJSON(), devAccess);
    }

    public void OpenGuide()
    {
        if (SteamManager.Initialized)
            Steamworks.SteamFriends.ActivateGameOverlayToWebPage(guideLink);
        else
            Application.OpenURL(guideLink);
    }

    private void HideAllScreens()
    {
        mainScreen.SetActive(false);
        objectsListScreen.SetActive(false);
        objectEditorScreen.SetActive(false);
    }

    private void RefreshObjectsList()
    {
        ObjectsManager.ClearObjectsList();

        if (devAccess)
            ObjectsManager.Instance.LoadObjectsFromPath(ObjectsManager.GameObjectsFullPath);
        else
            ObjectsManager.Instance.LoadObjectsFromPath(ObjectsManager.ModsObjectsFullPath);

        for (int i = objectsListContent.childCount - 1; i >= 0; i--)
        {
            Destroy(objectsListContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < ObjectsManager.Instance.objects.Count; i++)
        {
            Instantiate(objectButtonPrefab, objectsListContent).GetComponent<ModButton>().SetupModButton(ObjectsManager.Instance.objects[i]);
        }
    }

    private ObjectsManager.ObjectJSONModel GenerateObjectJSON()
    {
        ObjectsManager.ObjectType tempType;
        ObjectsManager.MinigameType tempMinigame;

        if (editorType.text.Length > 1)
            editorType.text = editorType.text.Substring(0, 1).ToUpper() + editorType.text.Substring(1).ToLower();

        try { tempType = (ObjectsManager.ObjectType)System.Enum.Parse(typeof(ObjectsManager.ObjectType), editorType.text); }
        catch (System.Exception) { tempType = ObjectsManager.ObjectType.Safe; }

        if (editorMinigame.text.Length > 1)
            editorMinigame.text = editorMinigame.text.Substring(0, 1).ToUpper() + editorMinigame.text.Substring(1).ToLower();

        try { tempMinigame = (ObjectsManager.MinigameType)System.Enum.Parse(typeof(ObjectsManager.MinigameType), editorMinigame.text); }
        catch (System.Exception) { tempMinigame = ObjectsManager.MinigameType.None; }

        return new ObjectsManager.ObjectJSONModel()
        {
            id = editorId.text,
            steamName = editorName.text.Replace("\"", "'"),
            steamDescription = editorDescription.text.Replace("\"", "'"),
            steamId = editorSteamId.text.Replace("\"", "'"),
            type = tempType,
            minigame = tempMinigame,
            alert = editorAlert.text.Replace("\"", "'"),
            researchTexts = new string[] {
            editorResearch0.text.Replace("\"", "'"),
            editorResearch1.text.Replace("\"", "'"),
            editorResearch2.text.Replace("\"", "'"),
            editorResearch3.text.Replace("\"", "'"),
            editorResearch4.text.Replace("\"", "'") }
        };
    }
}
