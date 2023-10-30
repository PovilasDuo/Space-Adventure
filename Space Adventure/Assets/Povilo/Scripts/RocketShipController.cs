using UnityEngine;

public class RocketShipController : MonoBehaviour
{
	public float speed = 10f;
	public float rotationSpeed = 100f;

	void Update()
	{
		// Spaceship movement
		float translation = Input.GetAxis("Vertical") * speed;
		float strafe = Input.GetAxis("Horizontal") * speed;
		translation *= Time.deltaTime;
		strafe *= Time.deltaTime;
		transform.Translate(strafe, 0, translation);

		// Spaceship rotation
		float rotation = 0f;
		if (Input.GetKey(KeyCode.Q))
		{
			rotation = -1f;
		}
		else if (Input.GetKey(KeyCode.E))
		{
			rotation = 1f;
		}
		rotation *= rotationSpeed * Time.deltaTime;
		transform.Rotate(0, rotation, 0);
	}
	/*	 Firing projectile
		if (Input.GetKeyDown(KeyCode.Space))
		{
			FireProjectile();
		}

	void FireProjectile()
	{
		GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
		Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
		projectileRb.AddForce(projectileSpawnPoint.up * projectileForce, ForceMode2D.Impulse);
	}*/
}
