using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AsteroidSpawner: MonoBehaviour
{
	public static GameObject player;
	private Camera mainCamera;
	public GameObject asteroidPrefab;

	public float spawnSpeed = 2f;
	public float spawnDelay = 1.0f;
	private float nextSpawnTime = 0.0f;

	public float asteroidSpeed = 750;
	public float asteroidRotationSpeed = 2f;

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
		float cameraHeight = 2f * mainCamera.orthographicSize;
		float cameraWidth = cameraHeight * mainCamera.aspect;
		float spawnX, spawnY;
		int spawnType = Random.Range(0, 4);
		switch (spawnType)
		{
			case 0:
				spawnX = cameraWidth;
				spawnY = Random.Range(-cameraHeight, cameraHeight);
				break;
			case 1:
				spawnX = -cameraWidth;
				spawnY = Random.Range(-cameraHeight, cameraHeight);
				break;
			case 2:
				spawnX = Random.Range(-cameraWidth, cameraWidth);
				spawnY = cameraHeight;
				break;
			default:
				spawnX = Random.Range(-cameraWidth, cameraWidth);
				spawnY = -cameraHeight;
				break;
		}
		Vector3 spawnPosition = new(spawnX, spawnY, 0f);
		GameObject asteroidSpawned = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

		Vector3 direction = (Camera.main.transform.position - asteroidSpawned.GetComponent<Transform>().position).normalized;
		asteroidSpawned.GetComponent<Rigidbody>().AddForce(direction * asteroidSpeed);
		asteroidSpawned.GetComponent<Rigidbody>().angularVelocity = direction * asteroidRotationSpeed;
		yield return new WaitForSeconds(spawnSpeed);
	}
}
