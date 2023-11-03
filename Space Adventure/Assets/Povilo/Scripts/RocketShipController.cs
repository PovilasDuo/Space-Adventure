using UnityEngine;

public class RocketShipController : MonoBehaviour
{
	public int health = 10;
	public float speed = 6f;
	public float rotationSpeed = 80f;

	public GameObject projectilePrefab;
	public Transform projectileSpawnPoint;
	public float projectileForce = 2f;
	public GameObject particlesFire;
	public AudioSource lazer;

	private bool moving;
	private float rotation;

	private void Start()
	{

	}

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
		lazer.Play();
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
		else moving = false;

		float scaleFactor = moving ? 0.4f : 0.2f; // Adjust the scale factor as needed
		particlesFire.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

		// Spaceship rotation
		if (moving)
		{
			//			particlesFire.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
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
	}
}
