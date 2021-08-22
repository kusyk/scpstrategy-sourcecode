using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DecisionPanel : MonoBehaviour
{
    public static DecisionPanel Instance;

    public TextMeshProUGUI headerTMP;
    public TextMeshProUGUI descriptionTMP;

    public Button backButton;
    public Button declineButton;
    public Button approveButton;

    public GameEvent OnBackClick;
    public GameEvent OnDeclineClick;
    public GameEvent OnApproveClick;

    public void SetupPanel(string header, string description, bool showBackButton = true, bool showDeclineButton = true, bool showApproveButton = true)
    {
        CleanEvents();
        headerTMP.text = header;
        descriptionTMP.text = description;

        backButton.gameObject.SetActive(showBackButton);
        declineButton.gameObject.SetActive(showDeclineButton);
        approveButton.gameObject.SetActive(showApproveButton);

        UIController.Instance.ShowDecisionPanel();
    }

    private void Awake()
    {
        Instance = this;
    }

    public void BackClick()
    {

        OnBackClick.Invoke();
        CleanEvents();
    }

    public void DeclineClick()
    {
        OnDeclineClick.Invoke();
        CleanEvents();
    }

    public void ApproveClick()
    {
        OnApproveClick.Invoke();
        CleanEvents();
    }

    public void CleanEvents()
    {
        OnBackClick.RemoveAllListeners();
        OnDeclineClick.RemoveAllListeners();
        OnApproveClick.RemoveAllListeners();
    }
}
