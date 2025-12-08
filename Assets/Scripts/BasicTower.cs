using UnityEngine;

public class BasicTower : Tower
{
    protected override void Fire()
    {
        if (projectilePrefab != null)
        {
            GameObject projObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile proj = projObj.GetComponent<Projectile>();
            if (proj) proj.Initialize(target, currentDamage);
        }
        else
        {
            target.TakeDamage(currentDamage);
        }
    }
}
