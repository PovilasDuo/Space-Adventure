using UnityEngine;

public class BoidCollision : MonoBehaviour
{
    BoidManager boidManager;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Bullet")
        {
            FindFirstObjectByType<BoidManager>().DestroyBoid(GetComponent<Boid>());
        }
    }
}
