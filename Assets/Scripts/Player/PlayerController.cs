using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPM;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public static float gameTimeScale = 1;
    private float lastGameTimeScale = 1;

    [Space]
    [SerializeField]
    private PlayerInfo playerInfo;

    [Space(30)]
    public GameEvent AfterGameLoading;

    public int lastSelectedCell = -1;
    private WorldMapGlobe map;
    private int lastUnit = -1;

    public static PlayerInfo PlayerInfo
    {
        get
        {
            if (Instance == null)
                return null;
            return Instance.playerInfo;
        }
        set
        {
            if (Instance == null)
                return;
            Instance.playerInfo = value;
        }
    }

    public static float gameUITimeScale
    {
        get
        {
            if (gameTimeScale > 1)
                return gameTimeScale;

            if (UIController.IsPauseObjectActivated)
                return 0;

            return 1;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        map = WorldMapGlobe.instance;

        if(SaveLoad.LoadedPlayerInfo == null)
            CreateNewGame();
        else
            LoadGame();


        AfterGameLoading.Invoke();
    }

    private void CreateNewGame()
    {
        playerInfo = new PlayerInfo();
        playerInfo.gamesaveid = SaveLoad.NewPlayerInfoName;

        playerInfo.score = 50;
        playerInfo.money = 1500;
        playerInfo.researchPoints = 3;

        StartNextMonth();

        SetupSettingPlayerName();
    }

    private void LoadGame()
    {
        playerInfo = SaveLoad.LoadedPlayerInfo;

        for (int i = 0; i < PlayerInfo.branches.Count; i++)
        {
            StickersManager.Instance.SpawnSticker(PlayerInfo.branches[i].cellId, StickersManager.StickerType.Branch,
                PlayerInfo.branches[i].name, i);
        }

        for (int i = 0; i < PlayerInfo.units.Count; i++)
        {
            StickersManager.Instance.SpawnSticker(PlayerInfo.units[i].cellId, StickersManager.StickerType.Unit,
                PlayerInfo.units[i].unitName, i);
        }

        ColorsManager.Instance.RecolorWholeGlobe();
        UIController.Instance.ShowGamePanels();
    }

    private void Update()
    {
        playerInfo.playTime += Time.deltaTime / 60;

        UpdateUnits();
        UpdateStats();
    }

    private void UpdateUnits()
    {
        for (int i = 0; i < PlayerInfo.units.Count; i++)
        {
            Unit unit = PlayerInfo.units[i];
            if (unit.status == Unit.Status.Searching)
            {
                unit.progressFloat += Time.deltaTime * PlayerController.gameTimeScale;

                if (unit.progressFloat >= unit.SearchingTime)
                {
                    unit.progressFloat = 0;
                    unit.status = Unit.Status.Waiting;
                    //NotificationsManager.Instance.SpawnNotification(unit.unitName, unit.unitName + " has just finished searching. \n\n<b>click here to show me</b>")
                    //    .OnClickEvent += unit.FlyToMe;
                    if (AnomaliesManager.instance.FinishSearching(unit.cellId))
                    {
                        Notification tempNotification = NotificationsManager.Instance.SpawnNotification(unit.unitName,
                            "Searching finished.\nthe object has been secured\n\n<u>click to open <b>Scientific Department</b></u>",
                            true, -1, PlayerInfo.objects.Count - 1);

                        tempNotification.OnClickEventInt += UIController.Instance.ShowScpsScreen;
                        tempNotification.OnClickEvent += tempNotification.DestroyNotification;

                        PlayerInfo.score += 10;

                        AchievementsManager.Instance.CheckSCPs();

                        if(PlayerInfo.objects.Count == 1)
                        {
                            ShowFirstSkillMessage();
                        }
                    }
                    else
                    {
                        Notification tempNotification = NotificationsManager.Instance.SpawnNotification(unit.unitName, unit.unitName +
                            "Searching finished.\nstatus: nothing has been found\n\n<u>click find this unit on globe</u>");

                        tempNotification.OnClickEvent += unit.FlyToMe;
                        tempNotification.OnClickEvent += tempNotification.DestroyNotification;

                        PlayerInfo.score += 3;
                    }
                }
            }
        }
    }

    private void UpdateStats()
    {
        if(PlayerInfo.score >= 10000 && PlayerInfo.mainGoal == false)
        {
            PlayerInfo.mainGoal = true;
            ShowWinMessage();
        }

        if (PlayerInfo.money <= -5000 && PlayerInfo.mainGoal == false)
        {
            PlayerInfo.mainGoal = true;
            ShowGameOverMessage();
        }

        if (PlayerInfo.satisfactionLevel <= 0 && PlayerInfo.mainGoal == false)
        {
            PlayerInfo.mainGoal = true;
            ShowGameOverMessage();
        }
    }

    private void SpawnFirstAnomaly()
    {
        do
        {
            try
            {
                AnomaliesManager.instance.SpawnNewAnomaly(false);
            }
            catch { }
        }
        while (AnomaliesManager.instance.anomalies.Count == 0);

        AnomaliesManager.instance.FlyToAnomaly(0);
        ShowFirstAnomalyMessage();

        SpawnNextAnomalies();
    }

    #region BRANCHES AND UNITS LOCATION

    public void SetupCreatingFirstBranch()
    {
        SelectionPanelManager.Instance.SetupPanel("Where do you want to create the first branch?");
        SelectionPanelManager.Instance.OnApproveClick.AddListener(CreateBranch);
        SelectionPanelManager.Instance.OnApproveClick.AddListener(SpawnFirstAnomaly);
        SelectionPanelManager.Instance.OnValidate.AddListener(ValidateLocation);
        SelectionPanelManager.Instance.OnCancelClick.AddListener(ShowWelcomeMessage);
    }

    public void SetupCreatingNewBranch()
    {
        SelectionPanelManager.Instance.SetupPanel("Where do you want to create a new branch?");
        SelectionPanelManager.Instance.OnApproveClick.AddListener(CreateBranch);
        SelectionPanelManager.Instance.OnValidate.AddListener(ValidateBrnachLocation);
        SelectionPanelManager.Instance.OnCancelClick.AddListener(UIController.Instance.ShowBranchesScreen);
    }

    public void SetupCreatingFirstUnit()
    {
        SelectionPanelManager.Instance.SetupPanel("Where do you want to create the first unit?");
        SelectionPanelManager.Instance.OnApproveClick.AddListener(CreateUnit);
        SelectionPanelManager.Instance.OnValidate.AddListener(ValidateLocation);
        SelectionPanelManager.Instance.OnCancelClick.AddListener(ShowFirstAnomalyMessage);
    }

    public void SetupCreatingNewUnit()
    {
        SelectionPanelManager.Instance.SetupPanel("Where do you want to create a new unit?");
        SelectionPanelManager.Instance.OnApproveClick.AddListener(CreateUnit);
        SelectionPanelManager.Instance.OnValidate.AddListener(ValidateLocation);
        SelectionPanelManager.Instance.OnCancelClick.AddListener(UIController.Instance.ShowUnitsScreen);
    }

    public void SetupChangingUnitPosition(int unitId)
    {
        lastUnit = unitId;
        SelectionPanelManager.Instance.SetupPanel("Where do you want to fly?");
        SelectionPanelManager.Instance.OnApproveClick.AddListener(MoveUnit);
        SelectionPanelManager.Instance.OnValidate.AddListener(ValidateLocation);
        SelectionPanelManager.Instance.OnValidate.AddListener(DrawUnitPath);
        SelectionPanelManager.Instance.OnCancelClick.AddListener(UIController.Instance.ShowGamePanels);
        SelectionPanelManager.Instance.OnCancelClick.AddListener(CancelUnitFlight);
    }

    public void ValidateLocation(int tempCell)
    {
        for (int i = 0; i < PlayerInfo.branches.Count; i++)
        {
            List<int> tempPath = new List<int>();
            tempPath = map.FindPath(PlayerInfo.branches[i].cellId, tempCell, 0, true, WPM.PathFinding.HiddenCellsFilterMode.UseAllCells);

            bool canContinue = true;

            if(tempCell == PlayerInfo.branches[i].cellId)
            {
                canContinue = false;
            }

            if (tempPath != null)
            {
                if (tempPath.Count < 16)
                {
                    canContinue = false;
                }
            }

            if(canContinue == false)
            {
                SelectionPanelManager.Instance.SetApproveButton(false);
                SelectionPanelManager.Instance.SetInfoText("It is too close to other branch.");
                lastSelectedCell = -1;
                return;
            }
        }

        for (int i = 0; i < PlayerInfo.units.Count; i++)
        {
            bool canContinue = true;

            if (lastUnit != -1)
            {
                if (PlayerInfo.units[i].cellId == PlayerInfo.units[lastUnit].cellId)
                {
                    continue;
                }
            }

            List<int> tempPath = new List<int>();
            tempPath = map.FindPath(PlayerInfo.units[i].cellId, tempCell, 0, true, WPM.PathFinding.HiddenCellsFilterMode.UseAllCells);

            if (tempCell == PlayerInfo.units[i].cellId)
            {
                canContinue = false;
            }

            if (tempPath != null)
            {
                if (tempPath.Count < 16)
                {
                    canContinue = false;
                }
            }

            if (canContinue == false)
            {
                SelectionPanelManager.Instance.SetApproveButton(false);
                SelectionPanelManager.Instance.SetInfoText("It is too close to other MTF.");
                lastSelectedCell = -1;
                return;
            }
        }

        SelectionPanelManager.Instance.SetInfoText(null);
        SelectionPanelManager.Instance.SetApproveButton(true);
        lastSelectedCell = tempCell;
    }

    public void ValidateBrnachLocation(int tempCell)
    {
        ValidateLocation(tempCell);

        //easiest way to check prev last validation status
        if (SelectionPanelManager.Instance.infoPanel.show == true)
            return;

        for (int i = 0; i < AnomaliesManager.instance.anomalies.Count; i++)
        {
            bool canContinue = true;
            
            List<int> tempPath = new List<int>();
            tempPath = map.FindPath(AnomaliesManager.instance.anomalies[i].cells[0], tempCell, 0, true, WPM.PathFinding.HiddenCellsFilterMode.UseAllCells);
            

            if (tempPath != null)
            {
                if (tempPath.Count < 16)
                {
                    canContinue = false;
                }
            }

            if (canContinue == false)
            {
                SelectionPanelManager.Instance.SetApproveButton(false);
                SelectionPanelManager.Instance.SetInfoText("It is too dangerous area. Get rid of local anomalies.");
                lastSelectedCell = -1;
                return;
            }
        }

        SelectionPanelManager.Instance.SetInfoText(null);
        SelectionPanelManager.Instance.SetApproveButton(true);
        lastSelectedCell = tempCell;
    }

    public void CreateBranch()
    {
        if (lastSelectedCell == -1)
            return;

        //wywalone bo demo
        //if (PlayerInfo.branches.Count == 0)
        //{
        //    Notification tempNotification = NotificationsManager.Instance.SpawnNotification("Hello!", "Welcome in SCP Strategy!", false);
        //    tempNotification.OnClickEvent += PrototypeQuickScript.Instance.NoToSiePouczyles;
        //    tempNotification.OnClickEvent += tempNotification.DestroyNotification;
        //}

        PlayerInfo.branches.Add(new Branch(lastSelectedCell, "Site #" + PlayerInfo.branches.Count, GetNearestBranch(lastSelectedCell)));
        PlayerInfo.money -= 600;

        PlayerInfo.branches[PlayerInfo.branches.Count - 1].FlyToMe();

        ColorsManager.Instance.RecolorWholeGlobe();
        StickersManager.Instance.SpawnSticker(PlayerInfo.branches[PlayerInfo.branches.Count - 1].cellId, StickersManager.StickerType.Branch,
            PlayerInfo.branches[PlayerInfo.branches.Count - 1].name, PlayerInfo.branches.Count - 1);

        UIController.Instance.ShowGamePanels();

        AchievementsManager.Instance.CheckBranches();
    }

    public void CreateUnit()
    {
        if (lastSelectedCell == -1)
            return;

        //wywalone bo demo
        //if (PlayerInfo.units.Count == 0)
        //{
        //    Notification tempNotification = NotificationsManager.Instance.SpawnNotification("New unit!", "Click here to learn more about units.", false);
        //    tempNotification.OnClickEvent += PrototypeQuickScript.Instance.NoToSiePouczyles;
        //    tempNotification.OnClickEvent += tempNotification.DestroyNotification;
        //}

        PlayerInfo.units.Add(new Unit(lastSelectedCell, "MTF #" + PlayerInfo.units.Count));
        PlayerInfo.money -= 350;

        PlayerInfo.units[PlayerInfo.units.Count - 1].FlyToMe();

        StickersManager.Instance.SpawnSticker(PlayerInfo.units[PlayerInfo.units.Count - 1].cellId, StickersManager.StickerType.Unit,
            PlayerInfo.units[PlayerInfo.units.Count - 1].unitName, PlayerInfo.units.Count - 1);

        UIController.Instance.ShowGamePanels();

        AchievementsManager.Instance.CheckMTFs();
    }

    public void DrawUnitPath(int tempCell)
    {
        ColorsManager.Instance.RecolorWholeGlobeWithoutRefresh();

        List<int> tempPath = map.FindPath(PlayerInfo.units[lastUnit].cellId, tempCell, 0, true, WPM.PathFinding.HiddenCellsFilterMode.UseAllCells);

        if (lastSelectedCell == -1)
            ColorsManager.Instance.PaintPath(tempPath, false);
        else
            ColorsManager.Instance.PaintPath(tempPath, true);
    }

    public void MoveUnit()
    {
        PlayerInfo.units[lastUnit].cellId = lastSelectedCell;
        StickersManager.Instance.UpdateUnitPositionTest(lastUnit, map.cells[lastSelectedCell].latlonCenter);
        UIController.Instance.ShowGamePanels();
        ColorsManager.Instance.RecolorWholeGlobeWithoutRefresh();
        lastUnit = -1;
    }

    public void CancelUnitFlight()
    {
        lastUnit = -1;
        ColorsManager.Instance.RecolorWholeGlobeWithoutRefresh();
    }

    #endregion

    #region CHANGING NAMES

    public void SetupSettingPlayerName()
    {
        InputPanelManager.Instance.SetupPanel("What is your name?", PlayerInfo.name);
        InputPanelManager.Instance.SetApproveButton(true);
        InputPanelManager.Instance.OnApproveClick.AddListener(SetPlayerName);
        InputPanelManager.Instance.OnValidate.AddListener(ValidateString);
        InputPanelManager.Instance.OnCancelClick.AddListener(Exit);
    }

    public void SetupChangingObjectName(int objectIndex)
    {
        InputPanelManager.Instance.SetupPanel("What is new name of this object?", PlayerInfo.objects[objectIndex].name);
        InputPanelManager.Instance.SetApproveButton(true);
        InputPanelManager.Instance.OnApproveClick.AddListener(SetObjectName);
        InputPanelManager.Instance.OnValidate.AddListener(ValidateString);
        InputPanelManager.Instance.OnCancelClick.AddListener(CancelSettingObjectName);
        lastUnit = objectIndex;
    }

    public void SetupChangingUnitName(int unitIndex)
    {
        InputPanelManager.Instance.SetupPanel("What is new name of this unit?", PlayerInfo.units[unitIndex].unitName);
        InputPanelManager.Instance.SetApproveButton(true);
        InputPanelManager.Instance.OnApproveClick.AddListener(SetUnitName);
        InputPanelManager.Instance.OnValidate.AddListener(ValidateString);
        InputPanelManager.Instance.OnCancelClick.AddListener(CancelSettingUnitName);
        lastUnit = unitIndex;
    }

    public void SetupChangingBranchName(int branchIndex)
    {
        InputPanelManager.Instance.SetupPanel("What is new name of this branch?", PlayerInfo.branches[branchIndex].name);
        InputPanelManager.Instance.SetApproveButton(true);
        InputPanelManager.Instance.OnApproveClick.AddListener(SetBranchName);
        InputPanelManager.Instance.OnValidate.AddListener(ValidateString);
        InputPanelManager.Instance.OnCancelClick.AddListener(CancelSettingBranchName);
        lastUnit = branchIndex;
    }

    public void ValidateString(string input)
    {
        if (input.Length < 4 || input.Length > 25)
        {
            InputPanelManager.Instance.SetApproveButton(false);
            InputPanelManager.Instance.SetInfoText("The name should be 4 to 25 characters long.");
            return;
        }

        if (input[0] == ' ')
        {
            InputPanelManager.Instance.SetApproveButton(false);
            InputPanelManager.Instance.SetInfoText("The first character cannot be a space.");
            return;
        }

        if (input[input.Length - 1] == ' ')
        {
            InputPanelManager.Instance.SetApproveButton(false);
            InputPanelManager.Instance.SetInfoText("The last character cannot be a space.");
            return;
        }

        InputPanelManager.Instance.SetInfoText(null);
        InputPanelManager.Instance.SetApproveButton(true);
    }

    public void SetPlayerName(string input)
    {
        PlayerInfo.name = input;

        ShowWelcomeMessage();
    }

    public void SetObjectName(string input)
    {
        PlayerInfo.objects[lastUnit].name = input;
        UIController.Instance.ShowScpsScreen(lastUnit);
        lastUnit = -1;
    }

    public void CancelSettingObjectName()
    {
        UIController.Instance.ShowScpsScreen(lastUnit);
        lastUnit = -1;
    }

    public void SetUnitName(string input)
    {
        PlayerInfo.units[lastUnit].unitName = input;
        UIController.Instance.ShowUnitsScreen(lastUnit);
        StickersManager.Instance.UpdateUnitName(lastUnit, input);
        lastUnit = -1;
    }

    public void CancelSettingUnitName()
    {
        UIController.Instance.ShowUnitsScreen(lastUnit);
        lastUnit = -1;
    }

    public void SetBranchName(string input)
    {
        PlayerInfo.branches[lastUnit].name = input;
        UIController.Instance.ShowBranchesScreen(lastUnit);
        StickersManager.Instance.UpdateBranchName(lastUnit, input);
        lastUnit = -1;
    }

    public void CancelSettingBranchName()
    {
        UIController.Instance.ShowBranchesScreen(lastUnit);
        lastUnit = -1;
    }

    #endregion

    #region GAME MESSAGES

    public void ShowWelcomeMessage()
    {
        DecisionPanel.Instance.SetupPanel("Welcome to SCP Strategy!", string.Format(GameTexts.WelcomeMessage, PlayerInfo.name), false, false, true);
        DecisionPanel.Instance.OnApproveClick.AddListener(SetupCreatingFirstBranch);
    }

    public void ShowFirstAnomalyMessage()
    {
        DecisionPanel.Instance.SetupPanel("The first anomaly!", GameTexts.FirstAnnomalyMessage, false, false, true);
        DecisionPanel.Instance.OnApproveClick.AddListener(SetupCreatingFirstUnit);
    }

    public void ShowFirstSkillMessage()
    {
        DecisionPanel.Instance.SetupPanel("Good job!", GameTexts.FirstSkillMessage, false, false, true);
        DecisionPanel.Instance.OnApproveClick.AddListener(UIController.Instance.ShowSkillsScreen);
    }

    public void ShowFirstResearchMessage()
    {
        DecisionPanel.Instance.SetupPanel("You are ready!", GameTexts.FirstResearchMessage, false, false, true);
        DecisionPanel.Instance.OnApproveClick.AddListener(UIController.Instance.ShowScpsScreen);
    }

    public void ShowGoodLuckMessage()
    {
        DecisionPanel.Instance.SetupPanel("GOOD JOB!", GameTexts.GoodLuckMessage, false, false, true);
        DecisionPanel.Instance.OnApproveClick.AddListener(UIController.Instance.ShowGamePanels);
    }

    public void ShowWinMessage()
    {
        DecisionPanel.Instance.SetupPanel("Congratulations!", GameTexts.WinMessage, false, false, true);
        DecisionPanel.Instance.OnApproveClick.AddListener(UIController.Instance.ShowGamePanels);
    }

    public void ShowGameOverMessage()
    {
        DecisionPanel.Instance.SetupPanel("GAME OVER!", GameTexts.LoseMessage, false, true, false);
        DecisionPanel.Instance.OnDeclineClick.AddListener(GameOverExit);
    }

    #endregion


    public void StartNextMonth()
    {
        playerInfo.monthsPlayed++;

        //add money
        int moneyToAdd = NextMonthIncome;
        playerInfo.ChangeMoney((long)moneyToAdd);

        //sbstract money
        int moneyToSubstract = (int)(playerInfo.outcomeUnits * GetUnitsEmplCount() + playerInfo.outcomeScientists * GetBrnahcesEmplCount());

        if (playerInfo.money - (long)moneyToSubstract < 0)
        {
            Notification tempNotification = NotificationsManager.Instance.SpawnNotification("Expenses Alert",
                "Expenses were higher than the stock of cash. To avoid bankruptcy change your expenses.\n\n" +
                "<i>Click here</i> to edit payouts.");

            tempNotification.OnClickEvent += UIController.Instance.ShowManagementScreen;
            tempNotification.OnClickEvent += tempNotification.DestroyNotification;

            //playerInfo.outcomeUnits = 40;
            //playerInfo.outcomeScientists = 40;
            moneyToSubstract = moneyToAdd;
        }

        playerInfo.ChangeMoney(-(long)moneyToSubstract);


        //satisfaction
        float whereWeShouldGo = 1f - ((playerInfo.contribution - 1f) / (GetMaxContribution() - 1f));
        float change = Mathf.Abs(playerInfo.satisfactionLevel - whereWeShouldGo) * 0.3f;

        if (change < 0.03f)
            change = 0.03f;

        playerInfo.satisfactionLevel = Mathf.MoveTowards(playerInfo.satisfactionLevel, Mathf.Clamp01(whereWeShouldGo * 2), change);

        //update historical management data
        playerInfo.oldContibution = playerInfo.contribution;
        playerInfo.oldOutcomeUnits = playerInfo.outcomeUnits;
        playerInfo.oldOutcomeScientists = playerInfo.outcomeScientists;

        //if (playerInfo.monthsPlayed == 0)
        //    return;

        //anomalies
        if (PlayerInfo.monthsPlayed > 1)
            SpawnNextAnomalies();


        //satisfaction check
        PlayerInfo.satisfactionLevel = Mathf.Clamp01(PlayerInfo.satisfactionLevel);

        if(PlayerInfo.satisfactionLevel < 0.3f)
        {
            Notification tempNotification = NotificationsManager.Instance.SpawnNotification("Satisfaction Alert",
                "Governments satisfaction is below 30%. Reaching 0% is equal with losing the game.\n\n" +
                "<i>Click here</i> to edit governments' fee.");

            tempNotification.OnClickEvent += UIController.Instance.ShowManagementScreen;
            tempNotification.OnClickEvent += tempNotification.DestroyNotification;
        }

        //Save
        Save();
    }

    private void SpawnNextAnomalies()
    {
        int activeAnomaliesCount = AnomaliesManager.instance.ActivatedAnomaliesCount;
        int targetAnomaliesCount = PlayerInfo.units.Count + 6 + (int)(playerInfo.contribution / GetMaxContribution() * 7);

        if (activeAnomaliesCount <= PlayerInfo.units.Count)
        {
            PlayerInfo.satisfactionLevel += 0.03f;
        }

        if (activeAnomaliesCount < targetAnomaliesCount)
        {
            for (int i = 0; i < targetAnomaliesCount - activeAnomaliesCount; i++)
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
        }
        else
        {
            PlayerInfo.satisfactionLevel -= 0.03f;
        }
    }

    public void SetGameTimeScale(float _scale)
    {
        gameTimeScale = _scale;

        if (_scale != 0)
            lastGameTimeScale = gameTimeScale;

        TimerManager.Instance.UpdateArrowsColors(lastGameTimeScale);
    }

    public void SetLastGameTimeScale()
    {
        gameTimeScale = lastGameTimeScale;
    }

    public static int NextMonthIncome
    {
        get
        {
            return (int)(PlayerController.PlayerInfo.contribution * 204 * (0.5f * PlayerController.PlayerInfo.satisfactionLevel + 0.5f));
        }
    }

    public static int GetNearestBranch (int cellId)
    {
        if(PlayerInfo.branches.Count == 0)
        {
            return -1;
        }

        int nearestBranch = int.MaxValue;
        int nearestBranchDistance = int.MaxValue;

        for (int i = 0; i < PlayerInfo.branches.Count; i++)
        {
            int distance = Instance.map.FindPath(PlayerInfo.branches[i].cellId, cellId, 0, true, WPM.PathFinding.HiddenCellsFilterMode.UseAllCells).Count;

            if(distance < nearestBranchDistance)
            {
                nearestBranch = i;
                nearestBranchDistance = distance;
            }
        }

        return nearestBranch;
    }
    public static int GetManualResearchReward()
    {
        if (SkillsManager.Instance.GetSkillProgress("mr2") == 1f)
            return 12;
        else if (SkillsManager.Instance.GetSkillProgress("mr1") == 1f)
            return 8;
        else
            return 5;
    }

    public static string CanOpenNewBranch(int requiredMoney)
    {
        if (PlayerInfo.money < requiredMoney)
            return "not enough money".ToUpper();
        else if (PlayerInfo.branches.Count >= 20)
            return "you reached the limit";
        else if (SkillsManager.Instance.GetSkillProgress("man3") != 1f && PlayerInfo.branches.Count >= 10)
            return SkillsManager.Instance.GetSkillFullName("man3").ToUpper() + "<br>skill required";
        else if (SkillsManager.Instance.GetSkillProgress("man2") != 1f && PlayerInfo.branches.Count >= 5)
            return SkillsManager.Instance.GetSkillFullName("man2").ToUpper() + "<br>skill required";
        else if (SkillsManager.Instance.GetSkillProgress("man1") != 1f && PlayerInfo.branches.Count >= 1)
            return SkillsManager.Instance.GetSkillFullName("man1").ToUpper() + "<br>skill required";

        return null;
    }

    public static string CanCreateNewUnit(int requiredMoney)
    {
        if (PlayerInfo.money < requiredMoney)
            return "not enough money".ToUpper();
        else if (PlayerInfo.units.Count >= 20)
            return "you reached the limit";
        else if (SkillsManager.Instance.GetSkillProgress("ia3") != 1f && PlayerInfo.units.Count >= 10)
            return SkillsManager.Instance.GetSkillFullName("ia3").ToUpper() + "<br>skill required";
        else if (SkillsManager.Instance.GetSkillProgress("ia2") != 1f && PlayerInfo.units.Count >= 5)
            return SkillsManager.Instance.GetSkillFullName("ia2").ToUpper() + "<br>skill required";
        else if (SkillsManager.Instance.GetSkillProgress("ia1") != 1f && PlayerInfo.units.Count >= 1)
            return SkillsManager.Instance.GetSkillFullName("ia1").ToUpper() + "<br>skill required";

        return null;
    }

    public static int GetAvailableActionPoints()
    {
        int ap = GetAllActionPoints();
        for (int i = 0; i < PlayerInfo.skills.Count; i++)
        {
            ap -= PlayerInfo.skills[i].actionPoints;
        }

        ap -= AutoResearchManager.GetResearchCount * 2;

        if (ap < 0)
            ap = 0;

        return ap;
    }

    public static int GetAllActionPoints()
    {
        int ap = 0;
        for (int i = 0; i < PlayerInfo.branches.Count; i++)
        {
            ap += 1 + PlayerInfo.branches[i].upgradeLevel;
        }
        return ap;
    }

    public static int GetMaxContribution()
    {
        if (SkillsManager.Instance.GetSkillProgress("dip2") == 1f)
            return 20;
        else if (SkillsManager.Instance.GetSkillProgress("dip1") == 1f)
            return 10;

        return 5;
    }

    public static int GetUnitsEmplCount()
    {
        int employees = 0;

        for (int i = 0; i < PlayerInfo.units.Count; i++)
        {
            employees += 10 * PlayerInfo.units[i].upgradeLevel + 10;
        }

        return employees;
    }

    public static int GetBrnahcesEmplCount()
    {
        int employees = 0;

        for (int i = 0; i < PlayerInfo.branches.Count; i++)
        {
            employees += 20 * PlayerInfo.branches[i].upgradeLevel + 20;
        }

        return employees;
    }

    public void SaveAndExit()
    {
        Save();
        Exit();
    }
    public void GameOverExit()
    {
        SaveLoad.Delete(PlayerInfo.gamesaveid);
        Exit();
    }

    public void Save()
    {
        SaveLoad.Save(playerInfo);
    }

    public void Exit()
    {
        Michsky.LSS.LoadingScreen.LoadSceneFuturistic("MainMenu");
    }
}
