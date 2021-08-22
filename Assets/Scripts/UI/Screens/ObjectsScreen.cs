using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectsScreen : MonoBehaviour
{
    public static ObjectsScreen Instance;

    [Header("Objects List")]
    public RectTransform listContent;
    public GameObject objectPrefab;
    public ScrollRect scrollList;

    [Header("Object Details")]
    public TextMeshProUGUI scpName;
    public TextMeshProUGUI scpDetails;
    public TextMeshProUGUI scpReport;
    public RawImage scpImage;
    public ScrollRect reportScrollRect;
    public TextMeshProUGUI reportsPage;
    public TextMeshProUGUI nextResearchPrice;
    public GameObject editButton;

    [Header("Auto-Research Objects")]
    public GameObject automaticResearchButton;
    public TextMeshProUGUI automaticRequirements;
    public GameObject automaticRequirementsNotMatch;
    public TextMeshProUGUI automaticResearchTimeLabel;
    public GameObject autoResearchBarHolder;
    public Image autoResearchBar;

    [Header("Manual-Research Objects")]
    public GameObject manualResearchButton;
    public TextMeshProUGUI manualRequirements;
    public TextMeshProUGUI manualResearchReward;

    [Header("Standard Data")]
    public string standardDetails = "Select an object from the list on the left.";
    public string emptyListDetails = "This panel will be available when you find new objects.";
    public Texture2D standardTexture;

    public int visibleObject = -1;

    private bool openFirstTime = true;

    public void PrepareList()
    {
        if (Instance != this)
            Instance = this;

        for (int i = listContent.childCount - 1; i >= 0; i--)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }

        RefreshList();
    }

    public void RefreshList()
    {
        int childCount = listContent.transform.childCount;
        int objectsCount = PlayerController.PlayerInfo.objects.Count;

        //destroy if more
        for (int i = childCount - 1; i >= objectsCount; i--)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }

        //add new (if need) and refresh all
        for (int i = 0; i < objectsCount; i++)
        {
            if (childCount > i)
                listContent.GetChild(i).GetComponent<ObjectButton>().RefreshButton();
            else
                Instantiate(objectPrefab, listContent.transform).GetComponent<ObjectButton>().RefreshButton();
        }
    }

    public void OpenObjectsScreen(int selectedObject = -1)
    {
        if (Instance != this)
            Instance = this;

        visibleObject = selectedObject;

        if (visibleObject == -1)
        {
            RefreshList();
            scrollList.verticalNormalizedPosition = 1f;
            ClearDetails();
            return;
        }
        ShowObject(selectedObject);
        ScrollToSelected();
    }

    public void ShowObject(int _objIndex)
    {
        visibleObject = _objIndex;
        RefreshList();

        ObjectInfo tempObjectInfo = PlayerController.PlayerInfo.objects[visibleObject];
        ObjectsManager.ObjectData tempObjectData = ObjectsManager.GetObjectDataById(tempObjectInfo.id);

        string _name = tempObjectInfo.name;
        string _coordinates = "<size=24>" + FormatChanger.VectorToCoordinates(tempObjectInfo.coords) + "</size>";
        string _type = "Type: ";
        string _researchesAvailable = "Researches: ";
        string _recievedPoints = "Recieved Research Poins: " + tempObjectInfo.recievedResearchPoins;
        string _researchText = "";
        Texture2D _texture = null;

        automaticRequirements.gameObject.SetActive(false);

        if (tempObjectData == null)
        {
            _type += "UNKNOWN";
            _researchesAvailable += tempObjectInfo.doneResearches + "/#";
            _researchText = "This object's data is not available. \n\n" +
                "It is probably caused by not-loaded mod. Check if you have this mod installed properly. \n\n" +
                "Don't worry! \n" +
                "Your research progress is saved and you will be able to continue doing research when you fix this issue. \n\n" +
                tempObjectInfo.id;

            automaticResearchButton.SetActive(false);
            manualResearchButton.SetActive(false);
            manualRequirements.gameObject.SetActive(false);
            autoResearchBarHolder.SetActive(false);
        }
        else// (tempObjectData != null)
        {
            int maxResearches = tempObjectData.ResearchesCount;

            if(tempObjectInfo.autoResearch > 0)
            {
                automaticResearchButton.SetActive(false);
                manualResearchButton.SetActive(false);
                manualRequirements.gameObject.SetActive(false);
                autoResearchBarHolder.SetActive(true);
            }
            else if (tempObjectInfo.doneResearches >= maxResearches)
            {
                tempObjectInfo.doneResearches = maxResearches;
                automaticResearchButton.SetActive(false);
                manualResearchButton.SetActive(false);
                manualRequirements.gameObject.SetActive(false);
                autoResearchBarHolder.SetActive(false);
            }
            else
            {
                if (SkillsManager.Instance.GetSkillProgress("ar1") == 1f)
                {
                    automaticResearchButton.SetActive(true);
                    automaticRequirements.gameObject.SetActive(false);
                }
                else
                {
                    automaticResearchButton.SetActive(false);
                    automaticRequirements.gameObject.SetActive(true);
                    automaticRequirements.text = string.Format("<b>{0}</b><br>skill is required to begin automatic research", SkillsManager.Instance.GetSkillFullName("ar1"));
                }

                automaticResearchTimeLabel.text = (int)AutoResearchManager.instance.GetResearchDuration + "s";
                autoResearchBarHolder.SetActive(false);

                if (ObjectsManager.GetObjectDataById(tempObjectInfo.id).json.minigame == ObjectsManager.MinigameType.None)
                {
                    manualResearchButton.SetActive(false);
                    manualRequirements.gameObject.SetActive(false);
                }
                else
                {
                    string requiredSkill = "man1";

                    switch (ObjectsManager.GetObjectDataById(tempObjectInfo.id).json.minigame)
                    {
                        case ObjectsManager.MinigameType.Waves:
                            requiredSkill = "waves1";
                            break;
                        case ObjectsManager.MinigameType.Defense:
                            requiredSkill = "def1";
                            break;
                    }

                    if (SkillsManager.Instance.GetSkillProgress(requiredSkill) == 1f)
                    {
                        manualResearchButton.SetActive(true);
                        manualRequirements.gameObject.SetActive(false);
                        manualResearchReward.text = "+" + PlayerController.GetManualResearchReward();
                    }
                    else
                    {
                        manualResearchButton.SetActive(false);
                        manualRequirements.gameObject.SetActive(true);
                        manualRequirements.text = string.Format("<b>{0}</b><br>skill is required to begin manual research", SkillsManager.Instance.GetSkillFullName(requiredSkill));
                    }

                }
            }

            _type += tempObjectData.json.type;
            _researchesAvailable += tempObjectInfo.doneResearches + "/" + maxResearches;

            _researchText += "<b>Short description #0</b>\n";
            _researchText += tempObjectData.json.alert + "\n\n";

            for (int i = 0; i < tempObjectInfo.doneResearches; i++)
            {
                _researchText += string.Format("<b>Research #{0}</b>\n", (i + 1));
                _researchText += tempObjectData.json.researchTexts[i] + "\n\n";
            }

            _texture = tempObjectData.icon;
        }

        scpName.text = _name;
        scpDetails.text = _coordinates + "\n" + _type + "\n" + _researchesAvailable + "\n" + _recievedPoints;
        scpReport.text = _researchText;
        scpImage.texture = _texture != null ? _texture : standardTexture;

        editButton.SetActive(true);
        Canvas.ForceUpdateCanvases();
        reportScrollRect.verticalNormalizedPosition = 0;
    }

    public void ConductAutomaticResearch()
    {
        UIController.Instance.SelectUsefulButton();
        if (PlayerController.GetAvailableActionPoints() < 2)
        {
            return;
        }

        AutoResearchManager.instance.StartResarchingObject(visibleObject);
        ShowObject(visibleObject);
    }

    public void ConductManualResearch()
    {
        MinigamesController.Instance.SetupMinigame(visibleObject);
    }

    public void ChangeName()
    {
        if (visibleObject >= 0)
            PlayerController.Instance.SetupChangingObjectName(visibleObject);
    }

    private void ScrollToSelected()
    {
        Canvas.ForceUpdateCanvases();

        Vector2 newPosition =
            (Vector2)scrollList.transform.InverseTransformPoint(listContent.position)
            - (Vector2)scrollList.transform.InverseTransformPoint(listContent.GetChild(visibleObject).position + new Vector3(0, 5, 0));

        newPosition.x = 0;
        listContent.anchoredPosition = newPosition;
    }

    private void ClearDetails()
    {
        scpName.text = "";

        if (PlayerController.PlayerInfo.objects.Count > 0)
            scpDetails.text = standardDetails;
        else
            scpDetails.text = emptyListDetails;

        scpReport.text = "";
        scpImage.texture = standardTexture;
        editButton.SetActive(false);
        automaticResearchButton.SetActive(false);
        manualResearchButton.SetActive(false);
        manualRequirements.gameObject.SetActive(false);
        autoResearchBarHolder.SetActive(false);
        automaticRequirements.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (openFirstTime && visibleObject > 0)
        {
            openFirstTime = false;
            ScrollToSelected();
            return;
        }

        if (visibleObject >= 0)
        {
            try
            {
                float fill = PlayerController.PlayerInfo.objects[visibleObject].autoResearch;
                if (fill > 0)
                {
                    autoResearchBar.fillAmount = fill;
                }
            }
            catch { };
        }

        if (automaticResearchButton.activeSelf)
        {
            if(automaticRequirementsNotMatch.activeSelf && PlayerController.GetAvailableActionPoints() >= 2)
            {
                automaticRequirementsNotMatch.SetActive(false);
            }
            else if (automaticRequirementsNotMatch.activeSelf == false && PlayerController.GetAvailableActionPoints() < 2)
            {
                automaticRequirementsNotMatch.SetActive(true);
            }
        }
        else if (automaticRequirementsNotMatch.activeSelf)
        {
            automaticRequirementsNotMatch.SetActive(false);
        }
    }

    private void OnDisable()
    {
        visibleObject = -1;
    }
}
