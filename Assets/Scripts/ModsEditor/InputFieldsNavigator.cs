using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InputFieldsNavigator : MonoBehaviour
{
    public float yOffset = 70;

    EventSystem eventSystem;

    void Start()
    {
        eventSystem = EventSystem.current;
    }

    void LateUpdate()
    {
        try
        {
            if (Input.GetKeyDown(KeyCode.Tab) && !Input.GetKey(KeyCode.LeftShift))
            {
                if (eventSystem == null)
                {
                    if (EventSystem.current == null)
                    {
                        return;
                    }
                    eventSystem = EventSystem.current;
                }
                Debug.Log(Application.version);

                TMP_InputField currentInputfield = eventSystem.currentSelectedGameObject.GetComponent<TMP_InputField>();
                if (currentInputfield == null)
                    return;

                RemoveTabs_TMP_IF(currentInputfield);
                Selectable next;

                //if (Input.GetKey(KeyCode.LeftShift))
                //    next = eventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                //else
                next = eventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

                if (next == null)
                    return;

                eventSystem.SetSelectedGameObject(next.gameObject, new BaseEventData(eventSystem));

                TMP_InputField nextInputfield = next.GetComponent<TMP_InputField>();

                if (nextInputfield == null)
                    return;

                nextInputfield.caretPosition = nextInputfield.text.Length;
                //inputfield.OnPointerClick(new PointerEventData(eventSystem));

                Canvas.ForceUpdateCanvases();

                ScrollRect scrollRect = nextInputfield.GetComponentInParent<ScrollRect>();
                RectTransform content = nextInputfield.transform.parent.parent.GetComponent<RectTransform>();

                if (scrollRect == null || content == null)
                {
                    Debug.LogError("KURWO JEBAN!");
                    return;
                }

                Vector2 newPosition =
                    (Vector2)scrollRect.transform.InverseTransformPoint(content.position)
                    - (Vector2)scrollRect.transform.InverseTransformPoint(nextInputfield.transform.parent.position + new Vector3(0, yOffset, 0));

                newPosition.x = 0;
                content.anchoredPosition = newPosition;
            }
        }
        catch { }
    }

    private void RemoveTabs_TMP_IF(TMP_InputField inputfield)
    {
        inputfield.text = inputfield.text.Replace("\t", "");
    }
}