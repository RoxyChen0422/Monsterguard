using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Wave
{
    public int waveIndex;
    public EnemyType enemyType;
    public int count;
    public float spawnInterval;
}

public class WaveManager : MonoBehaviour
{
    [SerializeField] private List<Wave> waves;
    private int currentWaveIndex = 0;
    private bool isWaveInProgress = false;

    private void Awake()
    {
        // 每次场景加载，新的 WaveManager 都会把自己注册给 GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.waveManager = this;
        }
    }
    public EnemyManager enemyManager;

    public void InitializeWaves(Difficulty difficulty)
    {
        waves = new List<Wave>();
        int waveCount = difficulty == Difficulty.Easy ? 5 : (difficulty == Difficulty.Medium ? 10 : 20);

        for (int i = 0; i < waveCount; i++)
        {
            EnemyType type = EnemyType.Type1;
            if (i > 3) type = EnemyType.Type2;
            if (i > 7) type = EnemyType.Type3;

            waves.Add(new Wave
            {
                waveIndex = i + 1,
                enemyType = type,
                count = 5 + (i * 2),
                spawnInterval = Mathf.Max(0.5f, 1.5f - (i * 0.1f))
            });
        }
        currentWaveIndex = 0;
        UIManager.Instance.UpdateWaveUI(0, waveCount);
    }

    public void StartNextWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            StartCoroutine(WaitForVictory());
            return;
        }

        StartCoroutine(SpawnWaveRoutine(waves[currentWaveIndex]));
        UIManager.Instance.UpdateWaveUI(currentWaveIndex + 1, waves.Count);
        currentWaveIndex++;
    }

    private IEnumerator SpawnWaveRoutine(Wave wave)
    {
        isWaveInProgress = true;
        Debug.Log($"Spawn interval: {wave.spawnInterval}");
        for (int i = 0; i < wave.count; i++)
        {
            if (GameManager.Instance.CurrentState != GameState.Running) yield break;
            enemyManager.SpawnEnemy(wave.enemyType);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
        yield return new WaitUntil(() => enemyManager.activeEnemies.Count == 0);

        isWaveInProgress = false;
        UIManager.Instance.ShowMessage($"Wave {wave.waveIndex} Complete!");

        // Automatically start next wave
        // StartNextWave(); 
        // Need player tap the button to start next wave
        UIManager.Instance.ShowNextWaveButton(true);
    }

    private IEnumerator WaitForVictory()
    {
        yield return new WaitUntil(() => enemyManager.activeEnemies.Count == 0);
        GameManager.Instance.EndGame(true);
    }

    public void CleanAllEnemies()
    {
        enemyManager.ClearAllEnemies();
    }
}
