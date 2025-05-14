using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [SerializeField] private BoidSettings boidSettings;
    [SerializeField] private BoidManagerSettings boidManagerSettings;

    private QuadTree quadTree;
    private Dictionary<int, HashSet<Boid>> flocks = new Dictionary<int, HashSet<Boid>>();
    private RegularBoidFactory regularBoidFactory = new RegularBoidFactory();

    private const int maxBoidCount = 1000;

    void Start()
    {
        quadTree = QuadTree.GetInstance(new Rect(
            -boidManagerSettings.worldSizeX / 2, -boidManagerSettings.worldSizeY / 2,
            boidManagerSettings.worldSizeX, boidManagerSettings.worldSizeY),
            maxBoidCount);

        StartCoroutine(UpdateBoidsRoutine(boidManagerSettings.updateInterval));
        RegisterBoidsOnStart();
    }

    /// <summary>
    /// Starts a coroutine to update the boids at a specified interval.
    /// </summary>
    /// <param name="updateInterval">Update interval for the boid management</param>
    private IEnumerator UpdateBoidsRoutine(float updateInterval)
    {
        while (boidManagerSettings.manageBoids)
        {
            yield return new WaitForSeconds(updateInterval);
            UpdateBoids();
        }

        yield return new WaitForSeconds(boidManagerSettings.delayUntilNextRecursion);
        StartCoroutine(UpdateBoidsRoutine(updateInterval));
    }

    /// <summary>
    /// Updates the boids in the scene.
    /// </summary>
    private void UpdateBoids()
    {
        foreach (var flock in flocks.Values)
        {
            foreach (var boid in flock)
            {
                quadTree.Insert(boid);
            }
            UpdateFlock(flock);
        }
    }

    /// <summary>
    /// Updates the flock of boids by querying the QuadTree for nearby boids.
    /// </summary>
    /// <param name="flock">Flock to be updated</param>
    private void UpdateFlock(HashSet<Boid> flock)
    {
        foreach (Boid boid in flock)
        {
            if (boid != null)
            {
                boid.UpdateBoid(quadTree.Query(boid, boid.GetVisionRadius()));
            }
        }
    }

    /// <summary>
    /// Registers a boid to a specific flock.
    /// </summary>
    /// <param name="boid">Boid to be registered</param>
    /// <param name="flockID">The flock to which the boid will be registered</param>
    public void RegisterBoid(Boid boid, int flockID)
    {
        if (!flocks.ContainsKey(flockID))
        {
            flocks[flockID] = new HashSet<Boid>();
        }
        flocks[flockID].Add(boid);
    }

    /// <summary>
    /// Removes all flocks of boids from the scene.
    /// </summary>
    public void RemoveAllFlocksOfBoids()
    {
        List<int> flockKeys = new List<int>(flocks.Keys);
        foreach (var flockID in flockKeys)
        {
            RemoveFlockOfBoids(flockID);
        }
        quadTree.Clear();
        EnsureDeletion();
    }

    /// <summary>
    /// Removes a specific flock of boids from the scene.
    /// </summary>
    /// <param name="flockID">The flock to be removed</param>
    public void RemoveFlockOfBoids(int flockID)
    {
        if (flocks.ContainsKey(flockID))
        {
            StartCoroutine(DestroyBoidsInBatch(flocks[flockID]));
            flocks.Remove(flockID);
        }
    }

    /// <summary>
    /// Destroys a batch of boids in the scene.
    /// </summary>
    /// <param name="boids">Flock of boids to be destroyed</param>
    private IEnumerator DestroyBoidsInBatch(HashSet<Boid> boids)
    {
        int batchSize = 32;
        Boid[] boidArray = new Boid[boids.Count];
        boids.CopyTo(boidArray);

        for (int i = 0; i < boidArray.Length; i += batchSize)
        {
            for (int j = i; j < i + batchSize && j < boidArray.Length; j++)
            {
                if (boidArray[j] != null && boidArray[j].gameObject != null)
                {
                    quadTree.Remove(boidArray[j]);
                    Destroy(boidArray[j].gameObject);
                }
            }
            yield return null;
        }
        boids.Clear();
    }

    /// <summary>
    /// Ensures that all boids are deleted from the scene.
    /// </summary>
    private void EnsureDeletion()
    {
        GameObject[] boids = GameObject.FindGameObjectsWithTag("Boid");
        if (boids.Length != 0)
        {
            foreach (GameObject boid in boids)
            {
                if (boid != null)
                {
                    Destroy(boid);
                }
            }
        }
    }

    /// <summary>
    /// Spawns a boid at a specified position with the given prefab and flock ID.
    /// </summary>
    /// <param name="position">Specified position</param>
    /// <param name="boidPrfab">The prefab of the boid to be spawned</param>
    /// <param name="flockID">The flock id which will be assigned to the boid</param>
    /// <returns>The spawned boid</returns>
    public GameObject SpawnBoid(Vector3 position, GameObject boidPrfab, int flockID)
    {
        GameObject boid = regularBoidFactory.CreateBoid(position, boidPrfab, boidSettings);
        Boid boidComponent = boid.GetComponent<Boid>();

        RegisterBoid(boidComponent, flockID);

        return boid;
    }

    /// <summary>
    /// Spawns a leader boid at a specified position with the given prefab and flock ID.
    /// </summary>
    /// <param name="position">Specified position</param>
    /// <param name="boidPrfab">The prefab of the boid to be spawned</param>
    /// <param name="flockID">The flock id which will be assigned to the boid</param>
    /// <returns>The spawned boid</returns>
    public GameObject SpawnLeader(Vector3 position, GameObject boidPrfab, int flockID)
    {
        GameObject boidLeader = SpawnBoid(position, boidPrfab, flockID);
        UpdateFlockLeader(flockID, boidLeader);
        return boidLeader;
    }

    /// <summary>
    /// Updates the leader of a flock by setting the leader for all boids in the flock.
    /// </summary>
    /// <param name="flockID">Flock's id</param>
    /// <param name="leaderBoid">The leader boid to be aassigned to the flock</param>
    private void UpdateFlockLeader(int flockID, GameObject leaderBoid)
    {
        if (flocks.ContainsKey(flockID))
        {
            foreach (var boid in flocks[flockID])
            {
                if (boid.gameObject != leaderBoid)
                {
                    boid.SetLeader(leaderBoid.transform);
                }
            }
        }
    }

    /// <summary>
    /// Destroys a boid and removes it from the QuadTree and its flock.
    /// </summary>
    /// <param name="boid">The boid to be destroyed</param>
    public void DestroyBoid(Boid boid)
    {
        if (boid == null) return;

        foreach (var flock in flocks.Values)
        {
            if (flock.Contains(boid))
            {
                flock.Remove(boid);
                break;
            }
        }

        quadTree.Remove(boid);
        Destroy(boid.gameObject);
    }

    /// <summary>
    /// Returns the boid settings.
    /// </summary>
    public BoidSettings GetBoidSettings()
    {
        return boidSettings;
    }

    /// <summary>
    /// Returns the boid manager settings.
    /// </summary>
    public BoidManagerSettings GetBoidManagerSettings()
    {
        return boidManagerSettings;
    }

    /// <summary>
    /// Updates the properties of all boids in the scene.
    /// </summary>
    public void UpdatetFlockSettings()
    {
        foreach (HashSet<Boid> flock in flocks.Values)
        {
            foreach (Boid boid in flock)
            {
                boid.UpdateProperties();
            }
        }
    }

    /// <summary>
    /// Enables or disables the vision range of all boids in a specific flock.
    /// </summary>
    /// <param name="flockID">The flock for which to apply this</param>
    /// <param name="enabled">To enable or to disable the vision range</param>
    public void EnableFlockVisionRange(int flockID, bool enabled)
    {
        if (flocks.ContainsKey(flockID))
        {
            foreach (Boid boid in flocks[flockID])
            {
                boid.SetVisionRange(enabled);
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Creates a new BoidManager instance in the scene.
    /// </summary>
    [MenuItem("GameObject/Boid System/Create New Boid Manager", false, 10)]
    private static void CreateNewManagerInstance()
    {
        string folderPath = "Assets/Scripts/ScriptableObject/";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        BoidSettings boidSettingsSO = ScriptableObject.CreateInstance<BoidSettings>();
        BoidManagerSettings boidManagerSettingsSO = ScriptableObject.CreateInstance<BoidManagerSettings>();

        string boidSettingsPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "BoidSettings.asset");
        string boidManagerSettingsPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "BoidManagerSettings.asset");

        AssetDatabase.CreateAsset(boidSettingsSO, boidSettingsPath);
        AssetDatabase.CreateAsset(boidManagerSettingsSO, boidManagerSettingsPath);
        AssetDatabase.SaveAssets();

        string baseName = "BoidManager";
        int index = 1;
        while (GameObject.Find(baseName + index) != null)
        {
            index++;
        }
        string uniqueName = baseName + index;

        GameObject newBoidManager = new GameObject(uniqueName);

        BoidManager boidManagerComponent = newBoidManager.AddComponent<BoidManager>();

        boidManagerComponent.boidSettings = boidSettingsSO;
        boidManagerComponent.boidManagerSettings = boidManagerSettingsSO;

        Undo.RegisterCreatedObjectUndo(newBoidManager, "Create Boid Manager");

        Selection.activeObject = newBoidManager;
    }
#endif

    /// <summary>
    /// Registers all boids in the scene at the start.
    /// </summary>
    private void RegisterBoidsOnStart()
    {
        Boid[] boids = FindObjectsByType<Boid>(FindObjectsSortMode.None);
        if (boids.Length != 0)
        {
            foreach (Boid boid in boids)
            {
                if (boid.boidSettings == boidSettings)
                {
                    RegisterBoid(boid, boid.boidSettings.flockID);
                }
            }
        }
    }
}
