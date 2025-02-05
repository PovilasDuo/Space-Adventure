using UnityEngine;

public class Boid2d : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 5f;
    public float neighborRadius = 3f;
    public float separationDistance = 1f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float separationWeight = 1f;

    private Rigidbody2D rb;
    private Transform target;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 velocity = Vector2.zero;

        // Get nearby boids
        Collider2D[] nearbyBoids = Physics2D.OverlapCircleAll(transform.position, neighborRadius);

        // Variables for boid behaviors
        Vector2 alignment = Vector2.zero;
        Vector2 cohesion = Vector2.zero;
        Vector2 separation = Vector2.zero;
        int numNeighbors = 0;

        foreach (var boid in nearbyBoids)
        {
            if (boid != this.GetComponent<Collider2D>() && boid != null)
            {
                numNeighbors++;
                Boid2d otherBoid = boid.GetComponent<Boid2d>();

                // Alignment: steer towards average direction of neighbors
                alignment += (Vector2)otherBoid.transform.up;

                // Cohesion: steer towards average position of neighbors
                cohesion += (Vector2)boid.transform.position;

                // Separation: avoid crowding neighbors
                float distance = Vector2.Distance(transform.position, boid.transform.position);
                if (distance < separationDistance)
                {
                    separation += (Vector2)(transform.position - boid.transform.position) / distance;
                }
            }
        }

        if (numNeighbors > 0)
        {
            alignment /= numNeighbors;
            cohesion /= numNeighbors;
            separation /= numNeighbors;
        }

        // Apply boid behaviors
        velocity += alignment * alignmentWeight;
        velocity += cohesion * cohesionWeight;
        velocity += separation * separationWeight;

        // Normalize and apply speed
        if (velocity.sqrMagnitude > 0)
        {
            velocity.Normalize();
            velocity *= speed;
        }

        // Apply the velocity to the Rigidbody2D for movement
        rb.linearVelocity = velocity;

        // Rotate the boid towards the velocity vector
        if (velocity.sqrMagnitude > 0)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotationSpeed * Time.deltaTime);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, neighborRadius);
    }
}
