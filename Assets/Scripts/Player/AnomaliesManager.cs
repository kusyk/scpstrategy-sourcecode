using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPM;

public class AnomaliesManager : MonoBehaviour
{
    public static AnomaliesManager instance;
    private WorldMapGlobe map;

    [Header("Settings")]
    public int diversityLevel = 3;
    public int standartSize = 3;
    public int searchingDistance = 5;
    
    [Space]
    public List<AnomalyInfo> anomalies = new List<AnomalyInfo>();

    private bool GetRandomTrueChance()
    {
        float fakeChance = (1f - PlayerController.PlayerInfo.satisfactionLevel) * 0.35f + 0.35f;
        float random = Random.Range(0f, 1f);

        if (PlayerController.PlayerInfo.capturedObjects > 95)
            random = 0f;

        if (random < fakeChance)
            return false;

        return true;
    }

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        map = WorldMapGlobe.instance;
    }

    public int ActivatedAnomaliesCount
    {
        get
        {
            int count = 0;

            for (int i = 0; i < anomalies.Count; i++)
            {
                if (anomalies[i].active)
                    count++;
            }

            return count;
        }
    }

    public void SpawnNewAnomaly(bool spawnNotification = true)
    {
        int randomAnomalyCellIndex = 0;
        bool meetsConditions;
        int debug = 0;

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        do
        {
            debug++;
            meetsConditions = true;
            randomAnomalyCellIndex = Random.Range(0, map.cells.Length);

            if(map.cells[randomAnomalyCellIndex].visible == false)
            {
                meetsConditions = false;
                continue;
            }

            for (int i = 0; i < PlayerController.PlayerInfo.branches.Count; i++)
            {
                List<int> tempPath = new List<int>();
                tempPath = map.FindPath(PlayerController.PlayerInfo.branches[i].cellId, randomAnomalyCellIndex, 20, true, WPM.PathFinding.HiddenCellsFilterMode.UseAllCells);

                if(tempPath != null)
                {
                    meetsConditions = false;
                    break;
                }
            }
        }
        while (meetsConditions == false);

        SpawnNewAnomaly(randomAnomalyCellIndex, spawnNotification);

        stopwatch.Stop();

        Debug.Log("Anomaly found in " + stopwatch.Elapsed.Milliseconds + "ms. Loop done " + debug + " times.");
    }

    public void SpawnNewAnomaly(int cellIndex, bool spawnNotification = true)
    {
        AnomalyInfo anomaly = new AnomalyInfo(cellIndex, standartSize);

        List<int> nearCells = new List<int>();
        List<int> lastAdded = new List<int>();
        int[] neighbours = map.GetCellNeighboursIndices(cellIndex);

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (map.cells[neighbours[i]].visible)
            {
                nearCells.Add(neighbours[i]);
                lastAdded.Add(neighbours[i]);
            }
        }

        for (int i = 0; i < anomaly.size; i++)
        {
            List<int> nextCells = new List<int>();
            int tempDiversity = diversityLevel;

            if (tempDiversity >= lastAdded.Count)
            {
                tempDiversity = lastAdded.Count - 1;
            }

            do
            {
                int tempIndex;
                do
                {
                    tempIndex = Random.Range(0, lastAdded.Count);
                }
                while (nextCells.Contains(tempIndex));
                nextCells.Add(tempIndex);
            }
            while (nextCells.Count < tempDiversity);

            List<int> tempLastAdded = new List<int>();

            for (int j = 0; j < nextCells.Count; j++)
            {
                neighbours = map.GetCellNeighboursIndices(lastAdded[nextCells[j]]);

                for (int k = 0; k < neighbours.Length; k++)
                {
                    if (map.cells[neighbours[k]].visible && !nearCells.Contains(neighbours[k]))
                    {
                        nearCells.Add(neighbours[k]);
                        tempLastAdded.Add(neighbours[k]);
                    }
                }
            }

            lastAdded = tempLastAdded;
        }

        anomaly.cells.AddRange(nearCells);

        if (PlayerController.PlayerInfo.checkedCases < 10 || GetRandomTrueChance())
        {
            anomaly.cellWithObject = anomaly.cells[Random.Range(0, anomaly.cells.Count)];

            ObjectsManager.ObjectData anomalyObject = GetUndetectedObject();

            if (anomalyObject == null)
            {
                Debug.LogWarning("No undetected anomalies.");
                return;
            }

            anomaly.objectId = anomalyObject.json.id;
        }
        else
        {
            anomaly.cellWithObject = -1;

            anomaly.objectId = "fake";
        }

        anomalies.Add(anomaly);

        if (spawnNotification)
        {
            Debug.Log("spawning single notification is depreciated");
            //Notification tempNotification = NotificationsManager.Instance.SpawnNotification("NEW ANOMALY", anomalyObject.json.alert + "\n\n<u>click here to find it on the globe</u>",
            //    true, -1, anomalies.Count - 1);

            //tempNotification.OnClickEventInt += FlyToAnomaly;
            //tempNotification.OnClickEvent += tempNotification.DestroyNotification;
        }

        ColorsManager.Instance.RecolorWholeGlobe();
    }

    public void FlyToAnomaly(int anomalyIndex)
    {
        map.FlyToCell(anomalies[anomalyIndex].cells[0], 0.5f, 0.4f);
    }

    public bool FinishSearching(int cellIndex)
    {
        bool searchingStatus = false;

        List<int> checkedCells = new List<int>();
        checkedCells.Add(cellIndex);

        int optimizer = 0;

        for (int x = 0; x < searchingDistance; x++)
        {
            List<int> cellsToAdd = new List<int>();

            for (int i = optimizer; i < checkedCells.Count; i++)
            {
                int[] neighbours = map.GetCellNeighboursIndices(checkedCells[i]);

                for (int j = 0; j < neighbours.Length; j++)
                {
                    if(checkedCells.Contains(neighbours[j]) == false)
                    {
                        cellsToAdd.Add(neighbours[j]);
                    }
                }
            }

            optimizer = checkedCells.Count;

            for (int i = 0; i < cellsToAdd.Count; i++)
            {
                checkedCells.Add(cellsToAdd[i]);
            }
        }

        //debug coloring
        //for (int i = 0; i < checkedCells.Count; i++)
        //{
        //    map.SetCellColor(checkedCells[i], Color.red);
        //}

        for (int anomaly = 0; anomaly < anomalies.Count; anomaly++)
        {
            if (anomalies[anomaly].active == false)
                continue;

            if (checkedCells.Contains(anomalies[anomaly].cellWithObject))
            {
                FindAnomaly(anomaly);
                anomalies[anomaly].cells.Clear();
                searchingStatus = true;
                continue;
            }

            for (int checkedCell = 0; checkedCell < checkedCells.Count; checkedCell++)
            {
                anomalies[anomaly].cells.Remove(checkedCells[checkedCell]);
            }

            if(anomalies[anomaly].cells.Count == 0)
            {
                PlayerController.PlayerInfo.checkedCases++;
                anomalies[anomaly].active = false;
            }
        }

        ColorsManager.Instance.RecolorWholeGlobe();

        return searchingStatus;
    }

    private void FindAnomaly(int anomalyIndex)
    {
        if (PlayerController.PlayerInfo.objects == null)
        {
            PlayerController.PlayerInfo.objects = new List<ObjectInfo>();
        }

        PlayerController.PlayerInfo.objects.Add(new ObjectInfo(anomalies[anomalyIndex].objectId, "Object #" + (PlayerController.PlayerInfo.objects.Count + 1),
            map.cells[anomalies[anomalyIndex].cellWithObject].latlonCenter));

        PlayerController.PlayerInfo.checkedCases++;
        PlayerController.PlayerInfo.capturedObjects++;
        PlayerController.PlayerInfo.score += 13;

        anomalies[anomalyIndex].active = false;
    }

    private ObjectsManager.ObjectData GetUndetectedObject()
    {
        List<string> allIds = new List<string>();

        for (int i = 0; i < ObjectsManager.Instance.objects.Count; i++)
        {
            string tempObject = ObjectsManager.Instance.objects[i].json.id;

            if(CheckAnomalyExistance(tempObject) == false && CheckAnomalyContainment(tempObject) == false)
            {
                allIds.Add(tempObject);
            }
        }

        if (allIds.Count == 0)
        {
            return null;
        }

        return ObjectsManager.GetObjectDataById(allIds[Random.Range(0, allIds.Count)]);
    }

    private bool CheckAnomalyExistance(string objectId)
    {
        for (int i = 0; i < anomalies.Count; i++)
        {
            if (anomalies[i].objectId == objectId && anomalies[i].active == true)
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckAnomalyContainment(string objectId)
    {
        for (int i = 0; i < PlayerController.PlayerInfo.objects.Count; i++)
        {
            if (PlayerController.PlayerInfo.objects[i].id == objectId)
            {
                return true;
            }
        }

        return false;
    }
}
