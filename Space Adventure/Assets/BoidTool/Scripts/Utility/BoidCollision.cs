using UnityEngine;

public class BoidCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Bullet")
        {
            FindFirstObjectByType<BoidManager>().DestroyBoid(GetComponent<Boid>());
        }
    }
}
