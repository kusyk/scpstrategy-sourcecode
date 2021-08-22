using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPM;

public class DemoControllerRemoveThis : MonoBehaviour
{
    public static DemoControllerRemoveThis Instance;

    public float demoTime = 10;

    [Space(20)]
    [TextArea(4, 20)]
    public string welcomeMessage;
    [Space(20)]
    [TextArea(4, 20)]
    public string finalMessage;
    public string surveyLink = "https://forms.gle/5e9HdscRxYtMy2ut8";

    [Space(20)]
    public PlayerInfo demoPlayerInfo = new PlayerInfo();

    [Space(20)]
    public List<int> demoAnomaliesStartPositions = new List<int>();

    [Space(20)]
    public List<string> demoSkills = new List<string>();

    [Space(20)]
    public float demoTimer = int.MaxValue;

    [Space(20)]
    public int initialCountOfRandomAnomalies = 8;
    public float defaultAnomalyDelay = 35;
    public int maxVisibleAnomalies = 15;
    [Space(5)]
    public float anomalyTimer = 30;

    private WorldMapGlobe map;

    private int GetCountOfActiveAnomalies
    {
        get
        {
            int counter = 0;

            for (int i = 0; i < AnomaliesManager.instance.anomalies.Count; i++)
            {
                if (AnomaliesManager.instance.anomalies[i].active)
                    counter++;
            }

            return counter;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        map = WorldMapGlobe.instance;

        //randomize objects' cells
        for (int i = 0; i < demoPlayerInfo.objects.Count; i++)
        {
            demoPlayerInfo.objects[i].coords = map.cells[Random.Range(0, map.cells.Length - 1)].latlonCenter;
        }


        PlayerController.PlayerInfo = demoPlayerInfo;

        //spawn stickers
        for (int i = 0; i < demoPlayerInfo.branches.Count; i++)
        {
            StickersManager.Instance.SpawnSticker(demoPlayerInfo.branches[i].cellId, StickersManager.StickerType.Branch,
                demoPlayerInfo.branches[i].name, i);
            StickersManager.Instance.RefreshStickerHolo(i);
        }

        for (int i = 0; i < demoPlayerInfo.units.Count; i++)
        {
            StickersManager.Instance.SpawnSticker(demoPlayerInfo.units[i].cellId, StickersManager.StickerType.Unit,
                demoPlayerInfo.units[i].unitName, i);
        }

        ////load active skills
        //for (int i = 0; i < demoPlayerInfo.skills.Count; i++)
        //{
        //    if(demoPlayerInfo.skills[i].status == 2)
        //    {
        //        demoSkills.Add(demoPlayerInfo.skills[i].skillId);
        //    }
        //}

        //spawn prepared anomalies
        for (int i = 0; i < demoAnomaliesStartPositions.Count; i++)
        {
            try
            {
                AnomaliesManager.instance.SpawnNewAnomaly(demoAnomaliesStartPositions[i], false);
            }
            catch(System.Exception e)
            {
                Debug.LogError("small issue when i=" + i + " (prepared)\n" + e.Message);
            }
        }

        //spawn random anomalies
        for (int i = 0; i < initialCountOfRandomAnomalies; i++)
        {
            try
            {
                AnomaliesManager.instance.SpawnNewAnomaly(false);
            }
            catch (System.Exception e)
            {
                Debug.LogError("small issue when i=" + i + " (random)\n" + e.Message);
            }
        }


        ColorsManager.Instance.RecolorWholeGlobe();

        SetupSettingPlayerName();

        PlayerController.Instance.AfterGameLoading.Invoke();
        map.FlyToCell(demoPlayerInfo.branches[0].cellId, 2f, 0.3f);
    }

    private void Update()
    {
        //demo timer
        if (!UIController.Instance.tutorialScreen.activeSelf && !UIController.Instance.pauseScreen.activeSelf 
            && !UIController.Instance.settingsScreen.activeSelf && !MinigamesController.isAnyMinigameActive)
            demoTimer -= Time.deltaTime;

        if(demoTimer < 0 && !UIController.Instance.decisionPanel.show)
        {
            SetupFinalMessage();
        }

        //anomaly timer
        if (UIController.Instance.mainPanel.show)
        {
            anomalyTimer -= Time.deltaTime;

            if(anomalyTimer < 0)
            {
                anomalyTimer = defaultAnomalyDelay;

                if (GetCountOfActiveAnomalies < maxVisibleAnomalies)
                {
                    try
                    {
                        AnomaliesManager.instance.SpawnNewAnomaly();
                    }
                    catch (System.Exception e)
                    {
                        anomalyTimer = 1;
                        Debug.LogError("small issue after anomaly delay\n" + e.Message);
                    }
                }
            }
        }
    }

    public void SetupSettingPlayerName()
    {
        InputPanelManager.Instance.SetupPanel("What is your name?", PlayerController.PlayerInfo.name);
        InputPanelManager.Instance.SetApproveButton(true);
        InputPanelManager.Instance.OnApproveClick.AddListener(SetPlayerName);
        InputPanelManager.Instance.OnValidate.AddListener(PlayerController.Instance.ValidateString);
        InputPanelManager.Instance.OnCancelClick.AddListener(PlayerController.Instance.Exit);
    }

    public void SetPlayerName(string input)
    {
        PlayerController.PlayerInfo.name = input;
        demoTimer = 60 * demoTime;
        SetupFirstMessage();
    }

    public void SetupFirstMessage()
    {
        DecisionPanel.Instance.SetupPanel("Welcome to SCP Strategy", string.Format(welcomeMessage, PlayerController.PlayerInfo.name, demoTime), false);
        DecisionPanel.Instance.OnDeclineClick.AddListener(UIController.Instance.ShowGamePanels);
        DecisionPanel.Instance.OnApproveClick.AddListener(UIController.Instance.ShowTutorialScreen);
    }

    public void SetupFinalMessage()
    {
        SoundSystem.Instance.PlayNotificationSound();
        DecisionPanel.Instance.SetupPanel("Thank you for playing!", string.Format(finalMessage, PlayerController.PlayerInfo.name), false);
        DecisionPanel.Instance.OnDeclineClick.AddListener(PlayerController.Instance.Exit);
        DecisionPanel.Instance.OnApproveClick.AddListener(OpenSurveyWebsite);
        DecisionPanel.Instance.OnApproveClick.AddListener(PlayerController.Instance.Exit);
    }

    public void OpenSurveyWebsite()
    {
        Application.OpenURL(surveyLink);
    }

    public static string GetSkillFullName(string skillId)
    {
        //for (int i = 0; i < Instance.demoPlayerInfo.skills.Count; i++)
        //{
        //    if (Instance.demoPlayerInfo.skills[i].skillId == skillId)
        //        return Instance.demoPlayerInfo.skills[i].name.ToUpper();
        //}

        return "###";
    }

    public static int GetManualResearchReward()
    {
        if (Instance.demoSkills.Contains("mr2"))
            return 5;
        else if (Instance.demoSkills.Contains("mr1"))
            return 4;
        else
            return 3;
    }

    public static string CanOpenNewBranch(int requiredMoney)
    {
        if (Instance.demoPlayerInfo.money < requiredMoney)
            return "lack of money".ToUpper();
        else if (Instance.demoPlayerInfo.branches.Count >= 20)
            return "you reached the demo limit";
        else if (!Instance.demoSkills.Contains("man3") && Instance.demoPlayerInfo.branches.Count >= 10)
            return GetSkillFullName("man3").ToUpper() + " required";
        else if (!Instance.demoSkills.Contains("man2") && Instance.demoPlayerInfo.branches.Count >= 5)
            return GetSkillFullName("man2").ToUpper() + " required";

        return null;
    }

    public static string CanCreateNewUnit(int requiredMoney)
    {
        if (Instance.demoPlayerInfo.money < requiredMoney)
            return "lack of money".ToUpper();
        else if (Instance.demoPlayerInfo.units.Count >= 20)
            return "you reached the demo limit";
        else if (!Instance.demoSkills.Contains("ia3") && Instance.demoPlayerInfo.units.Count >= 10)
            return GetSkillFullName("ia3").ToUpper() + " required";
        else if (!Instance.demoSkills.Contains("ia2") && Instance.demoPlayerInfo.units.Count >= 5)
            return GetSkillFullName("ia2").ToUpper() + " required";

        return null;
    }
}
