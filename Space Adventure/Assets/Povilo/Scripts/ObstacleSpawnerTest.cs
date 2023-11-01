using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnerTest : MonoBehaviour
{
	public static GameObject player;
	private Camera mainCamera;
	public GameObject asteroidPrefab;
	public float spawnSpeed = 2f;
	public float asteroidMS = 2f;

	private float nextSpawnTime = 0.0f;
	public float spawnDelay = 1.0f;

	// Start is called before the first frame update
	void Start()
	{
		player = GameObject.Find("Rocket");
		mainCamera = Camera.main;
	}

	void Update()
	{
		// Check if it's time to spawn asteroids
		if (Time.time > nextSpawnTime)
		{
			StartCoroutine(SpawnAsteroids());
			nextSpawnTime = Time.time + spawnDelay;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	IEnumerator SpawnAsteroids()
	{
		// Get the camera's viewport dimensions
		float cameraHeight = 2f * mainCamera.orthographicSize;
		float cameraWidth = cameraHeight * mainCamera.aspect;

		// Define an offset from the camera's viewpoint
		float offset = 1.5f;

		// Calculate the spawn position
		float spawnX = Random.Range(-cameraWidth * offset, cameraWidth * offset);
		float spawnY = Random.Range(-cameraHeight * offset, cameraHeight * offset);
		Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

		// Instantiate the object at the spawn position
		GameObject asteroidSpanwed = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
		MoveTowards script = asteroidSpanwed.AddComponent<MoveTowards>();
		yield return new WaitForSeconds(spawnSpeed);
	}

	public class MoveTowards : MonoBehaviour
	{
		public float speed = 5.0f;

		void Update()
		{
			Vector3 targetPosition = player.transform.position;
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
		}
	}
}
