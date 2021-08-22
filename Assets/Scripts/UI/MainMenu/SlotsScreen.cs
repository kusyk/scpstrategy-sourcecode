using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotsScreen : MonoBehaviour
{
    public MainMenuController menuController;

    [Space]
    public TextMeshProUGUI slotLabel1;
    public TextMeshProUGUI slotLabel2;
    public TextMeshProUGUI slotLabel3;
    public TextMeshProUGUI slotLabel4;
    public TextMeshProUGUI slotLabel5;

    [Space]
    public GameObject editSlotButton1;
    public GameObject editSlotButton2;
    public GameObject editSlotButton3;
    public GameObject editSlotButton4;
    public GameObject editSlotButton5;

    [Space]
    public GameObject emptySlotText1;
    public GameObject emptySlotText2;
    public GameObject emptySlotText3;
    public GameObject emptySlotText4;
    public GameObject emptySlotText5;

    [Space]
    public string emptySlotString = "empty slot";
    public string playString = "click to play";
    public string newGameString = "click to start new game";

    [Space]
    public List<GameObject> objectsToAutoHide = new List<GameObject>();

    private void OnEnable()
    {
        UpdateUI();
    }

    public void PlaySlot(string slotName)
    {
        SaveLoad.Load(slotName);
        menuController.Play();
    }

    //We don't have time to editing
    public void EditSlot(string slotName)
    {
        SaveLoad.Delete(slotName);
        UpdateUI();
    }

    private void UpdateUI()
    {
        PlayerInfo tempPlayerInfo1 = SaveLoad.Load("game1");
        PlayerInfo tempPlayerInfo2 = SaveLoad.Load("game2");
        PlayerInfo tempPlayerInfo3 = SaveLoad.Load("game3");
        PlayerInfo tempPlayerInfo4 = SaveLoad.Load("game4");
        PlayerInfo tempPlayerInfo5 = SaveLoad.Load("game5");
        SaveLoad.LoadedPlayerInfo = null;

        slotLabel1.text = tempPlayerInfo1 == null ? emptySlotString : tempPlayerInfo1.name;
        slotLabel2.text = tempPlayerInfo2 == null ? emptySlotString : tempPlayerInfo2.name;
        slotLabel3.text = tempPlayerInfo3 == null ? emptySlotString : tempPlayerInfo3.name;
        slotLabel4.text = tempPlayerInfo4 == null ? emptySlotString : tempPlayerInfo4.name;
        slotLabel5.text = tempPlayerInfo5 == null ? emptySlotString : tempPlayerInfo5.name;

        editSlotButton1.SetActive(tempPlayerInfo1 != null);
        editSlotButton2.SetActive(tempPlayerInfo2 != null);
        editSlotButton3.SetActive(tempPlayerInfo3 != null);
        editSlotButton4.SetActive(tempPlayerInfo4 != null);
        editSlotButton5.SetActive(tempPlayerInfo5 != null);

        emptySlotText1.GetComponent<TextMeshProUGUI>().text = tempPlayerInfo1 == null ? newGameString : playString;
        emptySlotText2.GetComponent<TextMeshProUGUI>().text = tempPlayerInfo2 == null ? newGameString : playString;
        emptySlotText3.GetComponent<TextMeshProUGUI>().text = tempPlayerInfo3 == null ? newGameString : playString;
        emptySlotText4.GetComponent<TextMeshProUGUI>().text = tempPlayerInfo4 == null ? newGameString : playString;
        emptySlotText5.GetComponent<TextMeshProUGUI>().text = tempPlayerInfo5 == null ? newGameString : playString;

        for (int i = 0; i < objectsToAutoHide.Count; i++)
        {
            objectsToAutoHide[i].SetActive(false);
        }
    }
}
