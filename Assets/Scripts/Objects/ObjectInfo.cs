using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ObjectInfo
{
    public string id;
    public string name;
    public Vector2 coords;
    public int doneResearches;
    public int recievedResearchPoins;
    public float autoResearch;

    public ObjectInfo(string id, string name, Vector2 coords, int doneResearches = 0, int recievedResearchPoins = 0)
    {
        this.id = id;
        this.name = name;
        this.coords = coords;
        this.doneResearches = doneResearches;
        this.recievedResearchPoins = recievedResearchPoins;
        this.autoResearch = 0;
    }
}
