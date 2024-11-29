using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float distance;
    public int damage;
    public LayerMask whatIsSolid;

    public GameObject destroyEffect;

    // Set to track damaged enemies
    private HashSet<Collider2D> damagedEnemies = new HashSet<Collider2D>();

    private void Start()
    {
        // Ensure the projectile destroys itself after its lifetime
        StartCoroutine(DestroyAfterLifetime());
    }

    private void Update()
    {
        // Check for collisions with Raycast
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, distance, whatIsSolid);
        if (hitInfo.collider != null)
        {
            // If it's an enemy and hasn't been damaged yet
            if (hitInfo.collider.CompareTag("Enemy") && !damagedEnemies.Contains(hitInfo.collider))
            {
                damagedEnemies.Add(hitInfo.collider); // Mark enemy as damaged
                hitInfo.collider.GetComponent<HealthController>().TakeDamage(damage);
            }
        }

        // Move the projectile
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private IEnumerator DestroyAfterLifetime()
    {
        // Wait for the specified lifetime duration
        yield return new WaitForSeconds(lifeTime);
        DestroyProjectile();
    }

    void DestroyProjectile()
    {
        // Instantiate the destroy effect if available
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        // Destroy the projectile object
        Destroy(gameObject);
    }
}
