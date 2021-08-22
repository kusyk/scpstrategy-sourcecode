using UnityEngine;
using UnityEngine.UI;

public class BranchSticker: MonoBehaviour
{
    private int branchId = -1;

    public Transform content;
    public Text label;
    public float scalingSpeed = 3;
    public Transform holo;
    private bool showContent = false;

    private float stickerInitialSize;

    void Start()
    {
        content.localScale = showContent ? Vector3.one : Vector3.zero;
        stickerInitialSize = transform.localScale.x;
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        float scale = Mathf.MoveTowards(content.localScale.x, showContent ? 1 : 0, Time.deltaTime * scalingSpeed);
        content.localScale = new Vector3(scale, 0.5f * scale + 0.5f, scale);

        if(stickerInitialSize != transform.localScale.x)
        {
            scale = Mathf.MoveTowards(transform.localScale.x, stickerInitialSize, Time.deltaTime * stickerInitialSize * scalingSpeed);
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    public void OpenSettings()
    {
        if (SelectionPanelManager.isActive)
            return;

        UIController.Instance.ShowBranchesScreen(branchId);
        PlayerController.PlayerInfo.branches[branchId].FlyToMe();
        SoundSystem.Instance.PlayClickSound();
    }

    public void SetupSticker(int id, string labelText)
    {
        branchId = id;
        label.text = labelText;
        RefreshHoloSize();
    }

    public void RefreshHoloSize()
    {
        float scale = 0.75f + PlayerController.PlayerInfo.branches[branchId].upgradeLevel * 0.5f;
        holo.localScale = new Vector3(scale, scale, scale);
    }

    public void SwitchShowingContent(bool value)
    {
        if (SelectionPanelManager.isActive)
            value = false;

        showContent = value;
    }
}
