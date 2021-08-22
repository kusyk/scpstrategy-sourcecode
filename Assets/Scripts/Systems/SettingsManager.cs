using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WPM;
using UnityEngine.Rendering.PostProcessing;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public static SettingsData settings;

    [Header("Asset Settings")]
    public int maxLanguage = 0;
    public int maxQuality = 1;
    public GameObject languageObject;

    [Space]
    public TextMeshProUGUI languageTMPro;
    public TextMeshProUGUI sensitivityTMPro;
    public Toggle scrollingToggle;

    [Space]
    public TextMeshProUGUI graphicsTMPro;
    public TextMeshProUGUI resolutionTMPro;
    public TextMeshProUGUI brightnessTMPro;
    public Toggle fullScreenToggle;
    public Toggle vSyncToggle;

    [Space]
    public TextMeshProUGUI musicTMPro;
    public TextMeshProUGUI uiTMPro;


    private SettingsData tempSettings;
    private int tempResolution;

    [Header("Scene Settings")]
    public Color globeColorWithPP;
    public Color globeColorWithoutPP;
    public WorldMapGlobe globe;
    public PostProcessVolume postProcess;

    void Start()
    {
        LoadSettings();
    }

    void OnEnable()
    {
        if (WorldMapGlobe.instance == null)
            return;

        //permament hide
        //languageObject.SetActive(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0);
        languageObject.SetActive(false);

        LoadSettings();
    }

    public void PlayClickSound()
    {
        SoundSystem.Instance.PlayClickSound();
    }

    public void ChangeLanguage(int change)
    {
        tempSettings.language += change;
        if (tempSettings.language < 0) tempSettings.language = maxLanguage;
        if (tempSettings.language > maxLanguage) tempSettings.language = 0;

        switch (tempSettings.language)
        {
            case 0:
                languageTMPro.text = "English";
                break;
            default:
                languageTMPro.text = "English";
                break;
        }
    }

    public void ChangeSensitivity(float change)
    {
        if (scrollingToggle.isOn == false)
        {
            sensitivityTMPro.text = "OFF";
            return;
        }

        tempSettings.sensitivity += change;
        tempSettings.sensitivity = Mathf.Clamp(tempSettings.sensitivity, 0.1f, 2.0f);
        tempSettings.sensitivity = Mathf.Round(tempSettings.sensitivity * 10f) / 10;
        sensitivityTMPro.text = tempSettings.sensitivity.ToString();

        if (tempSettings.sensitivity == 1f || tempSettings.sensitivity == 2f)
            sensitivityTMPro.text += ".0";
        else
            sensitivityTMPro.text = sensitivityTMPro.text.Replace(",", ".");
    }

    public void ChangeResolution(int change)
    {
        tempResolution += change;
        if (tempResolution < 0) tempResolution = 0;
        if (tempResolution > Screen.resolutions.Length - 1) tempResolution = Screen.resolutions.Length - 1;

        resolutionTMPro.text = Screen.resolutions[tempResolution].ToString();
    }

    public void ChangeQuality(int change)
    {
        tempSettings.graphics += change;
        if (tempSettings.graphics < 0) tempSettings.graphics = 0;
        if (tempSettings.graphics > maxQuality) tempSettings.graphics = maxQuality;

        switch (tempSettings.graphics)
        {
            case 0:
                graphicsTMPro.text = "Low";
                break;
            case 1:
                graphicsTMPro.text = "High";
                break;
            default:
                break;
        }
    }

    public void ChangeBrightness(int change)
    {
        tempSettings.brightness += change;
        if (tempSettings.brightness < 0) tempSettings.brightness = 0;
        if (tempSettings.brightness > 10) tempSettings.brightness = 10;

        brightnessTMPro.text = tempSettings.brightness.ToString();
    }

    public void ChangeMusic(int change)
    {
        tempSettings.music += change;
        if (tempSettings.music < 0) tempSettings.music = 0;
        if (tempSettings.music > 10) tempSettings.music = 10;

        musicTMPro.text = tempSettings.music.ToString();
    }

    public void ChangeUISounds(int change)
    {
        tempSettings.ui += change;
        if (tempSettings.ui < 0) tempSettings.ui = 0;
        if (tempSettings.ui > 10) tempSettings.ui = 10;

        uiTMPro.text = tempSettings.ui.ToString();
    }


    public void SaveSettings()
    {
        SetVariables();
        SaveSettingsFile();
        ApplySettingsToGame();

        string previousLanguage = PlayerPrefs.GetString("Language", "en");

        switch (settings.language)
        {
            case 0:
                PlayerPrefs.SetString("Language", "en");
                break;
            default:
                PlayerPrefs.SetString("Language", "en");
                break;
        }

        PlayerPrefs.Save();

        if (previousLanguage != PlayerPrefs.GetString("Language", "en"))
        {
            Michsky.LSS.LoadingScreen.LoadSceneFuturistic("MainMenu");
        }
    }

    private void SetVariables()
    {
        settings.language = tempSettings.language;
        settings.sensitivity = tempSettings.sensitivity;
        settings.smoothScrolling = scrollingToggle.isOn;

        settings.graphics = tempSettings.graphics;
        settings.resolution = tempResolution;
        settings.brightness = tempSettings.brightness;
        settings.fullScreen = fullScreenToggle.isOn;
        settings.vSync = vSyncToggle.isOn;

        settings.music = tempSettings.music;
        settings.ui = tempSettings.ui;
    }

    private void SaveSettingsFile()
    {
        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(Application.persistentDataPath + "/Settings.scp", json);
    }

    public void LoadSettings()
    {
        FixSettings();
        LoadFile();
        LoadVariables();
        ApplySettingsToGame();
    }

    private void FixSettings()
    {
        if (settings == null)
            settings = new SettingsData(GetResolutionIdex(Screen.currentResolution));

        if (tempSettings == null)
            tempSettings = new SettingsData(GetResolutionIdex(Screen.currentResolution));

        if (File.Exists(Application.persistentDataPath + "/Settings.scp"))
        {
            if (File.ReadAllText(Application.persistentDataPath + "/Settings.scp").Length < 10)
            {
                File.Delete(Application.persistentDataPath + "/Settings.scp");
            }
        }
    }

    private void LoadFile()
    {
        if (!File.Exists(Application.persistentDataPath + "/Settings.scp"))
        {
            SaveSettingsFile();
        }

        try
        {
            string jsonContent = File.ReadAllText(Application.persistentDataPath + "/Settings.scp");
            SettingsData tempData = JsonUtility.FromJson<SettingsData>(jsonContent);
            settings = tempData;
            tempSettings = tempData;

        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error while opening settings json file\n" + e);
        }

        tempResolution = Screen.resolutions.Length - 1;

        if (settings.resolution != -1)
            tempResolution = settings.resolution;
    }

    private void LoadVariables()
    {
        ChangeLanguage(0);
        scrollingToggle.isOn = settings.smoothScrolling;
        ChangeSensitivity(0);

        ChangeResolution(0);
        ChangeQuality(0);
        ChangeBrightness(0);
        fullScreenToggle.isOn = settings.fullScreen;
        vSyncToggle.isOn = settings.vSync;

        ChangeMusic(0);
        ChangeUISounds(0);
    }

    public void ApplySettingsToGame()
    {
        Debug.Log("Apling settings...");

        Resolution tempRes;

        if (settings.resolution < Screen.resolutions.Length)
            tempRes = Screen.resolutions[settings.resolution];
        else
            tempRes = Screen.resolutions[Screen.resolutions.Length - 1];

        Screen.SetResolution(tempRes.width, tempRes.height, settings.fullScreen, tempRes.refreshRate);

        if (globe != null)
        {
            globe.dragConstantSpeed = !settings.smoothScrolling;
            globe.mouseWheelSensitivity = settings.smoothScrolling ? 0.29f : 0.60f;
            globe.cameraRotationSensibility = 0.5f * settings.sensitivity;

            switch (settings.graphics)
            {
                case 0:
                    if (postProcess != null)
                        postProcess.enabled = false;
                    //globe.showProvinces = false;
                    //globe.frontiersDetail = FRONTIERS_DETAIL.Low;
                    break;
                case 1:
                    if (postProcess != null)
                        postProcess.enabled = true;
                    //globe.showProvinces = false;
                    //globe.frontiersDetail = FRONTIERS_DETAIL.High;
                    break;
            }
        }

        QualitySettings.vSyncCount = settings.vSync ? 1 : 0;

        SoundSystem.Instance.SetUpVolume();
    }

    private int GetResolutionIdex(Resolution _resolution)
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].height == _resolution.height
                && Screen.resolutions[i].width == _resolution.width
                && Screen.resolutions[i].refreshRate == _resolution.refreshRate)
                return i;
        }

        return Screen.resolutions.Length - 1;
    }
}
