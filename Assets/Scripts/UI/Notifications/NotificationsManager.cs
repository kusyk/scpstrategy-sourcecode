using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationsManager : MonoBehaviour
{
    public static NotificationsManager Instance;

    public GameObject notificationPrefab;
    public Transform notificationsPanel;
    public GameObject clearButton;
    public int buttonVisibilityLimit = 3;

    private List<Notification> notifications = new List<Notification>();

    private void Awake()
    {
        Instance = this;
        UpdateClearButton();
    }

    public Notification SpawnNotification (string header, string content, bool destroyable = true, float customHeight = -1, int eventInt = 0, string eventString = "")
    {
        Notification tempNotification 
            = Instantiate(notificationPrefab, notificationsPanel).GetComponent<Notification>();

        tempNotification.SetupNotification(header, content, destroyable, customHeight, eventInt, eventString);

        SoundSystem.Instance.PlayNotificationSound();

        notifications.Add(tempNotification);
        UpdateClearButton();

        return tempNotification;
    }

    public void RemoveNotification(Notification notification)
    {
        if (notifications.Contains(notification))
        {
            notifications.Remove(notification);
            UpdateClearButton();
        }
    }

    public void RemoveAllNotifications()
    {
        for (int i = notifications.Count - 1; i >= 0; i--)
        {
            notifications[i].DestroyNotification();
        }
    }

    public void UpdateClearButton()
    {
        if (notifications.Count > buttonVisibilityLimit && !clearButton.activeSelf)
            clearButton.SetActive(true);
        else if (notifications.Count <= buttonVisibilityLimit && clearButton.activeSelf)
            clearButton.SetActive(false);
    }
}
