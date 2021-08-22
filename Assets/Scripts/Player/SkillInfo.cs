using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SkillInfo
{
    public string name;
    public float progress;
    public int actionPoints;

    public SkillInfo() { }

    public SkillInfo(string name, float progress, int actionPoints)
    {
        this.name = name;
        this.progress = progress;
        this.actionPoints = actionPoints;
    }
}
