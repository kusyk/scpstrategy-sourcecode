using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Michsky.LSS;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Game Panels")]
    public GameObject welcomeScreen;
    public GameObject mainScreen;
    public GameObject slotsScreen;
    public GameObject slotEditorScreen;
    public GameObject settingsScreen;
    public GameObject creditsScreen;
    public GameObject luxoGamesScreen;
    public GameObject exitScreen;

    [Space(30)]
    public GameObject surveyButton;
    public TextMeshProUGUI versionTMP;

    [Space(30)]
    public GameEvent AfterGameLoading;

    private static bool firstOpen = true;

    public void Play()
    {
        LoadingScreen.LoadSceneFuturistic("GameGlobe");
    }

    public void OpenModTools()
    {
        LoadingScreen.LoadSceneFuturistic("ModsEditor");
    }

    public void OpenWebsite(string url)
    {
        Application.OpenURL(url);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }

    public void ShowWelcomeScreen()
    {
        HideAllScreens();
        welcomeScreen.SetActive(true);
    }

    public void ShowMainScreen()
    {
        HideAllScreens();
        mainScreen.SetActive(true);
    }

    public void ShowSlotsScreen()
    {
        HideAllScreens();
        slotsScreen.SetActive(true);
    }

    public void ShowSlotEditorScreen()
    {
        HideAllScreens();
        slotEditorScreen.SetActive(true);
    }

    public void ShowSettingsScreen()
    {
        HideAllScreens();
        settingsScreen.SetActive(true);
    }

    public void ShowCreditsScreen()
    {
        HideAllScreens();
        creditsScreen.SetActive(true);
    }

    public void ShowLuxoGamesScreen()
    {
        HideAllScreens();
        luxoGamesScreen.SetActive(true);
    }

    public void ShowExitScreen()
    {
        HideAllScreens();
        exitScreen.SetActive(true);
    }

    public void HideAllScreens()
    {
        welcomeScreen.SetActive(false);
        mainScreen.SetActive(false);
        slotsScreen.SetActive(false);
        slotEditorScreen.SetActive(false);
        settingsScreen.SetActive(false);
        creditsScreen.SetActive(false);
        luxoGamesScreen.SetActive(false);
        exitScreen.SetActive(false);
    }

    private void Start()
    {
        Time.timeScale = 1;

        if (firstOpen)
        {
            surveyButton.SetActive(false);
            ShowWelcomeScreen();
        }
        else
        {
            surveyButton.SetActive(false);//was true
            ShowMainScreen();
        }

        firstOpen = false;
        versionTMP.text = "v" + Application.version;

        AfterGameLoading.Invoke();
    }
}
