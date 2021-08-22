using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WPM;

public class ColorsManager : MonoBehaviour
{
    public static ColorsManager Instance;

    public Color selectedCellColor = Color.red;
    public Color branchCellColor;
    public Color pathBranchCellColor;
    public Color pathUnitCellColor;
    public Color badPathUnitCellColor;
    public Color anomalyCellColor;
    [Range(0f, 1f)]
    public float invisibleCellColorMultiplier = 0.5f;

    private WorldMapGlobe map;

    private List<CellInfo> cells = new List<CellInfo>();

    public void RecolorWholeGlobe()
    {
        RefreshCellsList();
        RecolorWholeGlobeWithoutRefresh();
    }

    public void RecolorWholeGlobeWithoutRefresh()
    {
        map.ClearCells();

        //paint everything
        for (int i = 0; i < cells.Count; i++)
        {
            Color tempColor = GetColorByCellInfo(cells[i]);

            if (tempColor == Color.clear)
                map.ClearCell(cells[i].cellId);
            else
                map.SetCellColor(cells[i].cellId, tempColor);
        }
    }

    public void RecolorCell(int cellId)
    {
        map.ClearCell(cellId);

        Color tempColor = GetColorByCellInfo(GetCellInfoByCellIndex(cellId));

        if (tempColor == Color.clear)
            map.ClearCell(cellId);
        else
            map.SetCellColor(cellId, tempColor);

        Debug.LogWarning("Recoloring one cell is not perfect. Be careful!!");
    }

    public void PaintPath(List<int> path, bool pathIsRight)
    {
        if (path == null)
            return;

        for (int i = 0; i < path.Count; i++)
        {
            Color tempColor = pathIsRight ? pathUnitCellColor : badPathUnitCellColor;

            if (!map.cells[path[i]].visible)
                tempColor.a *= invisibleCellColorMultiplier;

            map.SetCellColor(path[i], tempColor);
        }
    }

    public void SelectCell(int cellId)
    {
        map.SetCellColor(cellId, selectedCellColor);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        map = WorldMapGlobe.instance;
    }

    private void RefreshCellsList()
    {
        cells.Clear();

        //anomalies
        for (int i = 0; i < AnomaliesManager.instance.anomalies.Count; i++)
        {
            for (int j = 0; j < AnomaliesManager.instance.anomalies[i].cells.Count; j++)
            {
                if (AnomaliesManager.instance.anomalies[i].active)
                {
                    cells.Add(new CellInfo(AnomaliesManager.instance.anomalies[i].cells[j], CellInfo.CellType.anomaly));
                }
            }
        }

        //paths
        for (int i = 0; i < PlayerController.PlayerInfo.branches.Count; i++)
        {
            Branch tempBranch = PlayerController.PlayerInfo.branches[i];

            if (tempBranch.nearestBranchId < 0)
                continue;

            List<int> tempPath = new List<int>();
            tempPath = map.FindPath(tempBranch.cellId, PlayerController.PlayerInfo.branches[tempBranch.nearestBranchId].cellId, 0, false, WPM.PathFinding.HiddenCellsFilterMode.UseAllCells);

            if (tempPath == null)
                continue;

            for (int j = 0; j < tempPath.Count; j++)
            {
                if (GetCellInfoByCellIndex(tempPath[j]) == null)
                {
                    cells.Add(new CellInfo(tempPath[j], CellInfo.CellType.path));
                }
            }
        }

        //branches
        for (int i = 0; i < PlayerController.PlayerInfo.branches.Count; i++)
        {
            Branch tempBranch = PlayerController.PlayerInfo.branches[i];

            List<int> nearCells = map.GetCellNeighboursIndices(tempBranch.cellId).OfType<int>().ToList();

            for (int x = 0; x < tempBranch.upgradeLevel; x++)
            {
                int neighbours = nearCells.Count;

                for (int y = 0; y < neighbours; y++)
                {
                    List<int> tempList = map.GetCellNeighboursIndices(nearCells[y]).OfType<int>().ToList();
                    tempList.ForEach(item => nearCells.Add(item));
                }
                nearCells = nearCells.Distinct().ToList();
            }

            for (int j = 0; j < nearCells.Count; j++)
            {
                cells.Add(new CellInfo(nearCells[j], CellInfo.CellType.branch));
            }
        }
    }

    private CellInfo GetCellInfoByCellIndex(int cellIndex)
    {
        for (int i = 0; i < cells.Count; i++)
            if(cells[i].cellId == cellIndex)
                return cells[i];

        return null;
    }

    private Color GetColorByCellInfo(CellInfo cellInfo)
    {
        if (cellInfo == null)
            return Color.clear;

        Color tempColor;

        switch (cellInfo.cellType)
        {
            case CellInfo.CellType.selected:
                tempColor = selectedCellColor;
                break;

            case CellInfo.CellType.branch:
                tempColor = branchCellColor;
                break;

            case CellInfo.CellType.path:
                tempColor = pathBranchCellColor;
                break;
            case CellInfo.CellType.anomaly:
                tempColor = anomalyCellColor;
                break;
            case CellInfo.CellType.unit:
            case CellInfo.CellType.none:
            default:
                tempColor = Color.clear;
                break;
        }

        if (!map.cells[cellInfo.cellId].visible)
            tempColor.a *= invisibleCellColorMultiplier;

        return tempColor;
    }
}
