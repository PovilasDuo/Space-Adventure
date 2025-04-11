using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObstacleFactory: Object
{
    public abstract GameObject CreateObstacle(Vector3 position);

    public Vector3 CreatePosition()
    {
        Camera cam = Camera.main;
        float cameraHeight = 2f * cam.orthographicSize;
        float cameraWidth = cameraHeight * cam.aspect;

        float halfWidth = cameraWidth / 2f;
        float halfHeight = cameraHeight / 2f;

        Vector3 camPos = cam.transform.position;

        float spawnX, spawnY;
        int spawnType = Random.Range(0, 4);
        switch (spawnType)
        {
            case 0: // Right
                spawnX = camPos.x + halfWidth + 1f;
                spawnY = Random.Range(camPos.y - halfHeight, camPos.y + halfHeight);
                break;
            case 1: // Left
                spawnX = camPos.x - halfWidth - 1f;
                spawnY = Random.Range(camPos.y - halfHeight, camPos.y + halfHeight);
                break;
            case 2: // Top
                spawnX = Random.Range(camPos.x - halfWidth, camPos.x + halfWidth);
                spawnY = camPos.y + halfHeight + 1f;
                break;
            default: // Bottom
                spawnX = Random.Range(camPos.x - halfWidth, camPos.x + halfWidth);
                spawnY = camPos.y - halfHeight - 1f;
                break;
        }

        return new Vector3(spawnX, spawnY, 0f);
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
	public GameObject CreateObstacle(Vector3 position)
	{
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

		GameObject asteroid = Instantiate(asteroidPrefab, position, Quaternion.identity);
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
			GameObject asteroidSpawned = asteroidFactory.CreateObstacle(asteroidFactory.CreatePosition());
			yield return new WaitForSeconds(spawnSpeed);
		}
	}
}
