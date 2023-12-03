using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAsteroidFactory
{
	GameObject CreateAsteroid(Camera mainCamera, float asteroidSpeed);
}

public class ScaledAsteroidFactory : Object, IAsteroidFactory
{
	private List<GameObject> asteroidPrefabs;

	public ScaledAsteroidFactory(List<GameObject> asteroidPrefabs)
	{
		this.asteroidPrefabs = asteroidPrefabs;
	}

	public GameObject CreateAsteroid(Camera mainCamera, float asteroidSpeed)
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

		int spawnScaleRNG = Random.Range(0, 100);
		int spawnScale;
		switch (spawnScaleRNG)
		{
			case > 90:
				spawnScale = 3;
				break;
			case > 60:
				spawnScale = 2;
				break;
			default:
				spawnScale = 1;
				break;
		}

		GameObject asteroidPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Count)];

		GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
		asteroid.transform.localScale *= spawnScale;

		Vector3 direction = (Camera.main.transform.position - asteroid.GetComponent<Transform>().position).normalized;
		asteroid.GetComponent<Rigidbody>().AddForce(direction * asteroidSpeed);
		asteroid.GetComponent<Rigidbody>().angularVelocity = direction * 2f;
		return asteroid;
	}
}

/// <summary>
/// 
/// </summary>
public class AsteroidSpawner : MonoBehaviour
{
	private Camera mainCamera;
	private GameObject asteroidPrefab;
	public List<GameObject> asteroidList;
	public GameObject powerUp;

	public float spawnSpeed = 2f;
	public float spawnDelay = 1.0f;
	private float nextSpawnTime = 0.0f;

	public int level1SpawnChance = 60;
	public int level2SpawnChance = 40;
	public int level3SpawnChance = 10;

	public float asteroidSpeed = 750;

	public bool gameStart = false;

	private IAsteroidFactory asteroidFactory;

	/// <summary>
	/// 
	/// </summary>
	void Start()
	{
		mainCamera = Camera.main;
		asteroidFactory = new ScaledAsteroidFactory(asteroidList);
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
		if (gameStart)
		{
			GameObject asteroidSpawned = asteroidFactory.CreateAsteroid(mainCamera, asteroidSpeed);
			yield return new WaitForSeconds(spawnSpeed);
		}
	}
}
