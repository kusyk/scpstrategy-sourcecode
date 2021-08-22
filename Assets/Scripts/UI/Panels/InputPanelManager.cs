using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputPanelManager : MonoBehaviour
{
    public static InputPanelManager Instance;
    public static bool isActive = false;

    public TextMeshProUGUI header;
    public TextMeshProUGUI info;
    public Button approveButton;
    public UIPositionAnimator infoPanel;
    public TMP_InputField inputField;

    public GameEventString OnApproveClick;
    public GameEventString OnValidate;
    public GameEvent OnCancelClick;

    public void SetupPanel(string header, string defaultInput = "")
    {
        CleanEvents();
        SetApproveButton(false);
        SetHeaderText(header);
        inputField.text = defaultInput;
        infoPanel.Hide();
        inputField.onValueChanged.AddListener(delegate { Validate(); });
        UIController.Instance.ShowInputPanel();
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

    public void ApproveClick()
    {
        isActive = false;
        OnApproveClick.Invoke(inputField.text);
        CleanEvents();
    }

    public void Validate()
    {
        OnValidate.Invoke(inputField.text);
    }

    public void CancelClick()
    {
        isActive = false;

        OnCancelClick.Invoke();
        CleanEvents();
    }

    public void CleanEvents()
    {
        OnApproveClick.RemoveAllListeners();
        OnValidate.RemoveAllListeners();
        OnCancelClick.RemoveAllListeners();
    }
}
