using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using WPM;

public class SelectionPanelManager : MonoBehaviour
{
    public static SelectionPanelManager Instance;
    public static bool isActive = false;

    public TextMeshProUGUI header;
    public TextMeshProUGUI info;
    public Button approveButton;
    public UIPositionAnimator infoPanel;

    public GameEvent OnApproveClick;
    public GameEventInt OnValidate;
    public GameEvent OnCancelClick;

    private WorldMapGlobe map;
    private int lastSelectedCell = -1;

    public void SetupPanel(string header)
    {
        if (map == null)
            map = WorldMapGlobe.instance;

        CleanEvents();
        map.hexaGridHighlightEnabled = true;
        map.OnCellClick += Validate;
        SetApproveButton(false);
        SetHeaderText(header);
        infoPanel.Hide();
        UIController.Instance.ShowSelectionPanel();
        isActive = true;
    }

    public void SetHeaderText(string text)
    {
        header.text = text;
    }

    public void SetInfoText(string text)
    {
        if (text == null)
        {
            infoPanel.Hide();
            return;
        }

        info.text = text;
        infoPanel.Show();
    }

    public void SetApproveButton(bool status)
    {
        approveButton.interactable = status;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        map = WorldMapGlobe.instance;

        map.hexaGridHighlightEnabled = false;
    }

    public void ApproveClick()
    {
        isActive = false;
        OnApproveClick.Invoke();
        CleanEvents();
    }

    public void Validate(int clickedCell)
    {
        ColorsManager.Instance.RecolorCell(lastSelectedCell);
        lastSelectedCell = clickedCell;
        ColorsManager.Instance.SelectCell(lastSelectedCell);
        OnValidate.Invoke(lastSelectedCell);
    }

    public void CancelClick()
    {
        isActive = false;

        ColorsManager.Instance.RecolorCell(lastSelectedCell);

        //if (cancelListenersCount == 0)
        //{
        //    UIController.Instance.ShowGamePanels();
        //    NotificationsManager.Instance.SpawnNotification("Warning!", "no cancel event has been set", true, 100);
        //}

        OnCancelClick.Invoke();
        CleanEvents();
    }

    public void CleanEvents()
    {
        map.hexaGridHighlightEnabled = false;
        map.OnCellClick -= Validate;
        OnApproveClick.RemoveAllListeners();
        OnValidate.RemoveAllListeners();
        OnCancelClick.RemoveAllListeners();
    }
}
