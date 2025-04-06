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

    public override GameObject CreateObstacle(Vector3 position)
    {
        return boidManager.SpawnBoid(position, boidPrefab, boidManager.GetBoidSettings().flockID);
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
            Vector3 position = adhocBoidFactory.CreatePosition();
            for (int i = 0; i < boidsToBeSpawned; i++)
            {
                GameObject boid = adhocBoidFactory.CreateObstacle(position);

                int powerUpRNG = Random.Range(0, 100);
                if (powerUpRNG >= 75)
                {
                    boid.GetComponent<Renderer>().material.color = Color.cyan;
                    boid.GetComponent<AsteroidCollision>().isPowerUp = true;
                }
            }
            yield return new WaitForSeconds(spawnSpeed);
        }
    }
}
