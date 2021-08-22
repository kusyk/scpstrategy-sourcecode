using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Minigames;

public class MinigamesController : MonoBehaviour
{
    public static MinigamesController Instance;

    public List<MinigameInfo> minigames = new List<MinigameInfo>();

    [Header("Dunno why is it public")]
    public int objectIndex;
    public MinigameBehaviour activeMinigame;
    public float timer;
    public bool showingResult = false;
    public bool finalResult;

    [Header("Main UI")]
    public GameObject mainPanel;
    public TextMeshProUGUI objectLabel;
    public TextMeshProUGUI researchLabel;
    public TextMeshProUGUI timerLabel;
    public TextMeshProUGUI descriptionLabel;

    [Header("Result UI")]
    public Image resultPanel;
    public TextMeshProUGUI successTMP;
    public TextMeshProUGUI defeatTMP;

    [Header("Other settings")]
    public List<GameObject> objectsToHide = new List<GameObject>();

    public static bool isAnyMinigameActive
    {
        get
        {
            if (Instance == null)
                return false;

            return Instance.activeMinigame == null ? false : true;
        }
    }

    public void SetupMinigame(int _objectIndex)
    {
        if (PlayerController.PlayerInfo == null)
            return;

        objectIndex = _objectIndex;
        ObjectInfo selectedObject = PlayerController.PlayerInfo.objects[objectIndex];
        ObjectsManager.MinigameType minigameType = ObjectsManager.GetObjectDataById(selectedObject.id).json.minigame;

        if (minigameType == ObjectsManager.MinigameType.None)
            return;

        SetActiveGameObjects(false);
        mainPanel.SetActive(true);
        UIController.Instance.HideAllPanels(0);

        timer = 30;
        
        objectLabel.text = "Object: " + selectedObject.name;
        researchLabel.text = "Research: #" + (selectedObject.doneResearches + 1);

        SoundSystem.Instance.PlayMinigameClip();


        for (int i = 0; i < minigames.Count; i++)
        {
            if (minigames[i].minigame == minigameType)
            {
                activeMinigame = Instantiate(minigames[i].prefab).GetComponent<MinigameBehaviour>();
                minigames[i].ui.gameObject.SetActive(true);
                minigames[i].ui.UpdateUI();
                descriptionLabel.text = minigames[i].description;
                timer = activeMinigame.GetMinigameDuration;
                return;
            }
        }
    }

    public void SetupTestMinigame(ObjectsManager.MinigameType minigameType)
    {
        if (minigameType == ObjectsManager.MinigameType.None)
            return;

        for (int i = 0; i < minigames.Count; i++)
        {
            if (minigames[i].minigame == minigameType)
            {
                activeMinigame = Instantiate(minigames[i].prefab).GetComponent<MinigameBehaviour>();
                minigames[i].ui.UpdateUI();
                timer = 30;
                return;
            }
        }
    }

    public void FinishMinigame(bool result)
    {
        if (showingResult)
            return;

        showingResult = true;
        timer = 0;
        finalResult = result;

        resultPanel.gameObject.SetActive(true);

        if (result == true)
            successTMP.gameObject.SetActive(true);
        else
            defeatTMP.gameObject.SetActive(true);

        UpdateTimer();

        SoundSystem.Instance.PlayMinigameResultSound();
    }

    public void CloseMinigame()
    {
        if (finalResult == true)
        {
            PlayerController.PlayerInfo.objects[objectIndex].doneResearches++;
            PlayerController.PlayerInfo.objects[objectIndex].recievedResearchPoins += 3;
            PlayerController.PlayerInfo.score += 30;
            PlayerController.PlayerInfo.conductedResearch++;
            PlayerController.PlayerInfo.researchPoints += PlayerController.GetManualResearchReward();

            AchievementsManager.Instance.CheckResearch();
        }

        UIController.Instance.ShowScpsScreen(objectIndex);
        HideAllPanels();
        SetActiveGameObjects(true);
        Destroy(activeMinigame.gameObject);
        activeMinigame = null;
        showingResult = false;
        SoundSystem.Instance.SetUpVolume();
    }

    private void Awake()
    {
        Instance = this;
        HideAllPanels();
    }

    private void Start()
    {
        //only for tests
        //SetupTestMinigame(ObjectsManager.MinigameType.Waves);
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (activeMinigame == null)
            return;

        timer -= Time.deltaTime;

        if (!showingResult)
            timerLabel.text = ((int)GetTimerPositive / 60) + ":" + FormatChanger.TimeToString((int)GetTimerPositive%60);

        if (timer < 0)
        {
            if (!showingResult)
            {
                FinishMinigame(activeMinigame.resultOnTimeout);
            }
            else
            {
                float tempAlfa = Mathf.Clamp01(-timer * 3);
                resultPanel.color = new Color(resultPanel.color.r, resultPanel.color.g, resultPanel.color.b, tempAlfa * 0.9f);
                successTMP.color = new Color(successTMP.color.r, successTMP.color.g, successTMP.color.b, tempAlfa);
                defeatTMP.color = new Color(defeatTMP.color.r, defeatTMP.color.g, defeatTMP.color.b, tempAlfa);

                if(timer < -2)
                {
                    CloseMinigame();
                }
            }
        }
    }

    private void HideAllPanels()
    {
        mainPanel.SetActive(false);

        for (int i = 0; i < minigames.Count; i++)
        {
            minigames[i].ui.gameObject.SetActive(false);
        }

        resultPanel.gameObject.SetActive(false);
        successTMP.gameObject.SetActive(false);
        defeatTMP.gameObject.SetActive(false);
    }

    private void SetActiveGameObjects(bool status)
    {
        for (int i = 0; i < objectsToHide.Count; i++)
        {
            objectsToHide[i].SetActive(status);
        }
    }

    public static float GetTimerPositive
    {
        get
        {
            return Mathf.Clamp(Instance.timer, 0, int.MaxValue);
        }
    }

    [Serializable]
    public class MinigameInfo
    {
        public string name;
        public ObjectsManager.MinigameType minigame;
        public GameObject prefab;//UpdateUI
        public MinigameUI ui;
        [TextArea(3,5)]
        public string description;
    }
}
