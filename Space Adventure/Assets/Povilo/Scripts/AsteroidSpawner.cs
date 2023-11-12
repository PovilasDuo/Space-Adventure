using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
	//public static GameObject player;
	private Camera mainCamera;
	private GameObject asteroidPrefab;
	public List<GameObject> asteroidList;

	public float spawnSpeed = 2f;
	public float spawnDelay = 1.0f;
	private float nextSpawnTime = 0.0f;

	public int level1SpawnChance = 60;
	public int level2SpawnChance = 40;
	public int level3SpawnChance = 10;

	public float asteroidSpeed = 750;
	public float asteroidRotationSpeed = 2f;

	public bool gameStart = false;

	// Start is called before the first frame update
	void Start()
	{
		//player = GameObject.Find("Rocket");
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
		if (gameStart)
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
			asteroidPrefab = asteroidList[Random.Range(0, asteroidList.Count - 1)];
			GameObject asteroidSpawned = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
			asteroidSpawned.transform.localScale *= spawnScale;

			Vector3 direction = (Camera.main.transform.position - asteroidSpawned.GetComponent<Transform>().position).normalized;
			asteroidSpawned.GetComponent<Rigidbody>().AddForce(direction * asteroidSpeed);
			asteroidSpawned.GetComponent<Rigidbody>().angularVelocity = direction * asteroidRotationSpeed;
			yield return new WaitForSeconds(spawnSpeed);
		}
	}
}
