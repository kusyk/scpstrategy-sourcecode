using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillsScreen : MonoBehaviour
{
    public static SkillsScreen Instance;

    public VerticalLayoutGroup verticalLayoutGroup;

    [Space]
    public TextMeshProUGUI nameTMP;
    public TextMeshProUGUI statusTMP;
    public TextMeshProUGUI descriptionTMP;
    public TextMeshProUGUI rSkillTMP;
    public TextMeshProUGUI rScoreTMP;
    public TextMeshProUGUI rMoneyTMP;
    public TextMeshProUGUI rdevPTMP;
    public TextMeshProUGUI rActionPTMP;
    public GameObject activationButton;
    public GameObject reqErrorText;
    public GameObject infoLabel;

    public List<GameObject> uiInfoObjects = new List<GameObject>();

    private Skill visibleSkill = null;

    public void ActivateSkill()
    {
        if (visibleSkill == null)
            return;

        PlayerController.PlayerInfo.ChangeMoney(-visibleSkill.rMoney);
        PlayerController.PlayerInfo.researchPoints -= visibleSkill.rResearch;

        PlayerController.PlayerInfo.skills.Add(new SkillInfo(visibleSkill.name, 0.01f, visibleSkill.rAction));
        UIController.Instance.ShowSkillsScreen(visibleSkill);
    }

    public void ShowSkill(string skillId)
    {
        ShowSkill(SkillsManager.Instance.GetSkillById(skillId));
    }

    public void ShowSkill(Skill skill)
    {
        if (skill == null)
        {
            ClearSkillView();
            return;
        }

        SetInfoObjectsVisibility(true);

        activationButton.SetActive(false);
        reqErrorText.SetActive(false);

        visibleSkill = skill;

        string skillStatus = "";
        float skillProgress = SkillsManager.Instance.GetSkillProgress(visibleSkill);

        if (skillProgress == -1f)
        {
            ClearSkillView();
            return;
        }
        else if(skillProgress == 0f)
        {
            skillStatus = "<color=red>NOT ACTIVE</color>";

            if (visibleSkill.CanBeActivated)
                activationButton.SetActive(true);
            else
                reqErrorText.SetActive(true);
        }
        else if(skillProgress == 1f)
        {
            skillStatus = "<color=green>ACTIVE</color>";
        }
        else
        {
            skillStatus = (int)(skillProgress * 100) + "%";
        }

        if (skill.rSkill != "")
            rSkillTMP.text = "<b>" + SkillsManager.Instance.GetSkillById(skill.rSkill).fullName + "</b> required";
        else
            rSkillTMP.text = "Requirements:";

        nameTMP.text = skill.fullName;
        statusTMP.text = "Status: " + skillStatus;
        descriptionTMP.text = skill.description;
        rScoreTMP.text = skill.rScore.ToString();
        rMoneyTMP.text = "-" + skill.rMoney.ToString();
        rdevPTMP.text = "-" + skill.rResearch.ToString();
        rActionPTMP.text = skill.rAction.ToString();

        Canvas.ForceUpdateCanvases();
        verticalLayoutGroup.SetLayoutVertical();
    }

    public void ClearSkillView()
    {
        visibleSkill = null;
        nameTMP.text = "SELECT SKILL";
        statusTMP.text = "Status: ???";
        descriptionTMP.text = "???";
        rSkillTMP.text = "Requirements:";
        rScoreTMP.text = "???";
        rMoneyTMP.text = "???";
        rdevPTMP.text = "???";
        rActionPTMP.text = "???";
        activationButton.SetActive(false);

        SetInfoObjectsVisibility(false);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        if (visibleSkill == null)
        {
            if (infoLabel.activeSelf)
                infoLabel.SetActive(false);
            return;
        }

        float skillProgress = SkillsManager.Instance.GetSkillProgress(visibleSkill);

        if(skillProgress == 1f)
        {
            statusTMP.text = "Status: <color=green>ACTIVE</color>";

            if (infoLabel.activeSelf)
                infoLabel.SetActive(false);
        }
        else if (skillProgress > 0f)
        {
            statusTMP.text = "Status: " + (int)(skillProgress * 100) + "%";

            if (!infoLabel.activeSelf)
                infoLabel.SetActive(true);
        }

    }

    private void SetInfoObjectsVisibility(bool value)
    {
        for (int i = 0; i < uiInfoObjects.Count; i++)
        {
            uiInfoObjects[i].SetActive(value);
        }
    }
}
