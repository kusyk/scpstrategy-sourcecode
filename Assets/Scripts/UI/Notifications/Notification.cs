using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Notification : MonoBehaviour
{
    public float tempSpeed = 20;

    public TextMeshProUGUI headerTMPro;
    public TextMeshProUGUI contentTMPro;
    public GameObject destroyButton;

    public RectTransform topBorder;
    public RectTransform bottomBorder;

    public float notificationHeight = 180;
    public float maxBorder = 4;

    public Action OnClickEvent = delegate { };
    public Action<int> OnClickEventInt = delegate { };
    private int eventInt;
    public Action<string> OnClickEventString = delegate { };
    private string eventString;

    private bool destroy = false;
    private RectTransform rectTransform;

    private bool initialized = false;

    public void SetupNotification(string header, string content, bool destroyable = true, float customHeight = -1, int eventInt = 0, string eventString = "")
    {
        if (!initialized)
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0);
            initialized = true;
        }

        if(customHeight >= 50)
        {
            notificationHeight = customHeight;
        }

        if (!destroyable)
        {
            destroyButton.GetComponent<Button>().interactable = false;
        }

        headerTMPro.text = header;
        contentTMPro.text = content;
        this.eventInt = eventInt;
        this.eventString = eventString;
    }

    public void ChangeHeight(float customHeight)
    {
        if (customHeight >= 50)
        {
            notificationHeight = customHeight;
        }
    }


    private void LateUpdate()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (destroy)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, Mathf.Lerp(rectTransform.sizeDelta.y, -15, tempSpeed * Time.deltaTime));
            UpdateBordersSize();
        }
        else if (rectTransform.sizeDelta.y != notificationHeight)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, Mathf.Lerp(rectTransform.sizeDelta.y, notificationHeight, tempSpeed * Time.deltaTime));
            UpdateBordersSize();
        }
    }

    private void UpdateBordersSize()
    {
        topBorder.sizeDelta = new Vector2(topBorder.sizeDelta.x, Mathf.Clamp(rectTransform.sizeDelta.y, 0, maxBorder));
        bottomBorder.sizeDelta = new Vector2(bottomBorder.sizeDelta.x, Mathf.Clamp(rectTransform.sizeDelta.y, 0, maxBorder));
    }

    public void ClickNotification()
    {
        OnClickEvent();
        OnClickEventInt(eventInt);
        OnClickEventString(eventString);
        SoundSystem.Instance.PlayClickSound();
    }

    public void DestroyNotification()
    {
        if (destroy == true)
            return;

        destroy = true;
        Destroy(gameObject, 0.5f);
        SoundSystem.Instance.PlayClickSound();
        NotificationsManager.Instance.RemoveNotification(this);
    }
}
