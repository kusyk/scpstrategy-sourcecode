using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnomalyInfo
{
    public string objectId;
    public int cellWithObject = -1;
    public int size;
    public List<int> cells;
    public bool active;

    public AnomalyInfo(int firstCell, int size = 5, string objectId = "")
    {
        cells = new List<int>();
        cells.Add(firstCell);

        this.objectId = objectId;
        this.size = size;
        this.active = true;
    }
}
