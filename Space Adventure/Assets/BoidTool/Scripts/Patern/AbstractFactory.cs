using UnityEngine;

public interface IBoidFactory
{
    GameObject CreateBoid(Vector3 position, GameObject boidPrefab,  BoidSettings boidSettings);
}

public class RegularBoidFactory : Object, IBoidFactory
{
    public  GameObject CreateBoid(Vector3 position, GameObject boidPrefab, BoidSettings boidSettings)
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