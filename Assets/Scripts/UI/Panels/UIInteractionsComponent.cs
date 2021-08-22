using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIInteractionsComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public GameEvent OnStart;
    public GameEvent OnMouseEnter;
    public GameEvent OnMouseExit;
    public GameEvent OnMouseDown;
    public GameEvent OnMouseUp;

    private void Start()
    {
        OnStart.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnMouseDown.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnMouseUp.Invoke();
    }
}