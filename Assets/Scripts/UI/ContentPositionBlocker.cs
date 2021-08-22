using UnityEngine;
using UnityEngine.UI;

public class ContentPositionBlocker : MonoBehaviour
{
    public bool blockX = false;
    public bool blockY = false;

    public Vector2 anchoredPosition;

    private bool positionWasBlocked = false;

    void Update()
    {
        if (!positionWasBlocked)
        {
            Vector2 newPosition = GetComponent<RectTransform>().anchoredPosition;

            if (blockX)
                newPosition.x = anchoredPosition.x;

            if (blockY)
                newPosition.y = anchoredPosition.y;

            GetComponent<RectTransform>().anchoredPosition = newPosition;
            positionWasBlocked = true;
        }
    }
}
