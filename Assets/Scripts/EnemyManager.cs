using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public PathingService pathingService;
    public List<Enemy> activeEnemies = new List<Enemy>();

    public void SpawnEnemy(EnemyType type)
    {
        GameObject enemyObj = Instantiate(enemyPrefab);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.Initialize(pathingService.GetPath(), type);
        activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy e)
    {
        if (activeEnemies.Contains(e)) activeEnemies.Remove(e);
    }
}
