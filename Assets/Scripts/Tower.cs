using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Tower : MonoBehaviour
{
    [Header("Visuals (4 Directions)")]
    public SpriteRenderer towerRenderer;
    public Sprite frontLeft;  // 敌人在左下 (Enemy X < Tower X, Enemy Y < Tower Y)
    public Sprite frontRight; // 敌人在右下 (Enemy X > Tower X, Enemy Y < Tower Y)
    public Sprite backLeft;   // 敌人在左上 (Enemy X < Tower X, Enemy Y > Tower Y)
    public Sprite backRight;  // 敌人在右上 (Enemy X > Tower X, Enemy Y > Tower Y)
    [Header("Base Attributes")]
    public int id;
    public TowerType type;
    public float baseDamage;
    public float baseRange;
    public float baseFireRate;
    public int buildCost;
    public float setupTime = 3.0f;

    protected float currentDamage;
    protected float currentRange;
    protected float currentFireRate;

    [Header("Runtime")]
    public GameObject projectilePrefab;
    protected bool isOperational = false;
    protected Enemy target;
    protected float fireTimer = 0f;

    public SpriteRenderer rangeIndicator;
    public GameObject constructionUI; // process bar or icon when building

    public virtual void Initialize(int id)
    {
        this.id = id;
        ApplyGlobalUpgrades();
        OnBuild();
    }

    public void ApplyGlobalUpgrades()
    {
        float dmgMult = StoreManager.Instance.GetUpgradeMultiplier(type, UpgradeType.Damage);
        float rngMult = StoreManager.Instance.GetUpgradeMultiplier(type, UpgradeType.Range);
        float spdMult = StoreManager.Instance.GetUpgradeMultiplier(type, UpgradeType.FireRate);

        currentDamage = baseDamage * dmgMult;
        currentRange = baseRange * rngMult;
        currentFireRate = baseFireRate * spdMult;
    }

    public virtual void OnBuild()
    {
        StartCoroutine(BuildProcess());
    }

    private IEnumerator BuildProcess()
    {
        if (constructionUI) constructionUI.SetActive(true);
        float timer = 0;
        while (timer < setupTime)
        {
            timer += Time.deltaTime;
            // Can update process bar UI here
            yield return null;
        }

        if (constructionUI) constructionUI.SetActive(false);
        isOperational = true;
    }

    protected virtual void Update()
    {
        if (!isOperational || GameManager.Instance.CurrentState != GameState.Running) return;

        AcquireTarget();
        UpdateVisualDirection();
        fireTimer -= Time.deltaTime;
        if (target != null && fireTimer <= 0)
        {
            Fire();
            fireTimer = 1f / currentFireRate;
        }
    }
    protected void UpdateVisualDirection()
    {
        if (target == null || towerRenderer == null) return;

        Vector3 enemyPos = target.transform.position;
        Vector3 towerPos = transform.position;

        // 核心逻辑：对比坐标
        if (enemyPos.y >= towerPos.y) // 敌人在上方区域
        {
            if (enemyPos.x >= towerPos.x)
                towerRenderer.sprite = backRight; // 右上 (你说的 x,y 都更大)
            else
                towerRenderer.sprite = backLeft;  // 左上
        }
        else // 敌人在下方区域
        {
            if (enemyPos.x >= towerPos.x)
                towerRenderer.sprite = frontRight; // 右下
            else
                towerRenderer.sprite = frontLeft;  // 左下
        }
    }

    protected virtual void AcquireTarget()
    {
        // Find closest enemy
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentRange);
        float closestDist = float.MaxValue;
        Enemy bestCandidate = null;

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    bestCandidate = enemy;
                }
            }
        }
        target = bestCandidate;
    }

    protected virtual void Fire()
    {
        if (projectilePrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0, 0.5f, 0);
            GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            Projectile proj = projObj.GetComponent<Projectile>();
            if (proj) proj.Initialize(target, currentDamage);
        }
        else
        {
        target.TakeDamage(currentDamage);
        }
    }

    // Draw attack range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, currentRange > 0 ? currentRange : baseRange);
    }
}
