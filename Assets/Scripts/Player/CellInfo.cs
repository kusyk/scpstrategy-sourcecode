using System.Collections.Generic;

[System.Serializable]
public class CellInfo
{
    public int cellId = -1;
    public CellType cellType = CellType.none;

    public CellInfo(int _cellId, CellType _cellType)
    {
        cellId = _cellId;
        cellType = _cellType;
    }

    public enum CellType
    {
        none,
        selected,
        branch,
        path,
        unit,
        anomaly
    }
}
