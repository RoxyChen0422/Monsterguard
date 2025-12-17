using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("引用")]
    public Animator animator; 

    // 这里不再存 Sprite，而是存 AnimationClip
    public AnimationClip ani1;
    public AnimationClip ani2;
    public AnimationClip anitype3;

    [Header("Runtime Attributes")]
    public float maxHp;
    public float currentHp;
    public float speed;
    public int coinReward;

    private int currentWaypointIndex = 0;
    private List<Vector2> path;

    public Transform hpBarFill;

    public void Initialize(List<Vector2> waypoints, EnemyType type)
    {
        path = waypoints;
        switch (type)
        {
            case EnemyType.Type1: maxHp = 10; speed = 1f; coinReward = 1; animator.Play(ani1.name);break;
            case EnemyType.Type2: maxHp = 10; speed = 1.5f; coinReward = 3; animator.Play(ani2.name);break;
            case EnemyType.Type3: maxHp = 20; speed = 2.5f; coinReward = 5; animator.Play(anitype3.name);break;
            default: maxHp = 10; speed = 1f; coinReward = 1; break;
        }
        currentHp = maxHp;
        transform.position = path[0];
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState == GameState.Running)
            Move();
    }

    private void Move()
    {
        if (path == null || currentWaypointIndex >= path.Count) return;

        Vector2 target = path[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= path.Count)
            {
                ReachEndPoint();
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHp -= amount;
        UpdateHPBar();
        if (currentHp <= 0)
        {
            OnDeath();
        }
    }

    private void UpdateHPBar()
    {
        if (hpBarFill)
        {
            float pct = Mathf.Clamp01(currentHp / maxHp);
            hpBarFill.localScale = new Vector3(1.5f * pct, 0.3f, 1);
        }
    }

    private void ReachEndPoint()
    {
        GameManager.Instance.ReduceLives(1);
        GameManager.Instance.waveManager.enemyManager.UnregisterEnemy(this);
        Destroy(gameObject);
    }

    private void OnDeath()
    {
        StoreManager.Instance.AddCoins(coinReward);
        if (UnityEngine.Random.value < 0.1f) // 10% drop
        {
            Debug.Log("Dropped an upgrade tool!");
            // Can instantiate an clickable item object here
        }

        GameManager.Instance.waveManager.enemyManager.UnregisterEnemy(this);
        Destroy(gameObject);
    }
}