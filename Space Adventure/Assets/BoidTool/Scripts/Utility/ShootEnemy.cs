using System.Collections;
using UnityEngine;

public interface IEnemyShooter
{
    void FireProjectile();
}

public class ShootEnemy : MonoBehaviour, IEnemyShooter
{
    public GameObject projectilePrefab;
    public float projectileForce = 2f;
    public float spawnDistance = 6f;
    public float delay = 1.0f;
    public bool isShooting = false;

    /// <summary>
    /// Method to fire a projectile.
    /// </summary>
    public void FireProjectile()
    {
        if (!isShooting)
        {
            StartCoroutine(DelayShooting());
        }
    }

    /// <summary>
    /// Coroutine to handle the delay between shots.
    /// </summary>
    private IEnumerator DelayShooting()
    {
        isShooting = true;
        CreateProjectile();
        yield return new WaitForSeconds(delay);
        isShooting = false;
    }

    /// <summary>
    /// Method to create and launch a projectile.
    /// </summary>
    private void CreateProjectile()
    {
        Vector3 spawnPosition = transform.position + (transform.up * spawnDistance);
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, transform.rotation);
        
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectileRb.AddForce(transform.up * projectileForce, ForceMode.Impulse);
        Destroy(projectile, 1f);
    }
}
