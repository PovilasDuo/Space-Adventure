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
    public float spawnDistance = 1.5f;
    public float delay = 1.0f;
    public bool isShooting = false;

    public void FireProjectile()
    {
        if (!isShooting)
        {
            StartCoroutine(DelayShooting());
        }
    }

    private IEnumerator DelayShooting()
    {
        isShooting = true;
        CreateProjectile();
        yield return new WaitForSeconds(delay);
        isShooting = false;
    }

    private void CreateProjectile()
    {
        Vector3 spawnPosition = transform.position + (transform.up * spawnDistance);
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, transform.rotation);
        
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectileRb.AddForce(transform.up * projectileForce, ForceMode.Impulse);
        Destroy(projectile, 1f);
    }
}
