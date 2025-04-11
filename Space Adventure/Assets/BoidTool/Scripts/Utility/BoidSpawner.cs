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
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Escape))
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

    /// <summary>
    /// Gets the mouse position in world coordinates.
    /// </summary>
    /// <returns>The mouse position in world coordinates</returns>
    private Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(mainCamera.transform.position.z);
        Vector3 desiredPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        desiredPosition.z = 0;
        return desiredPosition;
    }

    /// <summary>
    /// Spawns a game object at the mouse position.
    /// </summary>
    /// <param name="gameObject">GameObject to be spawned</param>
    /// <returns>The spawned GameObject</returns>
    private GameObject SpawnGameObject(GameObject gameObject)
    {
        return Instantiate(gameObject, GetMousePosition(), Quaternion.identity);
    }

    /// <summary>
    /// Deletes all boids and obstacles in the scene.
    /// </summary>
    private void DeleteEverything()
    {
        DeleteBoids();
        DeleteGameObjectByTag("Obstacle");
    }

    /// <summary>
    /// Deletes all boids in the scene.
    /// </summary>
    private void DeleteBoids()
    {
        boidManager.RemoveAllFlocksOfBoids();
    }

    /// <summary>
    /// Deletes all game objects with the specified tag.
    /// </summary>
    /// <param name="tag">The specified tag</param>
    private void DeleteGameObjectByTag(string tag)
    {
        List<GameObject> gameObjects = GameObject.FindGameObjectsWithTag(tag).ToList();
        foreach (GameObject gameObject in gameObjects) 
        { 
            Destroy(gameObject); 
        }
    }

    /// <summary>
    /// Gets the boid manager.
    /// </summary>
    /// <returns>BoidManager that is assigned to the BoidSpawner</returns>
    public BoidManager GetBoidManager()
    {
        return boidManager;
    }

    /// <summary>
    /// Sets the boid manager.
    /// </summary>
    /// <param name="boidManager">BoidManager to be set</param>
    public void SetBoidManager(BoidManager boidManager)
    {
        this.boidManager = boidManager;
    }

    /// <summary>
    /// Sets the boid prefab.
    /// </summary>
    /// <param name="boidPrefab">Boid prefab to be set</param>
    public void SetBoidPrefab(GameObject boidPrefab)
    {
        this.boidPrefab = boidPrefab;
    }

    /// <summary>
    /// Sets the leader prefab.
    /// </summary>
    /// <param name="leaderPrefab">Leader prefab to be set</param>
    public void SetLeaderPrefab(GameObject leaderPrefab)
    {
        this.leaderPrefab = leaderPrefab;
    }

    /// <summary>
    /// Sets the obstacle prefab.
    /// </summary>
    /// <param name="obstaclePrefab">Obstacle prefab to be set</param>
    public void SetObstaclePrefab(GameObject obstaclePrefab)
    {
        this.obstaclePrefab = obstaclePrefab;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Creates a new Boid Spawner instance in the scene.
    /// </summary>
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
#endif
}
