using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject gameHUD;
    public GameObject storePanel;
    public GameObject endGamePanel;
    public GameObject pausePanel;

    [Header("HUD Elements")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI messageText;
    public Button nextWaveBtn;

    [Header("Store Elements")]
    public Transform storeItemsContainer;
    public GameObject storeItemButtonPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowMenuScreen()
    {
        menuPanel.SetActive(true);
        gameHUD.SetActive(false);
        storePanel.SetActive(false);
        endGamePanel.SetActive(false);
    }

    public void ShowGameHUD()
    {
        menuPanel.SetActive(false);
        gameHUD.SetActive(true);
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
            GameManager.Instance.PauseGame();
            storePanel.SetActive(open);
            StoreManager.Instance.GenerateStoreItems(); // Renew for every time open or wave end
        }
        else
        {
            storePanel.SetActive(open);
            GameManager.Instance.ResumeGame();
        }
    }

    public void UpdateStoreUI(List<UpgradeItem> items)
    {
        // Clean old button
        // foreach (Transform child in storeItemsContainer) Destroy(child.gameObject);
        /*List<GameObject> toDestroy = new List<GameObject>();
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
        }*/

        for (int i = 0; i < items.Count; i++)
        {
            int index = i; // Closure capture
            GameObject btnObj = Instantiate(storeItemButtonPrefab, storeItemsContainer);
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{items[i].itemName}\n${items[i].cost}";
            btnObj.GetComponent<Button>().onClick.AddListener(() => StoreManager.Instance.PurchaseStoreItem(index));
        }
    }

    public void ShowEndGameScreen(bool victory)
    {
        endGamePanel.SetActive(true);
        TextMeshProUGUI title = endGamePanel.transform.Find("Title").GetComponent<TextMeshProUGUI>();
        if (title) title.text = victory ? "VICTORY!" : "DEFEAT";
    }

    public void TogglePauseMenu(bool show)
    {
        if (pausePanel) pausePanel.SetActive(show);
        if (show) GameManager.Instance.PauseGame();
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

    public void OnRestartClicked() => GameManager.Instance.RestartGame();
}
