using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitButton : MonoBehaviour
{
    public TextMeshProUGUI headerTMP;
    public TextMeshProUGUI levelTMP;
    public TextMeshProUGUI infoTMP;
    public Button upgradeBtn;
    public Image blinkerImage;

    public int unitIndex = -1;
    public float defaultBlinkingTime = 4;
    public float blinkingSpeed = 2;
    private float blinkingTimer;
    public void SetupButton(int _unitIndex, string header)
    {
        unitIndex = _unitIndex;
        headerTMP.text = header;
        RefreshInfo();
    }

    public void FlyToMe()
    {
        PlayerController.PlayerInfo.units[unitIndex].FlyToMe();
        UIController.Instance.ShowGamePanels();
        SoundSystem.Instance.PlayClickSound();
    }

    public void Upgrade()
    {
        if (PlayerController.PlayerInfo.researchPoints < 2)
            return;

        PlayerController.PlayerInfo.researchPoints -= 2;

        PlayerController.PlayerInfo.units[unitIndex].UpgradeUnit();
        RefreshInfo();
        SoundSystem.Instance.PlayClickSound();

        AchievementsManager.Instance.CheckMTFsUp();
    }

    public void ChangeName()
    {
        PlayerController.Instance.SetupChangingUnitName(unitIndex);
        SoundSystem.Instance.PlayClickSound();
    }

    public void StartBlinking()
    {
        blinkingTimer = defaultBlinkingTime;
    }

    private void RefreshInfo()
    {
        Unit myUnit = PlayerController.PlayerInfo.units[unitIndex];
        int level = myUnit.upgradeLevel;
        int months = myUnit.GetMonthsOfActivity;
        int employees = 10 * (level + 1);
        levelTMP.text = "level: " + level + "/3";
        infoTMP.text = "employees\t:  " + employees + "\n";
        infoTMP.text += "founded\t:  " + months + " months ago\n";
        infoTMP.text += "position\t:  " + myUnit.GetCoords;

        if (PlayerController.PlayerInfo.units[unitIndex].upgradeLevel < Branch.maxLevel)
            upgradeBtn.interactable = true;
        else
            upgradeBtn.interactable = false;

        blinkingTimer = 0;
        UpdateBlinker();
    }

    private void Update()
    {
        if (blinkingTimer > 0)
        {
            blinkingTimer -= Time.deltaTime * blinkingSpeed;
            UpdateBlinker();
        }
        if (blinkingTimer < 0)
        {
            blinkingTimer = 0;
            UpdateBlinker();
        }
    }

    private void UpdateBlinker()
    {
        Color tempColor = blinkerImage.color;
        tempColor.a = Mathf.PingPong(blinkingTimer, 1);
        blinkerImage.color = tempColor;
    }
}