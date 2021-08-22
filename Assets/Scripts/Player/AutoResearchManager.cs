using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoResearchManager : MonoBehaviour
{
    public static AutoResearchManager instance;

    [Range(0.1f, 60)]
    [SerializeField]
    private float standardDuration = 30;

    private List<int> researchObjects = new List<int>();

    public static int GetResearchCount => instance.researchObjects.Count;

    public float GetResearchDuration
    {
        get
        {
            float temp = standardDuration;
            if (SkillsManager.Instance.GetSkillProgress("ar3") == 1f)
                temp *= 0.4f;
            else if (SkillsManager.Instance.GetSkillProgress("ar2") == 1f)
                temp *= 0.7f;

            temp *= 0.3f * PlayerController.PlayerInfo.oldOutcomeScientists + 0.7f;

            return temp;
        }
    }

    public void StartResarchingObject(int objectIndex)
    {
        researchObjects.Add(objectIndex);
        PlayerController.PlayerInfo.objects[objectIndex].autoResearch = 0.001f;
    }

    void Awake()
    {
        instance = this;
        Debug.LogWarning("Remember to code adding researching objects after loading new save!");
    }

    void Update()
    {
        List<int> elementsToRemove = new List<int>();

        for (int i = 0; i < researchObjects.Count; i++)
        {
            PlayerController.PlayerInfo.objects[researchObjects[i]].autoResearch += PlayerController.gameUITimeScale * Time.deltaTime / GetResearchDuration;

            //Debug.Log("#" + researchObjects[i] + " " + PlayerController.PlayerInfo.objects[researchObjects[i]].autoResearch * 100 + "%");

            if (PlayerController.PlayerInfo.objects[researchObjects[i]].autoResearch >= 1)
            {
                PlayerController.PlayerInfo.objects[researchObjects[i]].autoResearch = 0;
                PlayerController.PlayerInfo.objects[researchObjects[i]].doneResearches++;
                PlayerController.PlayerInfo.objects[researchObjects[i]].recievedResearchPoins += 2;
                PlayerController.PlayerInfo.score += 20;
                PlayerController.PlayerInfo.conductedResearch++;
                PlayerController.PlayerInfo.researchPoints += 2;

                if (researchObjects[i] == ObjectsScreen.Instance.visibleObject)
                {
                    ObjectsScreen.Instance.ShowObject(researchObjects[i]);
                    SoundSystem.Instance.PlayNotificationSound();
                }
                else
                {
                    Notification tempNotification = NotificationsManager.Instance.SpawnNotification("RESEARCH COMPLETED", "<b>" + PlayerController.PlayerInfo.objects[researchObjects[i]].name + "</b><br>Click to open Scientific Department", true, -1, researchObjects[i]);
                    tempNotification.OnClickEventInt += UIController.Instance.ShowScpsScreen;
                    tempNotification.OnClickEvent += tempNotification.DestroyNotification;
                }

                elementsToRemove.Add(researchObjects[i]);

                AchievementsManager.Instance.CheckResearch();

                if(PlayerController.PlayerInfo.conductedResearch == 1)
                {
                    PlayerController.Instance.ShowGoodLuckMessage();
                }
            }
        }

        for (int i = 0; i < elementsToRemove.Count; i++)
        {
            researchObjects.Remove(elementsToRemove[i]);
        }
    }
}
