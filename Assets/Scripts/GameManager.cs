using UnityEngine;

public enum GameState
{
    Menu,
    Running,
    Paused,
    GameOver,
    Victory
}

public enum Difficulty
{
    Easy,   // 5 Waves
    Medium, // 10 Waves
    Hard    // 20 Waves
}

public enum TowerType
{
    Basic,
    Gatling,
    Archer,
    Bomb,
    Sniper
}

public enum EnemyType
{
    Type1, // Low HP, Low Speed
    Type2, // Med HP, Low Speed
    Type3, // Med HP, Med Speed
    Type4, // Med HP, High Speed
    Type5  // High HP, Med Speed
}

public enum UpgradeType
{
    Damage,
    Range,
    FireRate
}

public class GameManager : MonoBehaviour
{
    private Difficulty currentDifficulty;
    public static GameManager Instance { get; private set; }

    [Header("Game Status")]
    public GameState CurrentState;
    public Difficulty SelectedDifficulty;
    public float GameSpeed = 1.0f;

    [Header("Player Stats")]
    public int Lives = 20;

    [Header("Module References")]
    public WaveManager waveManager;
    public UIManager uiManager;
    public StoreManager storeManager;
    public TowerManager towerManager;
    public MapManager mapManager;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CurrentState = GameState.Menu;
        uiManager.ShowMenuScreen();
    }

    public void StartGame(Difficulty difficulty)
    {
        SelectedDifficulty = difficulty;
        CurrentState = GameState.Running;
        Time.timeScale = GameSpeed;
        storeManager.Initialize();
        waveManager.InitializeWaves(difficulty);
        uiManager.ShowGameHUD();
        uiManager.UpdateLivesUI(Lives);

        waveManager.StartNextWave();

        Debug.Log($"Game Started: {difficulty} Mode");
    }

    public void PauseGame()
    {
        CurrentState = GameState.Paused;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        CurrentState = GameState.Running;
        Time.timeScale = GameSpeed;
        uiManager.TogglePauseMenu(false);
    }

    public void ReduceLives(int amount)
    {
        Lives -= amount;
        uiManager.UpdateLivesUI(Lives);
        if (Lives <= 0)
        {
            EndGame(false);//show defeat
        }
    }

    public void EndGame(bool victory)
    {
        CurrentState = victory ? GameState.Victory : GameState.GameOver;
        Time.timeScale = 0;
        uiManager.ShowEndGameScreen(victory);
    }

    public void AdjustGameSpeed(float speed)
    {
        GameSpeed = speed;
        if (CurrentState == GameState.Running)
        {
            Time.timeScale = GameSpeed;
        }
    }
}
