using System.Collections.Generic;

[System.Serializable]
public class PlayerInfo
{
    public string gamesaveid;
    public string name = "The Administrator";

    public bool mainGoal = false;

    //main statistics
    public int score = 0;
    public long money = 0;
    public int researchPoints = 0;

    //management
    public List<Branch> branches = new List<Branch>();
    public List<Unit> units = new List<Unit>();
    public List<SkillInfo> skills = new List<SkillInfo>();
    public List<ObjectInfo> objects = new List<ObjectInfo>();

    //management settings
    public int contribution = 3;
    public float outcomeUnits = 1.3f;
    public float outcomeScientists = 1.35f;

    //historical settings
    public int oldContibution = 3;
    public float oldOutcomeUnits = 1.3f;
    public float oldOutcomeScientists = 1.35f;

    //stats
    public float playTime = 0; // in minutes
    public float monthTime = 0;
    public int monthsPlayed = -1;
    public int checkedCases = 0;
    public int capturedObjects = 0;
    public int conductedResearch = 0;
    public float satisfactionLevel = 0.5f; // 0 - 1
    public ulong earnedMoney = 0;
    public ulong spentMoney = 0;


    //world gdp $80,934,771,028,340

    public void ChangeMoney(long change)
    {
        money += change;

        if (change < 0)
            spentMoney += (ulong)-change;
        else
            earnedMoney += (ulong)change;
    }
}
