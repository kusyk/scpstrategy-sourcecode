using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsManager : MonoBehaviour
{
    public static SkillsManager Instance;

    public float defaultSkillTime = 30;

    public List<Skill> skills = new List<Skill>();


    public Skill GetSkillById(string skillId)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i].name == skillId)
            {
                return skills[i];
            }
        }
        return null;
    }

    public float GetSkillProgress(string skillId)
    {
        return GetSkillProgress(GetSkillById(skillId));
    }

    public float GetSkillProgress(Skill skill)
    {
        if (skill == null)
            return -1f;

        for (int i = 0; i < PlayerController.PlayerInfo.skills.Count; i++)
        {
            if (PlayerController.PlayerInfo.skills[i].name == skill.name)
            {
                return PlayerController.PlayerInfo.skills[i].progress;
            }
        }

        return 0f;
    }

    public string GetSkillFullName(string skillId)
    {

        Skill tempSkill = GetSkillById(skillId);

        if (tempSkill == null)
        {
            Debug.Log("GetSkillById failed: " + skillId);
            return "#";
        }

        return GetSkillById(skillId).fullName;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdatePlayersSkills();
    }

    private void UpdatePlayersSkills()
    {
        for (int i = 0; i < PlayerController.PlayerInfo.skills.Count; i++)
        {
            if(PlayerController.PlayerInfo.skills[i].progress < 1f)
            {
                PlayerController.PlayerInfo.skills[i].progress += PlayerController.gameUITimeScale * Time.deltaTime / GetSkillActivationTime();

                if (PlayerController.PlayerInfo.skills[i].progress > 1f)
                {
                    Notification tempNotification = NotificationsManager.Instance.SpawnNotification("Engineering Department", 
                        GetSkillFullName(PlayerController.PlayerInfo.skills[i].name) + " has been activated", true, -1, 0, PlayerController.PlayerInfo.skills[i].name);

                    tempNotification.OnClickEvent += tempNotification.DestroyNotification;
                    tempNotification.OnClickEventString += UIController.Instance.ShowSkillsScreenById;

                    PlayerController.PlayerInfo.skills[i].progress = 1f;
                    PlayerController.PlayerInfo.skills[i].actionPoints = 0;

                    AchievementsManager.Instance.CheckSkills();

                    if(PlayerController.PlayerInfo.skills.Count == 1)
                    {
                        PlayerController.Instance.ShowFirstResearchMessage();
                    }
                }
            }
        }
    }

    private float GetSkillActivationTime()
    {
        if (GetSkillProgress("ad2") == 1f)
            return defaultSkillTime * 0.5f;
        if (GetSkillProgress("ad1") == 1f)
            return defaultSkillTime * 0.75f;
        return defaultSkillTime;
    }
}
