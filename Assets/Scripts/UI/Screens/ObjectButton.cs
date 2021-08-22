using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectButton : MonoBehaviour
{
    [Header("Colors")]
    public Color inactiveColor;
    public Color activeColor;
    public Color hoverColor;

    [Header("UI Elements")]
    public TextMeshProUGUI nameLabel;
    public Image bacground;


    public void MouseHover()
    {
        if (ObjectsScreen.Instance.visibleObject != transform.GetSiblingIndex())
            bacground.color = hoverColor;
    }

    public void MouseExit()
    {
        if (ObjectsScreen.Instance.visibleObject == transform.GetSiblingIndex())
            bacground.color = activeColor;
        else
            bacground.color = inactiveColor;
    }

    public void ClickObject()
    {
        ObjectsScreen.Instance.ShowObject(transform.GetSiblingIndex());
        SoundSystem.Instance.PlayClickSound();
    }

    public void RefreshButton()
    {
        ObjectInfo myObjectInfo = PlayerController.PlayerInfo.objects[transform.GetSiblingIndex()];
        nameLabel.text = myObjectInfo.name + "<size=18>   " + myObjectInfo.doneResearches + "/" + ObjectsManager.GetObjectDataById(myObjectInfo.id).ResearchesCount + "</size>";
        MouseExit();
    }
}
