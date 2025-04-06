using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObstacleFactory: Object
{
    public abstract GameObject CreateObstacle(Camera mainCamera);

	public Vector3 CreatePosition(Camera mainCamera)
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
        return new(spawnX, spawnY, 0f);
    }
}

public class ScaledAsteroidFactory : ObstacleFactory
{
	private List<GameObject> asteroidPrefabs;
	private const float asteroidSpeed = 750;
    public ScaledAsteroidFactory(List<GameObject> asteroidPrefabs)
	{
		this.asteroidPrefabs = asteroidPrefabs;
	}

	override
	public GameObject CreateObstacle(Camera mainCamera)
	{
		Vector3 spawnPosition = CreatePosition(mainCamera);

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
		asteroid.GetComponent<Rigidbody>().angularVelocity = direction;

        return asteroid;
	}
}

public class AsteroidSpawner : MonoBehaviour
{
    private Camera mainCamera;
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

	private ObstacleFactory asteroidFactory;

	void Start()
	{
		mainCamera = Camera.main;
		asteroidFactory = new ScaledAsteroidFactory(asteroidList);
	}

	void Update()
	{
		if (Time.time > nextSpawnTime)
		{
			StartCoroutine(SpawnAsteroids());
			nextSpawnTime = Time.time + spawnDelay;
		}
	}

	IEnumerator SpawnAsteroids()
	{
		if (gameStart)
		{
			GameObject asteroidSpawned = asteroidFactory.CreateObstacle(mainCamera);
			yield return new WaitForSeconds(spawnSpeed);
		}
	}
}
