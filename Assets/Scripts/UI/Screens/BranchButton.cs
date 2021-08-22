using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BranchButton : MonoBehaviour
{
    public TextMeshProUGUI headerTMP;
    public TextMeshProUGUI levelTMP;
    public TextMeshProUGUI infoTMP;
    public Button upgradeBtn;
    public Image blinkerImage;

    public int branchIndex = -1;
    public float defaultBlinkingTime = 4;
    public float blinkingSpeed = 2;
    private float blinkingTimer;

    public void SetupButton(int _branchIndex, string header)
    {
        branchIndex = _branchIndex;
        headerTMP.text = header;
        RefreshInfo();
    }

    public void FlyToMe()
    {
        PlayerController.PlayerInfo.branches[branchIndex].FlyToMe();
        UIController.Instance.ShowGamePanels();
        SoundSystem.Instance.PlayClickSound();
    }

    public void Upgrade()
    {
        if (PlayerController.PlayerInfo.researchPoints < 3)
            return;

        PlayerController.PlayerInfo.researchPoints -= 3;

        PlayerController.PlayerInfo.branches[branchIndex].UpgradeBranch();
        RefreshInfo();
        ColorsManager.Instance.RecolorWholeGlobe();
        StickersManager.Instance.RefreshStickerHolo(branchIndex);
        SoundSystem.Instance.PlayClickSound();

        AchievementsManager.Instance.CheckBranchesUp();
    }

    public void ChangeName()
    {
        PlayerController.Instance.SetupChangingBranchName(branchIndex);
        SoundSystem.Instance.PlayClickSound();
    }

    public void StartBlinking()
    {
        blinkingTimer = defaultBlinkingTime;
    }

    private void RefreshInfo()
    {
        Branch myBranch = PlayerController.PlayerInfo.branches[branchIndex];
        int level = myBranch.upgradeLevel;
        int months = myBranch.GetMonthsOfActivity;
        int employees = 20 * (level + 1);
        levelTMP.text = "level: " + level + "/3";
        infoTMP.text = "employees\t:  " + employees + "\n";
        infoTMP.text += "founded\t:  " + months + " months ago\n";
        infoTMP.text += "position\t:  " + myBranch.GetCoords;

        if (PlayerController.PlayerInfo.branches[branchIndex].upgradeLevel < Branch.maxLevel)
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
        if(blinkingTimer < 0)
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
