using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutsManager : MonoBehaviour
{
    public static ShortcutsManager Instance;
    public static bool activeGeneral = true;


    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (activeGeneral)
            CheckGeneralShortcuts();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIController.Instance.statsPanel.show == true)
            {
                if (UIController.Instance.statsPanelPauseButton.activeSelf)
                    UIController.Instance.ShowPauseScreen();
                else if (UIController.Instance.statsPanelBackButton.activeSelf)
                    UIController.Instance.ShowGamePanels();
            }
            else if (UIController.Instance.pauseScreen.activeSelf)
            {
                UIController.Instance.ShowGamePanels();
            }

            UIController.Instance.SelectUsefulButton();
        }
    }

    private void CheckGeneralShortcuts()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UIController.Instance.ShowManagementScreen();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            UIController.Instance.ShowBranchesScreen();
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            UIController.Instance.ShowUnitsScreen();
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            UIController.Instance.ShowSkillsScreen();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            UIController.Instance.ShowScpsScreen();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TimerManager.Instance.ChangeTimeSpeed(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TimerManager.Instance.ChangeTimeSpeed(1);
        }
    }
}
