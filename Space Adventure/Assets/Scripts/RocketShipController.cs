using UnityEngine;
using UnityEngine.InputSystem;

public class RocketShipController : MonoBehaviour
{
	public int health = 10;
	public float speed = 6f;
	public float rotationSpeed = 80f;
	public float fireRate = 1f; 
	private float nextFireTime = 0f;

	public GameObject projectilePrefab;
	public Transform projectileSpawnPoint;
	public float projectileForce = 2f;
	public GameObject particlesFire;
	public AudioSource lazer;

	private bool moving;
	private float rotation;

	public bool p1;
	private string verticalAxis = "Vertical";
	private KeyCode left = KeyCode.A;
	private KeyCode right = KeyCode.D;
	private KeyCode fire = KeyCode.Space;

	public PlayerInput playerInput;
	private void Start()
	{
		playerInput = GetComponent<PlayerInput>();
		if (!p1)
		{
			verticalAxis = "Vertical2";
			left = KeyCode.LeftArrow;
			right = KeyCode.RightArrow;
			fire = KeyCode.Keypad0;
		}
	}

	void Update()
	{
		Movement();
		if (Input.GetKey(fire) && Time.time >= nextFireTime)
		{
			FireProjectile();
			nextFireTime = Time.time + 1f / fireRate;
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
		if (playerInput.actions["Move"].ReadValue<Vector2>().y != 0)
		{
			moving = true;
			float translation = Input.GetAxisRaw(verticalAxis) * speed;
			translation *= Time.deltaTime;
			transform.Translate(0, translation, 0, Space.Self);
		}
		else
		{
			moving = false;
		}


		float scaleFactor = moving ? 0.4f : 0.2f;
		particlesFire.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

		if (moving)
		{
			if (playerInput.actions["Move"].ReadValue<Vector2>().x != 0)
			{
				if (Input.GetKey(left))
				{
					rotation = 1f;
					rotation *= rotationSpeed * Time.deltaTime;
					transform.Rotate(0, 0, rotation);
				}
				else if (Input.GetKey(right))
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
				if (Input.GetKey(left))
				{
					rotation = -1f;
					rotation *= rotationSpeed * Time.deltaTime;
					transform.Rotate(0, 0, rotation);
				}
				else if (Input.GetKey(right))
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
