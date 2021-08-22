using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyNote : MonoBehaviour
{
    [TextArea(3,10)]
    public string note;

    private void Start()
    {
        if (NotesDebugger.Instance != null)
            NotesDebugger.Instance.OnDebug.AddListener(DebugMessage);
    }

    public void DebugMessage()
    {
        Debug.Log("\nSTICKY NOTE\n" + note, gameObject);
    }
}
