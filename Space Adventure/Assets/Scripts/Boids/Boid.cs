using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Boid : MonoBehaviour
{
    public float visionConeThreshold = 0.5f;
    public float radius = 5.0f;
    public float speed = 5.0f;
    public float cohesionWeight = 1.0f;
    public float alignmentWeight = 1.0f;
    public float separationWeight = 1.5f;
    public float maxSteerForce = 1.0f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Ensure no gravity is applied
        transform.forward = new Vector3(0, 1, 0);
    }

    void FixedUpdate()
    {
        Vector3 separationForce = SteerSeparation() * separationWeight;
        Vector3 alignmentForce = SteerAlignment() * alignmentWeight;
        Vector3 cohesionForce = SteerCohesion() * cohesionWeight;

        Vector3 steeringForce = separationForce + alignmentForce + cohesionForce;
        //steeringForce = Vector3.ClampMagnitude(steeringForce, maxSteerForce); // Limit steering force
        ApplySteeringForce(steeringForce);
    }

    private void ApplySteeringForce(Vector3 force)
    {
        if (force != Vector3.zero)
        {
            Vector3 desiredVelocity = force.normalized * speed;
            Vector3 velocityChange = desiredVelocity - rb.linearVelocity;
            velocityChange = Vector3.ClampMagnitude(velocityChange, maxSteerForce);

            Debug.Log("Steering force applied: " + velocityChange);  // Debug to see the force magnitude

            rb.AddForce(velocityChange, ForceMode.Acceleration);
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, speed); // Limit max speed

            if (rb.linearVelocity != Vector3.zero)
                transform.forward = rb.linearVelocity.normalized; // Rotate in movement direction
        }
    }

    private Vector3 SteerSeparation()
    {
        Vector3 direction = Vector3.zero;
        List<GameObject> boids = FindAllBoids();
        Debug.Log("Boids in separation range: " + boids.Count);

        foreach (GameObject boid in boids)
        {
            Vector3 offset = boid.transform.position - transform.position;
            float ratio = Mathf.Clamp01(offset.magnitude / radius);
            direction -= (offset.normalized * (1 - ratio)); // Inverse weighting for separation
        }

        Debug.Log("Separation Force: " + direction);  // Debug the force applied for separation
        return direction.normalized;
    }


    private Vector3 SteerAlignment()
    {
        List<GameObject> boids = FindAllBoids();
        if (boids.Count == 0)
            return Vector3.zero;

        Vector3 averageDirection = Vector3.zero;
        foreach (GameObject boid in boids)
        {
            averageDirection += boid.transform.forward;
        }
        averageDirection /= boids.Count;

        return (averageDirection - transform.forward).normalized;
    }

    private Vector3 SteerCohesion()
    {
        List<GameObject> boids = FindAllBoids();
        if (boids.Count == 0)
            return Vector3.zero;

        Vector3 centerOfMass = Vector3.zero;
        foreach (GameObject boid in boids)
        {
            centerOfMass += boid.transform.position;
        }
        centerOfMass /= boids.Count;

        return (centerOfMass - transform.position).normalized;
    }

    private bool InVisionCone(Vector3 otherBoidDirection)
    {
        ////return Vector3.Dot(transform.forward, otherBoidDirection.normalized) > visionConeThreshold;
        //float dot = Vector3.Dot(transform.forward, otherBoidDirection.normalized);
        //Debug.Log("Dot product: " + dot);
        //return dot > visionConeThreshold;
        // Project to 2D by zeroing out the z components to ignore 3D effects
        Vector3 forward2D = new Vector3(transform.forward.x, transform.forward.y, 0);
        Vector3 otherDirection2D = new Vector3(otherBoidDirection.x, otherBoidDirection.y, 0);

        // Log both vectors to check if they are aligned in the 2D plane
        Debug.Log("Boid Forward (2D): " + forward2D);
        Debug.Log("Other Boid Direction (2D): " + otherDirection2D);

        // Calculate the dot product for the 2D direction
        float dot = Vector3.Dot(forward2D.normalized, otherDirection2D.normalized);

        Debug.Log("Dot product: " + dot); // Debug log to check the dot product
        return dot > visionConeThreshold;
    }

    private List<GameObject> FindAllBoids()
    {
        var boidsInRange = GetBoidsInRange().FindAll(b =>
            b != gameObject &&
            (b.transform.position - transform.position).magnitude <= radius &&
            InVisionCone(b.transform.position - transform.position));

        Debug.Log("Boids detected in vision cone: " + boidsInRange.Count);  // Debug the detected boids
        return boidsInRange;
    }


    private List<GameObject> GetBoidsInRange()
    {
        return GameObject.FindGameObjectsWithTag("Boid").ToList();
    }
}
