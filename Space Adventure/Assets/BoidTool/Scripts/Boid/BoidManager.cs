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

    public QuadTree GetQuadTree()
    {
        return quadTree;
    }

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

    public void RegisterBoid(Boid boid, int flockID)
    {
        if (!flocks.ContainsKey(flockID))
        {
            flocks[flockID] = new HashSet<Boid>();
        }
        flocks[flockID].Add(boid);
    }

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

    public void RemoveFlockOfBoids(int flockID)
    {
        if (flocks.ContainsKey(flockID))
        {
            StartCoroutine(DestroyBoidsInBatch(flocks[flockID]));
            flocks.Remove(flockID);
        }
    }

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

    public GameObject SpawnBoid(Vector3 position, GameObject boidPrfab, int flockID)
    {
        GameObject boid = regularBoidFactory.CreateBoid(position, boidPrfab, boidSettings);
        Boid boidComponent = boid.GetComponent<Boid>();

        RegisterBoid(boidComponent, flockID);

        return boid;
    }

    public GameObject SpawnLeader(Vector3 position, GameObject boidPrfab, int flockID)
    {
        GameObject boidLeader = SpawnBoid(position, boidPrfab, flockID);
        UpdateFlockLeader(flockID, boidLeader);
        return boidLeader;
    }

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

    public BoidSettings GetBoidSettings()
    {
        return boidSettings;
    }

    public BoidManagerSettings GetBoidManagerSettings()
    {
        return boidManagerSettings;
    }

    private void SetLeader(Transform newLeader)
    {
        boidSettings.leader = newLeader;
    }

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

    public void EnableFlockVisionRange(int flockID, bool enabled)
    {
        foreach (Boid boid in flocks[flockID])
        {
            boid.SetVisionRange(enabled);
        }
    }

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
