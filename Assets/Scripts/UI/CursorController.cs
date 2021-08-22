using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public bool recorderCursor = false;

    [Space]
    public Texture2D cursorTexture;
    public Vector2 cursorHotspot;


#if UNITY_STANDALONE_WIN
    void Start()
    {
        recorderCursor = false;
    }
#endif

    void Update()
    {
        if (recorderCursor == false)
            return;

        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.ForceSoftware);
    }
}
