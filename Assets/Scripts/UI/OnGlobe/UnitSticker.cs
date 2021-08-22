using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSticker : MonoBehaviour
{
    private int unitId = -1;
    private Unit unitInfo;

    public Transform content;
    public Text label;
    public Image mainIcon;
    public Color normalIconColor;
    public Color hoverIconColor;
    public float scalingSpeed = 10;
    public RectTransform mainButtons;
    public RectTransform scanButtons;
    public Image scanBar;
    public Transform scan;
    public float changeSpeed = 50;
    public float scanRotationMultiplier = 1;
    private bool showContent = false;
    private bool destroy = false;

    private float stickerInitialSize;

    public bool ShowMainButtons { get => unitInfo.status == Unit.Status.Waiting; }
    public bool ShowScanButtons { get => unitInfo.status == Unit.Status.Searching; }

    public void SetupSticker(int id, string labelText)
    {
        unitId = id;
        unitInfo = PlayerController.PlayerInfo.units[unitId];
        label.text = labelText;

        content.localScale = showContent ? Vector3.one : Vector3.zero;
        mainButtons.anchoredPosition = ShowMainButtons ? Vector3.zero : Vector3.down * 100;
        scanButtons.anchoredPosition = ShowScanButtons ? Vector3.zero : Vector3.down * 100;
        scan.localScale = ShowScanButtons ? Vector3.one : Vector3.zero;
        stickerInitialSize = transform.localScale.x;
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        float scale = Mathf.MoveTowards(content.localScale.x, showContent ? 1 : 0, Time.deltaTime * scalingSpeed);
        content.localScale = new Vector3(scale, scale, scale);
        mainIcon.color = Color.Lerp(hoverIconColor, normalIconColor, scale);

        mainButtons.anchoredPosition = Vector3.MoveTowards(mainButtons.anchoredPosition, ShowMainButtons ? Vector3.zero : Vector3.down * 100, Time.deltaTime * changeSpeed);
        scanButtons.anchoredPosition = Vector3.MoveTowards(scanButtons.anchoredPosition, ShowScanButtons ? Vector3.zero : Vector3.down * 100, Time.deltaTime * changeSpeed);

        scale = Mathf.MoveTowards(scan.localScale.x, ShowScanButtons ? unitInfo.GetProgress01 / 3f * 2f + 1f/  3f : 0, Time.deltaTime * scalingSpeed);
        scan.localScale = new Vector3(scale, scale, scale);

        scan.Rotate(new Vector3(0, 0, Time.deltaTime * PlayerController.gameTimeScale * scanRotationMultiplier));
        scanBar.fillAmount = unitInfo.GetProgress01;

        if (stickerInitialSize != transform.localScale.x)
        {
            scale = Mathf.MoveTowards(transform.localScale.x, stickerInitialSize, Time.deltaTime * stickerInitialSize * scalingSpeed);
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void ChangePosition()
    {
        PlayerController.Instance.SetupChangingUnitPosition(unitId);
        SwitchShowingContent(false);
        SoundSystem.Instance.PlayClickSound();
    }

    public void OpenSettings()
    {
        UIController.Instance.ShowUnitsScreen(unitId);
        FlyToMe();
        SoundSystem.Instance.PlayClickSound();
    }

    public void FlyToMe()
    {
        PlayerController.PlayerInfo.units[unitId].FlyToMe();
    }

    public void SwitchShowingContent(bool value)
    {
        if (SelectionPanelManager.isActive)
            value = false;

        showContent = value;
    }

    public void StartScan()
    {
        unitInfo.StartScan();
        SoundSystem.Instance.PlayClickSound();
        UIController.Instance.SelectUsefulButton();
    }

    public void CancelScan()
    {
        unitInfo.status = Unit.Status.Waiting;
        SoundSystem.Instance.PlayClickSound();
    }
}
