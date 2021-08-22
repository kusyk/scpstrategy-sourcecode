using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitsScreen : MonoBehaviour
{
    public static UnitsScreen Instance;
    public GameObject unitPrefab;
    public RectTransform listContent;
    public ScrollRect scrollList;
    public TextMeshProUGUI requrementsLabel;

    private bool backToGame = false;

    public void OpenUnitsScreen(int selectedUnit = -1)
    {
        if (Instance != this)
            Instance = this;

        if (selectedUnit < 0)
            backToGame = false;
        else
            backToGame = true;

        RefreshList();
        ScrollToUnit(selectedUnit);
    }

    public void RefreshList()
    {
        int childCount = listContent.transform.childCount - 1;
        int unitsCount = PlayerController.PlayerInfo.units.Count;

        //destroy if more
        for (int i = childCount - 1; i >= unitsCount; i--)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }

        //add new (if need) and refresh all
        for (int i = 0; i < unitsCount; i++)
        {
            if (i < childCount)
                listContent.GetChild(i).GetComponent<UnitButton>().SetupButton(i, PlayerController.PlayerInfo.units[i].unitName);
            else
                Instantiate(unitPrefab, listContent.transform).GetComponent<UnitButton>().SetupButton(i, PlayerController.PlayerInfo.units[i].unitName);
        }

        listContent.GetChild(childCount).transform.SetAsLastSibling();

        requrementsLabel.gameObject.SetActive(false);
    }

    private void ScrollToUnit(int selectedUnit)
    {
        Canvas.ForceUpdateCanvases();

        if (selectedUnit >= listContent.childCount)
        {
            NotificationsManager.Instance.SpawnNotification("ERROR!", "Dunno why, but there are less buttons than you think it should be");
            selectedUnit = listContent.childCount - 1;
        }

        if (selectedUnit >= 0)
            listContent.GetChild(selectedUnit).GetComponent<UnitButton>().StartBlinking();

        if (selectedUnit < 0)
            selectedUnit = 0;

        Vector2 newPosition =
            (Vector2)scrollList.transform.InverseTransformPoint(listContent.position)
            - (Vector2)scrollList.transform.InverseTransformPoint(listContent.GetChild(selectedUnit).position + new Vector3(300, 0, 0));

        newPosition.x = Mathf.Clamp(newPosition.x, -100 - listContent.rect.width, -1900);

        newPosition.y = 0;
        listContent.anchoredPosition = newPosition;
    }

    public void AddUnit()
    {
        int moneyToRemove = 350;
        string errorMessage = PlayerController.CanCreateNewUnit(moneyToRemove);

        if (errorMessage != null)
        {
            requrementsLabel.text = errorMessage;
            requrementsLabel.gameObject.SetActive(true);
            return;
        }

        PlayerController.Instance.SetupCreatingNewUnit();
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
