using UnityEngine;

public class RocketShipController : MonoBehaviour
{
	public float speed = 10f;
	public float rotationSpeed = 100f;
	public GameObject projectilePrefab;
	public float projectileForce = 10f;

	private Transform projectileSpawnPoint;

	void Update()
	{
		Movement();

		if (Input.GetKeyDown(KeyCode.Space))
		{
			FireProjectile();
		}
	}
		
	/// <summary>
	/// 
	/// </summary>
	void FireProjectile()
	{
		GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
		Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
		projectileRb.AddForce(projectileSpawnPoint.up * projectileForce, ForceMode.Impulse);
	}

	/// <summary>
	/// 
	/// </summary>
	private void Movement()
	{
		// Spaceship movement
		float translation = Input.GetAxis("Vertical") * speed;
		float strafe = Input.GetAxis("Horizontal") * speed;
		translation *= Time.deltaTime;
		strafe *= Time.deltaTime;
		transform.Translate(strafe, translation, 0, Space.Self);
		projectileSpawnPoint = transform;

		// Spaceship rotation
		float rotation = 0f;
		if (Input.GetKey(KeyCode.Q))
		{
			rotation = -1f;
			rotation *= rotationSpeed * Time.deltaTime;
			transform.Rotate(0, 0, rotation);
		}
		else if (Input.GetKey(KeyCode.E))
		{
			rotation = 1f;
			rotation *= rotationSpeed * Time.deltaTime;
			transform.Rotate(0, 0, rotation);
		}
		else
		{
			transform.Rotate(0, 0, 0);
		}

	}
}
