using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModButton : MonoBehaviour
{
    private string objectID;

    public TextMeshProUGUI idText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public RawImage iconImage;

    private bool destory = false;
    private RectTransform rectTransform;
    private float initialHeight;

    public void SetupModButton(ObjectsManager.ObjectData tempObject)
    {
        objectID = tempObject.json.id;
        idText.text = objectID;
        nameText.text = tempObject.json.steamName;
        descriptionText.text = tempObject.json.steamDescription;
        iconImage.texture = tempObject.icon != null ? tempObject.icon : ModsUIController.Instance.standardIcon;
    }

    public void OpenMod()
    {
        if (destory)
            return;

        ModsUIController.Instance.ShowObjectEditorScreen(objectID);
    }

    public void RemoveMod()
    {
        if (destory)
            return;

        destory = true;
        Destroy(gameObject, 1);
        ObjectsManager.DeleteObjectById(objectID);
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialHeight = rectTransform.sizeDelta.y;
    }


    private void LateUpdate()
    {
        if (destory)
        {
            Vector3 newLocalScale = gameObject.transform.localScale * Mathf.MoveTowards(gameObject.transform.localScale.x, 0, Time.deltaTime);
            gameObject.transform.localScale = newLocalScale;

            Vector2 newSizeDelta = rectTransform.sizeDelta;
            newSizeDelta.y = Mathf.MoveTowards(rectTransform.sizeDelta.y, 0, Time.deltaTime * initialHeight);
            rectTransform.sizeDelta = newSizeDelta;
        }
    }
}
