using System.Collections;
using UnityEngine;

public class AdhocBoidFactory : ObstacleFactory
{
    private BoidManager boidManager;
    private GameObject boidPrefab;

    public AdhocBoidFactory(BoidManager boidManager, GameObject boidPrefab)
    {
        this.boidManager = boidManager;
        this.boidPrefab = boidPrefab;
    }

    public override GameObject CreateObstacle(Camera mainCamera)
    {
        Vector3 spawnPosition = CreatePosition(mainCamera);
        return boidManager.SpawnBoid(spawnPosition, boidPrefab, boidManager.GetBoidSettings().flockID);
    }
}

public class AdhocBoidSpawner : MonoBehaviour
{
    public BoidManager boidManager;
    public GameObject boidPrefab;

    public float spawnSpeed = 2f;
    public float spawnDelay = 1.0f;
    private float nextSpawnTime = 0.0f;
    public int minBoidsSpawned = 1;
    public int maxBoidsSpawned = 10;

    public bool gameStart = false;

    private AdhocBoidFactory adhocBoidFactory;

    void Start()
    {
        adhocBoidFactory = new AdhocBoidFactory(boidManager, boidPrefab);
    }

    void Update()
    {
        if (!gameStart)
        {
            gameStart = FindFirstObjectByType<AsteroidSpawner>().gameStart;
        }
        if (Time.time > nextSpawnTime && gameStart)
        {
            StartCoroutine(SpawnBoids());
            nextSpawnTime = Time.time + spawnDelay;
        }
    }


    IEnumerator SpawnBoids()
    {
        if (gameStart)
        {
            int boidsToBeSpawned = Random.Range(minBoidsSpawned, maxBoidsSpawned);
            for (int i = 0; i < boidsToBeSpawned; i++)
            {
                adhocBoidFactory.CreateObstacle(Camera.main);
            }
            yield return new WaitForSeconds(spawnSpeed);
        }
    }
}
