using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
{
    public string skillId = "";

    [Header("UI Elements")]
    public TextMeshProUGUI skillName;
    public Image background;
    public Image image;

    [Header("Colors")]
    public Color unactiveColor;
    public Color hoverColor;
    public Color activeColor;
    public Color blockedColor;
    public Color fillingColor;

    [Header("Sprites")]
    public Sprite bulb;
    public Sprite calendar;
    public Sprite bulbOn;
    
    private Skill skill;
    private int skillStatus;
    private float skillProgress;

    void OnEnable()
    {
        if (PlayerController.PlayerInfo == null)
            return;

        skill = SkillsManager.Instance.GetSkillById(skillId);


        if (skill == null)
        {
            Debug.Log("Skill " + skillId + " does not exist.");
            return;
        }

        skillName.text = skill.fullName.ToString();

        UpdateSkillStatus();
        Unhover();
    }

    void LateUpdate()
    {
        if(background.color != hoverColor)
        {
            UpdateSkillStatus();
            Unhover();
        }
    }

    private void UpdateSkillStatus()
    {
        skillStatus = 69;

        skillProgress = SkillsManager.Instance.GetSkillProgress(skill);

        if (skillProgress <= 0f)
            skillStatus = 0;
        else if (skillProgress >= 1f)
            skillStatus = 2;
        else
            skillStatus = 1;
    }

    public void Unhover()
    {
        if (skill == null)
            return;

        switch (skillStatus)
        {
            case 0:
                background.fillAmount = 1;
                background.color = unactiveColor;
                image.sprite = bulb;
                break;
            case 1:
                background.fillAmount = skillProgress;
                background.color = fillingColor;
                image.sprite = calendar;
                break;
            case 2:
                background.fillAmount = 1;
                background.color = activeColor;
                image.sprite = bulbOn;
                break;
            default:
                break;
        }
    }

    public void Hover()
    {
        if (skill == null)
            return;

        if(skillStatus != 0)
        {
            return;
        }

        background.fillAmount = 1;
        background.color = hoverColor;

        if (skillStatus == 2)
            GetComponent<Button>().interactable = false;
    }

    public void Click()
    {
        SoundSystem.Instance.PlayClickSound();
        SkillsScreen.Instance.ShowSkill(skill);
        UIController.Instance.SelectUsefulButton();

        //oldSolution
        //if (skill == null)
        //    return;

        //if (skillStatus == 2)
        //    return;

        //if (!skill.CanBeActivated)
        //{
        //    background.color = blockedColor;
        //    return;
        //}

        //skillStatus = 2;

        //PlayerController.PlayerInfo.ChangeMoney(-skill.rMoney);
        //PlayerController.PlayerInfo.researchPoints -= skill.rResearch;

        //DemoController.Instance.demoSkills.Add(skill.skillId);

        //Unhover();

        //UIController.Instance.ShowSkillsScreen();

        //return;
        ////code not for demo!
        //++skill.status;

        //if (skill.status > 2)
        //    skill.status = 0;

        //if (skill.status != 0)
        //{
        //    Unhover();
        //    return;
        //}

        //background.fillAmount = 1;
        //background.color = blockedColor;
        //Unhover();
    }
}
