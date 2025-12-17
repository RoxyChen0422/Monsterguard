using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class UpgradeItem
{
    public string itemName;
    public TowerType targetTower;
    public UpgradeType upgradeType;
    public float multiplier;
    public int cost;
}

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance;
    public int CurrentCoins = 100; // Startup coin

    // Key: "TowerType_UpgradeType"
    private Dictionary<string, float> upgrades = new Dictionary<string, float>();

    public List<UpgradeItem> itemsForSale = new List<UpgradeItem>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        Instance = this;
    }

    public void Initialize()
    {
        upgrades.Clear();
        foreach (TowerType t in Enum.GetValues(typeof(TowerType)))
        {
            foreach (UpgradeType u in Enum.GetValues(typeof(UpgradeType)))
            {
                upgrades[$"{t}_{u}"] = 1.0f;
            }
        }
        GenerateStoreItems();
    }

    public float GetUpgradeMultiplier(TowerType t, UpgradeType u)
    {
        string key = $"{t}_{u}";
        return upgrades.ContainsKey(key) ? upgrades[key] : 1.0f;
    }

    public bool HasEnoughCoins(int amount)
    {
        return CurrentCoins >= amount;
    }

    public void SpendCoins(int amount)
    {
        CurrentCoins -= amount;
        UIManager.Instance.UpdateResourceUI(CurrentCoins);
    }

    public void AddCoins(int amount)
    {
        CurrentCoins += amount;
        UIManager.Instance.UpdateResourceUI(CurrentCoins);
    }
    public void GenerateStoreItems()
    {
        float[] possibleMultipliers = new float[] { 0.1f, 0.3f, 0.5f, 0.7f, 0.9f };
        itemsForSale.Clear();
        int i = 0;
        while (i < 3)
        {
            int randomIndex = UnityEngine.Random.Range(0, possibleMultipliers.Length);
            UpgradeItem item = new UpgradeItem();
            item.targetTower = (TowerType)UnityEngine.Random.Range(0, 3);
            item.upgradeType = (UpgradeType)UnityEngine.Random.Range(0, 3);
            item.multiplier = possibleMultipliers[randomIndex];
            item.cost = 50 + (int)(item.multiplier * 1000);
            item.itemName = $"{item.targetTower} {item.upgradeType} +{item.multiplier * 100}%";
            if( itemsForSale.Exists(x => x.itemName == item.itemName)) continue;
            i++;
            itemsForSale.Add(item);
        }
        UIManager.Instance.UpdateStoreUI(itemsForSale);
    }

    public void PurchaseStoreItem(int index)
    {
        if (index >= itemsForSale.Count) return;
        UpgradeItem item = itemsForSale[index];

        if (HasEnoughCoins(item.cost))
        {
            SpendCoins(item.cost);
            ApplyPermanentUpgrade(item.targetTower, item.upgradeType, item.multiplier);
            itemsForSale.RemoveAt(index);
            UIManager.Instance.UpdateStoreUI(itemsForSale);
            UIManager.Instance.ShowMessage($"Purchased {item.itemName}!");
        }
        else
        {
            UIManager.Instance.ShowMessage("Not enough coins!");
        }
    }

    private void ApplyPermanentUpgrade(TowerType t, UpgradeType u, float amount)
    {
        string key = $"{t}_{u}";
        if (upgrades.ContainsKey(key))
        {
            upgrades[key] += amount;
        }
        GameManager.Instance.towerManager.RefreshAllTowers();
    }
}
