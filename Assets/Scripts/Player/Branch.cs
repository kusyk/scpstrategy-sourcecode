[System.Serializable]
public class Branch
{
    public int cellId = -1;
    public string name = "Branch #01";
    public int nearestBranchId = -1;
    private int fundationMonth = -1;

    public int upgradeLevel = 1;

    public const int maxLevel = 3;

    public Branch(int _cellId, string _name, int _nearestBranchId = -1)
    {
        cellId = _cellId;
        name = _name;
        nearestBranchId = _nearestBranchId;
        fundationMonth = PlayerController.PlayerInfo.monthsPlayed;
    }

    public void UpgradeBranch()
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

    public void FlyToMe()
    {
        WPM.WorldMapGlobe.instance.FlyToLocation(WPM.WorldMapGlobe.instance.cells[cellId].latlonCenter, 0.5f, 0.1f);
    }

}