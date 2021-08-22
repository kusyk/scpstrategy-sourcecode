using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPositionAnimator : MonoBehaviour
{
    [Tooltip("Pixels per second")]
    public float speed = 300;
    public Vector2 offsetPosition;
    private Vector2 startPosition;

    [HideInInspector]
    public bool show = true;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = startPosition + offsetPosition;
    }

    void LateUpdate()
    {
        if (show && rectTransform.anchoredPosition != startPosition)
        {
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, startPosition, Time.deltaTime * speed);
        }
        else if (!show && rectTransform.anchoredPosition != startPosition + offsetPosition)
        {
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, startPosition + offsetPosition, Time.deltaTime * speed);
        }
    }

    public void Show()
    {
        show = true;
    }

    public void Hide()
    {
        show = false;
    }
}
