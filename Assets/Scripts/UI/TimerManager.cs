using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;

    public float monthTime = 30;
    public float fadeOutSpeed = 1;

    [Space]
    public float monthProgress = 0;
    public float fadeOut = 0;
    public bool isWorking = true;

    [Space]
    public int[] timeSpeeds; 

    [Space]
    public Image timer;
    public Image[] timerArrows;
    public Color activeButtonColor;
    public Color inactiveButtonColor;
    public Color fillColor;
    public Color unfillColor;

    
    void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (isWorking)
            monthProgress += Time.deltaTime * PlayerController.gameTimeScale;
        else
            fadeOut -= Time.deltaTime * fadeOutSpeed;

        if (monthProgress >= monthTime)
        {
            monthProgress = 0;
            isWorking = false;
            fadeOut = 1;
            PlayerController.Instance.StartNextMonth();
        }
        else if (fadeOut <= 0)
        {
            isWorking = true;
            fadeOut = 0;
        }

        ControlImage();
    }

    public void ControlImage()
    {
        if (isWorking)
        {
            timer.fillAmount = monthProgress / monthTime;
            timer.color = fillColor;
        }
        else
        {
            timer.fillAmount = 1;
            timer.color = Color.Lerp(unfillColor, fillColor, fadeOut);
        }
    }

    public void ChangeTimeSpeed()
    {
        ChangeTimeSpeed(-1);
    }

    public void ChangeTimeSpeed(int selectedTimeSpeed)
    {
        if(selectedTimeSpeed != -1)
        {
            PlayerController.Instance.SetGameTimeScale(timeSpeeds[selectedTimeSpeed]);
            fadeOutSpeed = 1 + timeSpeeds[selectedTimeSpeed] / 3;
            return;
        }

        int selectedScale = 0;

        for (int i = 0; i < timeSpeeds.Length; i++)
        {
            if(PlayerController.gameTimeScale == timeSpeeds[i])
            {
                selectedScale = i + 1;
                break;
            }
        }

        if (selectedScale >= timeSpeeds.Length)
            selectedScale = 0;

        PlayerController.Instance.SetGameTimeScale(timeSpeeds[selectedScale]);
        fadeOutSpeed = 1 + timeSpeeds[selectedScale] / 3;
    }

    public void UpdateArrowsColors(float tempSpeed)
    {
        if (timeSpeeds.Length != timerArrows.Length)
        {
            Debug.LogError("Different lengths of arrays! All arrows will be white!");
            for (int i = 0; i < timerArrows.Length; i++)
                timerArrows[i].color = Color.white;
            return;
        }

        for (int i = 0; i < timeSpeeds.Length; i++)
            if (tempSpeed >= timeSpeeds[i])
                timerArrows[i].color = activeButtonColor;
            else
                timerArrows[i].color = inactiveButtonColor;
    }
}
