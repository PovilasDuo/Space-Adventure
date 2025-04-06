using UnityEngine;

[CreateAssetMenu(fileName = "BoidManagerSettings", menuName = "Boid/ManagerSettings", order = 2)]
public class BoidManagerSettings : ScriptableObject
{
    [Header("Boid Manager Settings")]
    public int worldSizeX = 1000;
    public int worldSizeY = 1000;
    public float updateInterval = 0.05f;
    public bool manageBoids = true;
    public float delayUntilNextRecursion = 1f;
    public GameObject boidPrefab = null;
}
