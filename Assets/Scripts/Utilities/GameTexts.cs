using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTexts : MonoBehaviour
{
    public static GameTexts Instance;

    [TextArea(4, 20)]
    public string welcomeMessage;

    [Space(20)]
    [TextArea(4, 20)]
    public string firstAnnomalyMessage;

    [Space(20)]
    [TextArea(4, 20)]
    public string firstSkillMessage;

    [Space(20)]
    [TextArea(4, 20)]
    public string firstResearchMessage;

    [Space(20)]
    [TextArea(4, 20)]
    public string goodLuckMessage;

    [Space(20)]
    [TextArea(4, 20)]
    public string winMessage;

    [Space(20)]
    [TextArea(4, 20)]
    public string loseMessage;


    public static string WelcomeMessage { get => Instance.welcomeMessage; }
    public static string FirstAnnomalyMessage { get => Instance.firstAnnomalyMessage; }
    public static string FirstSkillMessage { get => Instance.firstSkillMessage; }
    public static string FirstResearchMessage { get => Instance.firstResearchMessage; }
    public static string GoodLuckMessage { get => Instance.goodLuckMessage; }
    public static string WinMessage { get => Instance.winMessage; }
    public static string LoseMessage { get => Instance.loseMessage; }


    private void Awake()
    {
        Instance = this;
    }
}
