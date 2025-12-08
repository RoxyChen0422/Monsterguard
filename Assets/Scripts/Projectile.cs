using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy target;
    private float damage;
    private float speed = 10f;

    public void Initialize(Enemy target, float damage)
    {
        this.target = target;
        this.damage = damage;
        Destroy(gameObject, 5f); // destroy after 5 seconds
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 dir = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
        //transform.Translate(dir * speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.transform.position) < 0.2f)
        {
            target.TakeDamage(damage);
            Destroy(gameObject); // destroy when hitted
        }
    }
}