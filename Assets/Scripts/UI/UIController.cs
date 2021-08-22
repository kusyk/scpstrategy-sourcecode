using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WPM;
using UnityEngine.Video;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [Header("Main Panel Stats")]
    public TextMeshProUGUI[] scoreTexts;
    public TextMeshProUGUI[] moneyTexts;
    public TextMeshProUGUI[] devPoinsTexts;
    public TextMeshProUGUI[] actionPoinsTexts;
    public TextMeshProUGUI[] satisfactionTexts;

    [Header("Game Panels")]
    public UIPositionAnimator statsPanel;
    public UIPositionAnimator selectionPanel;
    public UIPositionAnimator inputPanel;
    public UIPositionAnimator decisionPanel;
    public UIPositionAnimator mainPanel;
    public UIPositionAnimator notificationsPanel;

    [Header("Full Screen Panels")]
    public GameObject pauseScreen;
    public GameObject settingsScreen;
    public GameObject managementScreen;
    public GameObject scpsScreen;
    public GameObject skillsScreen;
    public GameObject branchesScreen;
    public GameObject unitsScreen;

    [Header("Other Panels")]
    public GameObject tutorialScreen;
    public VideoPlayer tutorialVideo;
    public Slider tutorialVolumeSlider;

    [Header("Components")]
    public ManagementScreen managementScreenComponent;
    public BranchesScreen branchesScreenComponent;
    public UnitsScreen unitsScreenComponent;
    public ObjectsScreen objectsScreenComponent;

    [Header("Includes all game panels automatically")]
    public GameObject[] uiObjectsToActivate;

    [Header("Other UI Elements")]
    public Button usefulButton;
    public GameObject statsPanelPauseButton;
    public GameObject statsPanelBackButton;

    private WorldMapGlobe map;

    public static bool IsPauseObjectActivated
    {
        get
        {
            if (Instance.pauseScreen.activeSelf)
                return true;

            if (Instance.settingsScreen.activeSelf)
                return true;

            if (Instance.decisionPanel.show)
                return true;

            if (Instance.inputPanel.show)
                return true;

            if (Instance.selectionPanel.show)
                return true;

            return false;
        }
    }


    private void Awake()
    {
        Instance = this;
        ActivateImportantUI();
    }


    void Start()
    {
        map = WorldMapGlobe.instance;
        managementScreenComponent.SetupManagementScreen();

        tutorialVideo.loopPointReached += 
            delegate (VideoPlayer vp) 
            {
                ShowGamePanels();
                SoundSystem.Instance.UnmuteMusic();
            };
    }

    void FixedUpdate()
    {
        for (int i = 0; i < scoreTexts.Length; i++)
        {
            scoreTexts[i].text = PlayerController.PlayerInfo.score.ToString();
        }
        for (int i = 0; i < moneyTexts.Length; i++)
        {
            moneyTexts[i].text = FormatChanger.HugeNumerToString(PlayerController.PlayerInfo.money);
        }
        for (int i = 0; i < devPoinsTexts.Length; i++)
        {
            devPoinsTexts[i].text = PlayerController.PlayerInfo.researchPoints.ToString();
        }
        for (int i = 0; i < actionPoinsTexts.Length; i++)
        {
            actionPoinsTexts[i].text = (PlayerController.GetAvailableActionPoints()) + "/" + PlayerController.GetAllActionPoints();
        }
        for (int i = 0; i < satisfactionTexts.Length; i++)
        {
            satisfactionTexts[i].text = (int)(PlayerController.PlayerInfo.satisfactionLevel * 100) + "%";
        }
    }

    public void ShowSelectionPanel()
    {
        HideAllPanels(0);
        selectionPanel.show = true;
    }

    public void ShowInputPanel()
    {
        HideAllPanels(0);
        inputPanel.show = true;
    }

    public void ShowDecisionPanel()
    {
        HideAllPanels(0);
        decisionPanel.show = true;
    }

    public void ShowGamePanels()
    {
        HideAllPanels();
        mainPanel.show = true;
        notificationsPanel.show = true;
        ToggleStatsPanel(true, StatsPanelButton.Pause);
        ShortcutsManager.activeGeneral = true;
    }

    public void ShowPauseScreen()
    {
        HideAllPanels(0);
        pauseScreen.SetActive(true);
    }

    public void ShowSettingsScreen()
    {
        HideAllPanels(0);
        settingsScreen.SetActive(true);
    }

    public void ShowManagementScreen()
    {
        HideAllPanels(0);
        managementScreenComponent.LoadManagementScreenValues();
        managementScreen.SetActive(true);
        ToggleStatsPanel(true, StatsPanelButton.Back);
    }
    public void ShowScpsScreen()
    {
        ShowScpsScreen(-1);
    }

    public void ShowScpsScreen(int scpIndex = -1)
    {
        HideAllPanels(0);
        scpsScreen.SetActive(true);
        objectsScreenComponent.OpenObjectsScreen(scpIndex);
        ToggleStatsPanel(true, StatsPanelButton.Back);
    }

    public void ShowSkillsScreen()
    {
        ShowSkillsScreen(null);
    }

    public void ShowSkillsScreenById(string skillToShow)
    {
        ShowSkillsScreen(SkillsManager.Instance.GetSkillById(skillToShow));
    }

    public void ShowSkillsScreen(Skill skillToShow)
    {
        HideAllPanels(0);
        skillsScreen.SetActive(true);

        if (skillToShow != null)
            SkillsScreen.Instance.ShowSkill(skillToShow);
        else
            SkillsScreen.Instance.ClearSkillView();

        ToggleStatsPanel(true, StatsPanelButton.Back);
    }

    public void ShowBranchesScreen()
    {
        ShowBranchesScreen(-1);
    }

    public void ShowBranchesScreen(int branchIndex = -1)
    {
        HideAllPanels(0);
        branchesScreen.SetActive(true);
        branchesScreenComponent.OpenBranchesScreen(branchIndex);
        ToggleStatsPanel(true, StatsPanelButton.Back);
    }

    public void ShowUnitsScreen()
    {
        ShowUnitsScreen(-1);
    }

    public void ShowUnitsScreen(int unitIndex = -1)
    {
        HideAllPanels(0);
        unitsScreen.SetActive(true);
        unitsScreenComponent.OpenUnitsScreen(unitIndex);
        ToggleStatsPanel(true, StatsPanelButton.Back);
    }

    public void ShowTutorialScreen()
    {
        HideAllPanels(0);
        tutorialScreen.SetActive(true);
        tutorialVideo.SetDirectAudioVolume(0, SoundSystem.Instance.getMaxMusicVolume);
        tutorialVolumeSlider.SetValueWithoutNotify(SoundSystem.Instance.getMaxMusicVolume);
        SoundSystem.Instance.MuteMusic();
    }

    public void HideAllPanels(float newGameSpeed = -1)
    {
        ToggleStatsPanel(false);
        selectionPanel.show = false;
        inputPanel.show = false;
        decisionPanel.show = false;
        mainPanel.show = false;
        notificationsPanel.show = false;

        pauseScreen.SetActive(false);
        settingsScreen.SetActive(false);
        managementScreen.SetActive(false);
        scpsScreen.SetActive(false);
        skillsScreen.SetActive(false);
        branchesScreen.SetActive(false);
        unitsScreen.SetActive(false);
        tutorialScreen.SetActive(false);

        if (newGameSpeed != -1)
            PlayerController.Instance.SetGameTimeScale(newGameSpeed);
        else
            PlayerController.Instance.SetLastGameTimeScale();

        ShortcutsManager.activeGeneral = false;
    }

    public void UpdateTutorialVolume()
    {
        tutorialVideo.SetDirectAudioVolume(0, tutorialVolumeSlider.value);
    }

    public void SelectUsefulButton()
    {
        usefulButton.Select();
    }

    private void ToggleStatsPanel(bool visibility, StatsPanelButton button = StatsPanelButton.NoChange)
    {
        statsPanel.show = visibility;

        switch (button)
        {
            case StatsPanelButton.Pause:
                statsPanelPauseButton.SetActive(true);
                statsPanelBackButton.SetActive(false);
                break;
            case StatsPanelButton.Back:
                statsPanelPauseButton.SetActive(false);
                statsPanelBackButton.SetActive(true);
                break;
        }
    }

    private void ActivateImportantUI()
    {
        for (int i = 0; i < uiObjectsToActivate.Length; i++)
        {
            if (!uiObjectsToActivate[i].activeSelf)
                uiObjectsToActivate[i].SetActive(true);
        }

        if (!selectionPanel.gameObject.activeSelf)
            selectionPanel.gameObject.SetActive(true);

        if (!inputPanel.gameObject.activeSelf)
            inputPanel.gameObject.SetActive(true);

        if (!decisionPanel.gameObject.activeSelf)
            decisionPanel.gameObject.SetActive(true);

        if (!mainPanel.gameObject.activeSelf)
            mainPanel.gameObject.SetActive(true);

        if (!notificationsPanel.gameObject.activeSelf)
            notificationsPanel.gameObject.SetActive(true);
    }

    private enum StatsPanelButton
    {
        NoChange,
        Pause,
        Back
    }
}
