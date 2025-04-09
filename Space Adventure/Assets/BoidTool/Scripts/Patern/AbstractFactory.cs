using UnityEngine;

public interface IBoidFactory
{
    /// <summary>
    /// Creates a boid object at the specified position using the provided prefab and settings.
    /// </summary>
    /// <param name="position">The position for the boid to be spawned in</param>
    /// <param name="boidPrefab">The prefab for the boid</param>
    /// <param name="boidSettings">The settings for the boid</param>
    /// <returns>Created boid</returns>
    GameObject CreateBoid(Vector3 position, GameObject boidPrefab,  BoidSettings boidSettings);
}

public class RegularBoidFactory : Object, IBoidFactory
{
    public GameObject CreateBoid(Vector3 position, GameObject boidPrefab, BoidSettings boidSettings)
    {
        GameObject boid = Instantiate(boidPrefab, position, Quaternion.identity);
        boid.tag = "Boid";
        boid.AddComponent<Boid>();
        Boid boidComponent = boid.GetComponent<Boid>();

        if (boidComponent != null && boidSettings != null)
        {
            boidComponent.InitializeBoid(boidSettings);
        }

        return boid;
    }
}