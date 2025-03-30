using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    [SerializeField] private GameObject leaderPrefab;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject boidPrefab;
    [SerializeField] private BoidManager boidManager;
    [SerializeField] private float spawnDistance = 100f;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
        {
            boidManager.SpawnLeader(GetMousePosition(), leaderPrefab, boidManager.GetBoidSettings().flockID);
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            boidManager.SpawnBoid(GetMousePosition(), boidPrefab, boidManager.GetBoidSettings().flockID);
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1))
        {
            SpawnGameObject(obstaclePrefab);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeleteEverything();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q)) 
        {
            DeleteBoids();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.W))
        {
            DeleteGameObjectByTag("Obstacle");
        }
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = spawnDistance;
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }

    private GameObject SpawnGameObject(GameObject gameObject)
    {
        return Instantiate(gameObject, GetMousePosition(), Quaternion.identity);
    }

    private void DeleteEverything()
    {
        DeleteBoids();
        DeleteGameObjectByTag("Obstacle");
    }

    private void DeleteBoids()
    {
        boidManager.RemoveAllFlocksOfBoids();
    }

    private void DeleteGameObjectByTag(string tag)
    {
        List<GameObject> gameObjects = GameObject.FindGameObjectsWithTag(tag).ToList();
        foreach (GameObject gameObject in gameObjects) 
        { 
            Destroy(gameObject); 
        }
    }

    public BoidManager GetBoidManager()
    {
        return boidManager;
    }

    public void SetBoidManager(BoidManager boidManager)
    {
        this.boidManager = boidManager;
    }

    public void SetBoidPrefab(GameObject boidPrefab)
    {
        this.boidPrefab = boidPrefab;
    }

    public void SetLeaderPrefab(GameObject leaderPrefab)
    {
        this.leaderPrefab = leaderPrefab;
    }

    public void SetObstaclePrefab(GameObject obstaclePrefab)
    {
        this.obstaclePrefab = obstaclePrefab;
    }

    [MenuItem("GameObject/Boid System/Create New Boid Spawner", false, 10)]
    private static void CreateNewSpawnerInstance()
    {
        GameObject boidSpawnerGO = new GameObject("BoidSpawner");
        BoidSpawner boidSpawner = boidSpawnerGO.AddComponent<BoidSpawner>();
        try
        {
            BoidManager boidManager = GameObject.FindFirstObjectByType<BoidManager>();
            boidSpawner.SetBoidManager(boidManager);
            boidSpawner.SetBoidPrefab((GameObject)Resources.Load("Boid"));
            boidSpawner.SetLeaderPrefab((GameObject)Resources.Load("LeaderBoid"));
            boidSpawner.SetObstaclePrefab((GameObject)Resources.Load("Obstacle"));
        }
        catch
        {
            Debug.LogWarning("There has been an issue with Boid Spawner automatic set up\nYou will need to finish adding the components manually");
        }
    }
}
