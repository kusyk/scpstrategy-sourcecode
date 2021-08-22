using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BranchesScreen : MonoBehaviour
{
    public static BranchesScreen Instance;
    public GameObject branchPrefab;
    public RectTransform listContent;
    public ScrollRect scrollList;
    public TextMeshProUGUI requrementsLabel;

    private bool backToGame = false;

    public void OpenBranchesScreen(int selectedBranch = -1)
    {
        if (Instance != this)
            Instance = this;

        if (selectedBranch < 0)
            backToGame = false;
        else
            backToGame = true;

        RefreshList();
        ScrollToBranch(selectedBranch);
    }

    public void RefreshList()
    {
        int childCount = listContent.transform.childCount - 1;
        int branchesCount = PlayerController.PlayerInfo.branches.Count;

        //destroy if more
        for (int i = childCount - 1; i >= branchesCount; i--)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }

        //add new (if need) and refresh all
        for (int i = 0; i < branchesCount; i++)
        {
            if (i < childCount)
                listContent.GetChild(i).GetComponent<BranchButton>().SetupButton(i, PlayerController.PlayerInfo.branches[i].name);
            else
                Instantiate(branchPrefab, listContent.transform).GetComponent<BranchButton>().SetupButton(i, PlayerController.PlayerInfo.branches[i].name);
        }

        listContent.GetChild(childCount).transform.SetAsLastSibling();

        requrementsLabel.gameObject.SetActive(false);
    }

    private void ScrollToBranch(int selectedBranch)
    {
        Canvas.ForceUpdateCanvases();


        if (selectedBranch >= listContent.childCount)
        {
            NotificationsManager.Instance.SpawnNotification("ERROR!", "Dunno why, but there are less buttons than you think it should be");
            return;
        }

        if (selectedBranch >= 0)
            listContent.GetChild(selectedBranch).GetComponent<BranchButton>().StartBlinking();

        if (selectedBranch < 0)
            selectedBranch = 0;

        Vector2 newPosition =
            (Vector2)scrollList.transform.InverseTransformPoint(listContent.position)
            - (Vector2)scrollList.transform.InverseTransformPoint(listContent.GetChild(selectedBranch).position + new Vector3(300, 0, 0));

        newPosition.y = 0;
        listContent.anchoredPosition = newPosition;
    }

    public void AddBranch()
    {
        int moneyToRemove = 600;
        string errorMessage = PlayerController.CanOpenNewBranch(moneyToRemove);

        if (errorMessage != null)
        {
            requrementsLabel.text = errorMessage;
            requrementsLabel.gameObject.SetActive(true);
            return;
        }

        PlayerController.Instance.SetupCreatingNewBranch();
    }

    public void Back()
    {
        UIController.Instance.ShowGamePanels();
        return;

        //old solution
        if (backToGame)
            UIController.Instance.ShowGamePanels();
        else
            UIController.Instance.ShowManagementScreen();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
