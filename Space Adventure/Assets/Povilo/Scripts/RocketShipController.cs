using UnityEngine;

public class RocketShipController : MonoBehaviour
{
	public int health = 10;
	public float speed = 6f;
	public float rotationSpeed = 80f;
	public GameObject projectilePrefab;
	public Transform projectileSpawnPoint;
	public float projectileForce = 2f;

	private bool moving;
	private float rotation;

	void Update()
	{
		Movement();
		if (Input.GetKeyDown(KeyCode.Space))
		{
			FireProjectile();
		}
	}
		
	/// <summary>
	/// Projectile fire script
	/// Projectile is instantiated based on the projectileSpawnPoint location and rotation
	/// Then forward force is added to it based on the projectileForce
	/// </summary>
	void FireProjectile()
	{
		GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
		Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
		projectileRb.AddForce(projectileSpawnPoint.up * projectileForce, ForceMode.Impulse);
	}

	/// <summary>
	/// Simple movement
	/// Spaceship moves with WASD or Arrow keys
	/// IMPORTANT: Spaceship can not rotate (A, D) if it is not moving
	/// If it is going backwards, the direction of (A, D) is reversed so it would feel more intuitive
	/// </summary>
	private void Movement()
	{
		// Spaceship movement
		if (Input.GetAxis("Vertical") != 0) 
		{
			moving = true;
			float translation = Input.GetAxis("Vertical") * speed;
			translation *= Time.deltaTime;
			transform.Translate(0, translation, 0, Space.Self);
		}
		// Spaceship rotation
		if (moving)
		{
			if (Input.GetAxis("Vertical") < 0)
			{
				if (Input.GetKey(KeyCode.D))
				{
					rotation = 1f;
					rotation *= rotationSpeed * Time.deltaTime;
					transform.Rotate(0, 0, rotation);
				}
				else if (Input.GetKey(KeyCode.A))
				{
					rotation = -1f;
					rotation *= rotationSpeed * Time.deltaTime;
					transform.Rotate(0, 0, rotation);
				}
				else
				{
					transform.Rotate(0, 0, 0);
				}
			}
			else
			{
				if (Input.GetKey(KeyCode.D))
				{
					rotation = -1f;
					rotation *= rotationSpeed * Time.deltaTime;
					transform.Rotate(0, 0, rotation);
				}
				else if (Input.GetKey(KeyCode.A))
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
		moving = false;
	}
}
