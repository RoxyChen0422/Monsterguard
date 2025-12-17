using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Resources;
using Mono.Cecil;

[System.Serializable]
public class TowerSpriteMapping
{
    public TowerType towerType;
    public Sprite towerSprite;
}
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject gameHUD;
    public GameObject storePanel;
    public GameObject endGamePanel;
    public GameObject pausePanel;
    public GameObject Resource;

    [Header("HUD Elements")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI messageText;

    public TextMeshProUGUI messageText1;
    public Button nextWaveBtn;
    public Button pauseBtn;

    [Header("Store Elements")]
    public TowerSpriteMapping[] towerSprites;
    public Transform storeItemsContainer;
    public GameObject storeItemButtonPrefab;
    private Dictionary<TowerType, Sprite> spriteMap;

    private void Awake()
    {
        // Standard Singleton logic
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return; 
        }
        Instance = this;

        // Re-link the immortal GameManager to this NEW UI
        if (GameManager.Instance != null)
        {
            GameManager.Instance.uiManager = this;
            
            // IF we are restarting (State is Running), ensure panels are hidden
            if (GameManager.Instance.CurrentState == GameState.Running)
            {
                // Force hide the store and show the HUD
                storePanel.SetActive(false);
                pausePanel.SetActive(false);
                menuPanel.SetActive(false);
                gameHUD.SetActive(true);
                
                // Re-sync the numbers
                UpdateLivesUI(GameManager.Instance.Lives);
                UpdateResourceUI(StoreManager.Instance.CurrentCoins);
            }
        }

        // Your dictionary logic...
        spriteMap = new Dictionary<TowerType, Sprite>();
        foreach (var mapping in towerSprites)
        {
            spriteMap[mapping.towerType] = mapping.towerSprite; 
        }
    }

    public void ShowMenuScreen()
    {
        menuPanel.SetActive(true);
        gameHUD.SetActive(false);
        storePanel.SetActive(false);
        endGamePanel.SetActive(false);
        pausePanel.SetActive(false);
        Resource.SetActive(false);
    }

    public void ShowGameHUD()
    {
        menuPanel.SetActive(false);
        gameHUD.SetActive(true);
        Resource.SetActive(true);
        if (nextWaveBtn.IsActive()) nextWaveBtn.gameObject.SetActive(false);
        UpdateResourceUI(StoreManager.Instance.CurrentCoins);
    }

    public void UpdateResourceUI(int coins)
    {
        if (coinsText) coinsText.text = $"Coins: {coins}";
    }

    public void UpdateLivesUI(int lives)
    {
        if (livesText) livesText.text = $"Lives: {lives}";
    }

    public void UpdateWaveUI(int current, int total)
    {
        if (waveText) waveText.text = $"Wave: {current} / {total}";
    }

    public void ShowNextWaveButton(bool show)
    {
        if (nextWaveBtn) nextWaveBtn.gameObject.SetActive(show);
        if (pauseBtn && show) pauseBtn.enabled = false;
    }

    public void ShowMessage(string msg)
    {
        if (messageText)
        {
            messageText.text = msg;
            StopAllCoroutines();
            StartCoroutine(ClearMessage());
        }
        Debug.Log(msg);
    }

    private IEnumerator ClearMessage()
    {
        yield return new WaitForSeconds(2f);
        if (messageText) messageText.text = "";
    }

    // Store UI update
    public void ToggleStore(bool open)
    {
        if (open)
        {
            GameManager.Instance.PauseGame();//in pause
            endGamePanel.SetActive(false);
            storePanel.SetActive(open);
            StoreManager.Instance.GenerateStoreItems(); // Renew for every time open or wave end
            ShowMessage("");
        }
        else
        {
            storePanel.SetActive(open);
            GameManager.Instance.ResumeGame();
        }
    }

    public void UpdateStoreUI(List<UpgradeItem> items)
    {
        // clean old items
        foreach (Transform child in storeItemsContainer) Destroy(child.gameObject);

        // 
        for (int i = 0; i < items.Count; i++)
        {
            int index = i; 
            GameObject btnObj = Instantiate(storeItemButtonPrefab, storeItemsContainer);
            
            // 【关键步骤】获取 StoreSlotManager 引用
            ItemScript slotManager = btnObj.GetComponent<ItemScript>();

            // --- 赋值逻辑 ---
            string combinedText = $"Tower: {items[i].targetTower}\n" + $"{items[i].upgradeType} + "+$"{items[i].multiplier*100}%\n"+ $"Cost: ${items[i].cost}";
            slotManager.ItemText.text = combinedText;

            // B. 赋值图片
            TowerType currentTower = items[i].targetTower;
            // 确保 spriteMap 存在且包含该类型
            if ( spriteMap!= null && spriteMap.ContainsKey(currentTower)) 
            {
                slotManager.ItemIcon.sprite= spriteMap[currentTower];
            }
            else
            {
                Debug.LogError($"Missing sprite for {currentTower} or spriteMap not initialized!");
            }

            // C. 添加购买监听器 (保持不变)
            btnObj.GetComponentInChildren<Button>().onClick.AddListener(() => StoreManager.Instance.PurchaseStoreItem(index));
        }
    }
    /*public void UpdateStoreUI(List<UpgradeItem> items)
    {
        // Clean old button
        foreach (Transform child in storeItemsContainer) Destroy(child.gameObject);
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in storeItemsContainer)
        {
            toDestroy.Add(child.gameObject);
        }

        foreach (GameObject go in toDestroy)
        {
            if (go != null)
            {
                go.transform.SetParent(null);
                Destroy(go);
            }
        }

        for (int i = 0; i < items.Count; i++)
        {
            int index = i; // Closure capture
            GameObject btnObj = Instantiate(storeItemButtonPrefab, storeItemsContainer);
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{items[i].itemName}\n${items[i].cost}";
            btnObj.GetComponent<Button>().onClick.AddListener(() => StoreManager.Instance.PurchaseStoreItem(index));
        }
    }*/

    public void ShowEndGameScreen(bool victory)
    {
        endGamePanel.SetActive(true);
        TextMeshProUGUI title = endGamePanel.transform.Find("Title").GetComponent<TextMeshProUGUI>();
        if (title) title.text = victory ? "VICTORY!" : "DEFEAT";
    }

    public void TogglePauseMenu(bool show)
    {
        if (pausePanel) pausePanel.SetActive(show);
        if (show)
        { GameManager.Instance.PauseGame(); }
        else
        { 
            pausePanel.SetActive(false);
            //GameManager.Instance.ResumeGame(); 
            }
    }

    // Button callbacks

    public void OnStartGameEasy() => GameManager.Instance.StartGame(Difficulty.Easy);
    public void OnStartGameMedium() => GameManager.Instance.StartGame(Difficulty.Medium);
    public void OnStartGameHard() => GameManager.Instance.StartGame(Difficulty.Hard);

    public void OnBuildBasicTower() => GameManager.Instance.towerManager.SelectTowerToBuild(TowerType.Basic);
    public void OnBuildGatlingTower() => GameManager.Instance.towerManager.SelectTowerToBuild(TowerType.Gatling);
    public void OnBuildSniperTower() => GameManager.Instance.towerManager.SelectTowerToBuild(TowerType.Sniper);

    public void OnNextWaveClicked()
    {
        ShowNextWaveButton(false);
        GameManager.Instance.waveManager.StartNextWave();
    }

    public void OnOpenStoreClicked() => ToggleStore(true);
    public void OnCloseStoreClicked() => ToggleStore(false);

}
