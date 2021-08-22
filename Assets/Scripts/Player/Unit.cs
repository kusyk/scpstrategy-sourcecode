[System.Serializable]
public class Unit
{
    public int cellId = -1;
    public string unitName = "Unit #01";
    private int fundationMonth = -1;

    public int upgradeLevel = 1;

    public Status status = Status.Waiting;

    public float progressFloat = 0;

    public const int maxLevel = 3;

    public float SearchingTime
    {
        get
        {
            return (45 - (10 * upgradeLevel)) * (1.5f - (PlayerController.PlayerInfo.oldOutcomeUnits * 0.3f));
        }
    }

    public Unit(int _cellId, string _name, Status _status = Status.Waiting)
    {
        ChangeCell(_cellId);
        unitName = _name;
        fundationMonth = PlayerController.PlayerInfo.monthsPlayed;
        ChangeStatus(_status);
    }

    public void ChangeCell(int _cellId)
    {
        cellId = _cellId;
    }

    public void ChangeStatus(Status _status)
    {
        status = _status;
    }

    public void UpgradeUnit()
    {
        if (upgradeLevel < maxLevel)
            upgradeLevel++;
    }

    public int GetMonthsOfActivity
    {
        get
        {
            return PlayerController.PlayerInfo.monthsPlayed - fundationMonth;
        }
    }

    public string GetCoords
    {
        get
        {
            return FormatChanger.VectorToCoordinates(WPM.WorldMapGlobe.instance.cells[cellId].latlonCenter);
        }
    }

    public float GetProgress01
    {
        get
        {
            float value;
            switch (status)
            {
                case Status.Searching:
                    value = progressFloat / SearchingTime;
                    break;
                case Status.Founding:
                case Status.Waiting:
                default:
                    value = 0;
                    break;
            }
            return value;
        }
    }

    public void StartScan()
    {
        status = Status.Searching;
        progressFloat = 0;
    }

    public void FlyToMe()
    {
        WPM.WorldMapGlobe.instance.FlyToLocation(WPM.WorldMapGlobe.instance.cells[cellId].latlonCenter, 0.5f, 0.1f);
    }

    public enum Status
    {
        Founding,
        Waiting,
        Searching
    }
}