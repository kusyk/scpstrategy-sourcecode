using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPM;

public class StickersManager : MonoBehaviour
{
    public static StickersManager Instance;

    public GameObject branchStickerPrefab;
    public GameObject unitStickerPrefab;


    [Space]
    public List<BranchSticker> branchStickers = new List<BranchSticker>();
    public List<UnitSticker> unitStickers = new List<UnitSticker>();

    private WorldMapGlobe map;
     
    void Start()
    {
        Instance = this;
        map = WorldMapGlobe.instance;
    }

    /// <summary> Use <c>SpawnSticker</c> only when branch/unit has been added!</summary>
    public void SpawnSticker(int cellId, StickerType stickerType, string header, int itemIndex)
    {
        Vector2 latLon = map.cells[cellId].latlonCenter;
        GameObject tempStickerObject = null;
        switch (stickerType)
        {
            case StickerType.Branch:
                BranchSticker tempBranchSticker = Instantiate(branchStickerPrefab).GetComponent<BranchSticker>();
                tempBranchSticker.SetupSticker(itemIndex, header);
                branchStickers.Add(tempBranchSticker);
                tempStickerObject = tempBranchSticker.gameObject;
                break;
            case StickerType.Unit:
                UnitSticker tempUnitSticker = Instantiate(unitStickerPrefab).GetComponent<UnitSticker>();
                tempUnitSticker.SetupSticker(itemIndex, header);
                unitStickers.Add(tempUnitSticker);
                tempStickerObject = tempUnitSticker.gameObject;
                break;
        }

        if (tempStickerObject == null)
            return;

        map.AddMarker(tempStickerObject, Conversion.GetSpherePointFromLatLon(latLon), 0.0002f, true, 0.1f);
    }

    public void RefreshStickerHolo(int branchIndex)
    {
        branchStickers[branchIndex].RefreshHoloSize();
    }

    public void UpdateUnitPositionTest(int unitId ,Vector2 newLatLon)
    {
        unitStickers[unitId].DestroyMe();
        UnitSticker tempUnitSticker = Instantiate(unitStickerPrefab).GetComponent<UnitSticker>();
        tempUnitSticker.SetupSticker(unitId, PlayerController.PlayerInfo.units[unitId].unitName);
        unitStickers[unitId] = tempUnitSticker;
        tempUnitSticker.FlyToMe();
        map.AddMarker(tempUnitSticker.gameObject, Conversion.GetSpherePointFromLatLon(newLatLon), 0.0002f, true, 0.1f);
    }

    public void UpdateBranchName(int branchIndex, string name)
    {
        branchStickers[branchIndex].SetupSticker(branchIndex, name);
    }

    public void UpdateUnitName(int unithIndex, string name)
    {
        unitStickers[unithIndex].SetupSticker(unithIndex, name);
    }

    public enum StickerType
    {
        Branch,
        Unit
    }
}
