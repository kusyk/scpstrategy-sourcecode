using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesDebugger : MonoBehaviour
{
    public static NotesDebugger Instance;

    public GameEvent OnDebug;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(DebugInNextFrame());
    }

    private IEnumerator DebugInNextFrame()
    {
        yield return new WaitForEndOfFrame();
        OnDebug.Invoke();
    }
}
