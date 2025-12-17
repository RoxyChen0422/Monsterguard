using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public Dictionary<int, Tower> activeTowers = new Dictionary<int, Tower>();
    public MapManager mapManager;

    [Header("Prefabs")]
    public GameObject basicTowerPrefab;
    public GameObject gatlingTowerPrefab;
    public GameObject sniperTowerPrefab;

    private TowerType selectedTypeToBuild;
    private bool isBuildingMode = false;

    private void Update()
    {
        // Handle mouse cick to build tower
        if (isBuildingMode && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TryBuildTower(mousePos);
        }

        // Cancel build
        if (Input.GetMouseButtonDown(1))
        {
            isBuildingMode = false;
        }
    }

    public void SelectTowerToBuild(TowerType type)
    {
        selectedTypeToBuild = type;
        isBuildingMode = true;
        UIManager.Instance.ShowMessage($"Selected {type} to build. Click on map.");
    }

    public void TryBuildTower(Vector2 rawPos)
    {
        // Align position with cell
        Vector2 gridPos = new Vector2(Mathf.Round(rawPos.x), Mathf.Round(rawPos.y));

        if (!mapManager.IsCellAvailable(gridPos))
        {
            UIManager.Instance.ShowMessage("Invalid Position!");
            return;
        }

        int cost = GetTowerCost(selectedTypeToBuild);
        if (!StoreManager.Instance.HasEnoughCoins(cost))
        {
            UIManager.Instance.ShowMessage("Not Enough Coins!");
            return;
        }

        StoreManager.Instance.SpendCoins(cost);

        GameObject prefab = GetPrefabByType(selectedTypeToBuild);
        GameObject towerObj = Instantiate(prefab, gridPos, Quaternion.identity);
        Tower newTower = towerObj.GetComponent<Tower>();

        int newId = activeTowers.Count + 1;
        newTower.Initialize(newId);

        activeTowers.Add(newId, newTower);
        mapManager.MarkCellOccupied(gridPos);

        isBuildingMode = false; // Quit build mode
    }

    public void RefreshAllTowers()
    {
        foreach (var tower in activeTowers.Values)
        {
            tower.ApplyGlobalUpgrades();
        }
    }

    public void ClearAllTowers()
    {
        foreach (var tower in activeTowers.Values)
        {
            Destroy(tower.gameObject);
            Destroy(tower);
        }
        activeTowers.Clear();
    }

    private GameObject GetPrefabByType(TowerType type)
    {
        switch (type)
        {
            case TowerType.Gatling: return gatlingTowerPrefab;
            case TowerType.Sniper: return sniperTowerPrefab;
            default: return basicTowerPrefab;
        }
    }

    private int GetTowerCost(TowerType type)
    {
        // Should be set as same as tower prefab
        switch (type)
        {
            case TowerType.Basic: return 10;
            case TowerType.Gatling: return 50;
            case TowerType.Sniper: return 100;
            default: return 10;
        }
    }
}
