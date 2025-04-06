using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Boid : MonoBehaviour
{
    private HashSet<Boid> nearbyBoids = new HashSet<Boid>();
    private Rigidbody rb;
    private LineRenderer lineRenderer;

    public BoidSettings boidSettings;

    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 pendingVelocity = Vector3.zero;

    private Vector3 cohesionForce;
    private Vector3 alignmentForce;
    private Vector3 separationForce;
    private Vector3 obstacleAvoidanceForce;
    private Vector3 leaderFollowForce;
    private Vector3 lineFormationForce;
    private Vector3 boundToAreaForce;
    private Vector3 enemyInteractionForce;

    private Vector3 lastPosition;
    private float stuckTimer;

    private Transform leader;
    private const int distanceFromLeader = 2;
    private const int boundToAreWeight = 10;

    private Color currentColor = Color.white;
    private float initialZ;
    private const float colorLerp = 5f;
    private float initialRayCastCooldown;
    private float rayCastCooldown;
    private float speedScalingFactor;
    private const float speedScalingFactorConst = 5f;
    private const float avoidanceTreshold = 0.2f;

    void Start()
    {
        speedScalingFactor = boidSettings.speed / speedScalingFactorConst;
        SetUpAllComponents();
        initialZ = transform.position.z;
        initialRayCastCooldown = boidSettings.rayCastCooldown;
    }

    public void InitializeBoid(BoidSettings boidSettings)
    {
        this.boidSettings = boidSettings;
    }

    public float GetVisionRadius()
    {
        return boidSettings.visionRadius;
    }

    public void SetVisionRange(bool enabled)
    {
        lineRenderer.enabled = enabled;
    }

    public float GetFlockID()
    {
        return boidSettings.flockID;
    }

    public void SetLeader(Transform newLeader)
    {
        leader = newLeader;
    }

    private void SetUpAllComponents()
    {
        SetUpRigidBody();
        SetUpLineRenderer();
        SetUpTrailRenderer();

        if (!GetComponent<Collider>())
        {
            this.AddComponent<BoxCollider>();
        }
        SetUpScreenWarp();
    }

    private void SetUpScreenWarp()
    {
        ScreenWarp screenWarp = GetOrAddComponent<ScreenWarp>();
        screenWarp.enabled = boidSettings.useScreenWarp;
    }

    private void SetUpTrailRenderer()
    {
        TrailRenderer trail = GetOrAddComponent<TrailRenderer>();
        trail.enabled = boidSettings.useTrail;

        if (boidSettings.useTrail)
        {
            trail.time = 1.5f;
            trail.startWidth = 0.1f;
            trail.endWidth = 0.05f;
            trail.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
            trail.startColor = Color.yellow;
            trail.endColor = Color.clear;
        }
    }

    private void SetUpLineRenderer()
    {
        lineRenderer = GetOrAddComponent<LineRenderer>();
        lineRenderer.enabled = boidSettings.displayVisionRange;

        if (boidSettings.displayVisionRange)
        {
            if (lineRenderer.sharedMaterial == null)
            {
                lineRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
                lineRenderer.sharedMaterial.color = Color.red;
            }

            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
        }
    }

    private void SetUpRigidBody()
    {
        if (!GetComponent<Rigidbody>())
        {
            this.AddComponent<Rigidbody>();
        }
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 1.0f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void ResetZ()
    {
        if (transform.position.z != initialZ && boidSettings.fixZPosition)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, initialZ);
        }
    }

    private T GetOrAddComponent<T>() where T : Component
    {
        return TryGetComponent(out T component) ? component : gameObject.AddComponent<T>();
    }

    public void UpdateProperties()
    {
        TrailRenderer trailRenderer = GetOrAddComponent<TrailRenderer>();
        trailRenderer.enabled = boidSettings.useTrail;
        if (boidSettings.useTrail)
        {
            SetUpTrailRenderer();
        }

        LineRenderer lineRenderer = GetOrAddComponent<LineRenderer>();
        lineRenderer.enabled = boidSettings.displayVisionRange;
        if (boidSettings.displayVisionRange)
        {
            SetUpLineRenderer();
        }


        ScreenWarp screenWarp = GetOrAddComponent<ScreenWarp>();
        screenWarp.enabled = boidSettings.useScreenWarp;
        if (boidSettings.useScreenWarp)
        {
            SetUpScreenWarp();
        }
    }

    void FixedUpdate()
    {
        if (pendingVelocity != Vector3.zero)
        {
            ApplyForce(pendingVelocity);
            pendingVelocity = Vector3.zero;
        }
    }

    public void UpdateBoid(HashSet<Boid> neighbors)
    {
        if (boidSettings.displayVisionRange)
        {
            DrawVisionArc();
        }
        if (boidSettings.visualizeColors)
        {
            UpdateBoidColor();
        }
        if (boidSettings.setBoidColorBasedOnFlockId)
        {
            SetBoidColorBasedOnFlockId();
        }

        nearbyBoids.Clear();
        nearbyBoids.UnionWith(neighbors);

        Vector3 steeringForce = Vector3.zero;
        Vector3 desiredVelocity = Vector3.zero;
        Vector3 smoothVelocity = Vector3.zero;

        if (nearbyBoids.Count > 0)
        {
            separationForce = boidSettings.separationWeight > 0 ? SteerSeparation() * boidSettings.separationWeight * speedScalingFactor : Vector3.zero;
            alignmentForce = boidSettings.alignmentWeight > 0 ? SteerAlignment() * boidSettings.alignmentWeight * speedScalingFactor : Vector3.zero;
            cohesionForce = boidSettings.cohesionWeight > 0 ? SteerCohesion() * boidSettings.cohesionWeight * speedScalingFactor : Vector3.zero;
            leaderFollowForce = leader != null ? SteerFollowLeader() * boidSettings.leaderFollowWeight * speedScalingFactor : Vector3.zero;
            lineFormationForce = boidSettings.lineFormationWeight > 0 ? SteerLineFormation() * boidSettings.lineFormationWeight * speedScalingFactor : Vector3.zero;
            enemyInteractionForce = boidSettings.enemyInteractionWeight > 0 ? EnemyInteraction() * boidSettings.enemyInteractionWeight * speedScalingFactor : Vector3.zero;
        }

        boundToAreaForce = boidSettings.boundToArea ? BoundToArea(boidSettings.minX, boidSettings.maxX, boidSettings.minY, boidSettings.maxY) * boundToAreWeight * speedScalingFactor : Vector3.zero;
        obstacleAvoidanceForce = boidSettings.avoidanceWeight > 0 ? SteerObstacleAvoidance() * boidSettings.avoidanceWeight * speedScalingFactor : Vector3.zero;

        steeringForce = separationForce + alignmentForce + cohesionForce + obstacleAvoidanceForce + leaderFollowForce + boundToAreaForce + lineFormationForce + enemyInteractionForce;

        desiredVelocity = (rb.linearVelocity + steeringForce).normalized * boidSettings.speed;
        smoothVelocity = Vector3.SmoothDamp(rb.linearVelocity, desiredVelocity, ref currentVelocity, boidSettings.smoothTime);

        CheckStuckBoid(ref smoothVelocity);

        pendingVelocity = smoothVelocity;
    }

    private void ApplyForce(Vector3 desiredVelocity)
    {
        desiredVelocity.z = 0f;
        rb.linearVelocity = desiredVelocity;
        ResetZ();

        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            transform.up = rb.linearVelocity.normalized;
        }
    }

    public bool Interacts(Boid otherBoid)
    {
        if (otherBoid.GetFlockID() == GetFlockID())
        {
            InvokeEvents(boidSettings.allyInteractionActions);
            return true;
        }
        else
        {
            InvokeEvents(boidSettings.enemyInteractionActions);
        }
        if (boidSettings.interactsWithEnemies)
        {
            return true;
        }
        return false;
    }

    private void InvokeEvents(List<BaseAction> interactionEvents)
    {
        if (interactionEvents.Count > 0)
        {
            foreach (IAction interactionEvent in interactionEvents)
            {
                interactionEvent.Invoke(this.gameObject);
            }
        }
    }

    private Vector3 SteerFollowLeader()
    {
        Vector3 ahead = leader.position + leader.forward * distanceFromLeader;
        Vector3 desiredPosition = (ahead - transform.position).normalized;
        return desiredPosition;
    }

    private Vector3 SteerLineFormation()
    {
        Vector3 steerForce = Vector3.zero;
        Vector3 averageDirection = Vector3.zero;
        Vector3 averagePosition = Vector3.zero;

        int interactedBoids = 0;
        foreach (Boid nearbyBoid in nearbyBoids)
        {
            if (Interacts(nearbyBoid))
            {
                averageDirection += nearbyBoid.transform.up;
                averagePosition += nearbyBoid.transform.position;
                interactedBoids++;
            }
        }

        averageDirection.Normalize();
        averagePosition /= interactedBoids;

        Vector3 projectedPosition = Vector3.Project(transform.position - averagePosition, averageDirection) + averagePosition;

        steerForce = (projectedPosition - transform.position).normalized;

        return steerForce;
    }

    private Vector3 SteerSeparation()
    {
        Vector3 separation = Vector3.zero;

        foreach (Boid nearbyBoid in nearbyBoids)
        {
            if (Interacts(nearbyBoid))
            {
                Vector3 offset = transform.position - nearbyBoid.transform.position;
                float distance = offset.sqrMagnitude;

                if (distance > 0)
                {
                    separation += offset.normalized;
                }
            }
        }

        return separation.normalized;
    }

    private Vector3 SteerAlignment()
    {
        Vector3 averageDirection = Vector3.zero;

        foreach (Boid nearbyBoid in nearbyBoids)
        {
            if (Interacts(nearbyBoid))
            {
                averageDirection += nearbyBoid.transform.up;
            }
        }

        return averageDirection != Vector3.zero ? (averageDirection.normalized - transform.up).normalized : Vector3.zero;
    }

    private Vector3 SteerCohesion()
    {
        Vector3 flockCenter = Vector3.zero;
        int interactedBoids = 0;

        foreach (Boid nearbyBoid in nearbyBoids)
        {
            if (Interacts(nearbyBoid))
            {
                flockCenter += nearbyBoid.transform.position;
                interactedBoids++;
            }
        }

        if (interactedBoids == 0)
        {
            return Vector3.zero;
        }

        flockCenter /= interactedBoids;

        return (flockCenter - transform.position).normalized;
    }

    private Vector3 SteerObstacleAvoidance()
    {
        if (rayCastCooldown > 0)
        {
            rayCastCooldown -= Time.deltaTime;
            return Vector3.zero;
        }
        rayCastCooldown = initialRayCastCooldown;

        int numRays = 8;
        float angleStep = boidSettings.visionAngle / (numRays - 1);
        int middleStart = numRays / 4;
        int middleEnd = (numRays * 3) / 4;
        Vector3 totalDirection = Vector3.zero;

        for (int i = 0; i < numRays; i++)
        {
            float angle = -boidSettings.visionAngle /2f + i * angleStep;
            Vector3 direction = Quaternion.AngleAxis(angle, transform.forward) * transform.up;

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, boidSettings.visionRadius))
            {
                if (hit.rigidbody != null)
                {
                    if (hit.rigidbody.CompareTag("Boid"))
                    {
                        continue;
                    }
                    else if (!hit.rigidbody.CompareTag("Obstacle") && (i >= middleStart && i <= middleEnd))
                    {
                        InvokeEvents(boidSettings.enemyInteractionActions);
                    }
                }

                float distanceFactor = (boidSettings.visionRadius - hit.distance);
                Vector3 avoidanceDirection = Vector3.Reflect(direction, hit.normal);
                float dot = Vector3.Dot(transform.up, hit.normal);
                if (dot < avoidanceTreshold)
                {
                    avoidanceDirection += -transform.up;
                }

                totalDirection += avoidanceDirection * distanceFactor * boidSettings.speed;
            }
        }
        return totalDirection;
    }

    private Vector3 BoundToArea(float minX, float maxX, float minY, float maxY)
    {
        Vector3 avoidance = Vector3.zero;

        if (transform.position.x < minX)
        {
            avoidance += new Vector3(1, 0, 0);
        }
        else if (transform.position.x > maxX)
        {
            avoidance += new Vector3(-1, 0, 0);
        }

        if (transform.position.y < minY)
        {
            avoidance += new Vector3(0, 1, 0);
        }
        else if (transform.position.y > maxY)
        {
            avoidance += new Vector3(0, -1, 0);
        }

        return avoidance.normalized;
    }

    private Vector3 EnemyInteraction()
    {
        Vector3 interactionForce = Vector3.zero;

        foreach (Boid nearbyBoid in nearbyBoids)
        {
            if (!Interacts(nearbyBoid))
            {
                Vector3 offset = transform.position - nearbyBoid.transform.position;
                float distance = offset.magnitude;

                if (distance > 0)
                {
                    if (!boidSettings.isAggressive)
                    {
                        interactionForce += offset.normalized;
                    }
                    else
                    {
                        Vector3 enemyPosition = nearbyBoid.transform.position;
                        Vector2 directionToEnemy = new Vector2(enemyPosition.x - transform.position.x, enemyPosition.y - transform.position.y);
                        Vector2 perpendicularDirection = new Vector2(-directionToEnemy.y, directionToEnemy.x);

                        perpendicularDirection.Normalize();
                        interactionForce += new Vector3(perpendicularDirection.x, perpendicularDirection.y, 0);
                    }
                }
            }
        }

        return interactionForce.normalized;
    }

    private void CheckStuckBoid(ref Vector3 velocity)
    {
        if ((transform.position - lastPosition).sqrMagnitude < boidSettings.stuckThreshold)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > boidSettings.maxStuckTime)
            {
                velocity += new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0).normalized * boidSettings.speed;
                stuckTimer = 0;
            }
        }
        else stuckTimer = 0;

        lastPosition = transform.position;
    }

    private void DrawVisionArc()
    {
        float viewAngle = boidSettings.visionAngle;
        float angleStep = viewAngle / boidSettings.lineSegments;

        lineRenderer.positionCount = boidSettings.lineSegments;

        lineRenderer.SetPosition(0, transform.position);

        for (int i = 1; i <= boidSettings.lineSegments - 2; i++)
        {
            float angle = -viewAngle / 2 + angleStep * i;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * transform.up;
            lineRenderer.SetPosition(i, transform.position + direction * boidSettings.visionRadius);
        }
        lineRenderer.SetPosition(boidSettings.lineSegments - 1, transform.position);
    }

    private void UpdateBoidColor()
    {
        float separationStrength = separationForce.sqrMagnitude;
        float cohesionStrength = cohesionForce.sqrMagnitude;
        float alignmentStrength = alignmentForce.sqrMagnitude;
        float avoidanceStrength = obstacleAvoidanceForce.sqrMagnitude;
        float leaderFollowStrength = leaderFollowForce.sqrMagnitude;
        float lineFormationStrength = lineFormationForce.sqrMagnitude;
        float boundToAreaStrength = boundToAreaForce.sqrMagnitude;
        float enemyInteractionStrength = enemyInteractionForce.sqrMagnitude;

        float totalStrength = separationStrength + cohesionStrength + alignmentStrength +
                              avoidanceStrength + leaderFollowStrength + lineFormationStrength +
                              boundToAreaStrength + enemyInteractionStrength;

        if (totalStrength <= 0)
        {
            return;
        }

        float separationWeight = separationStrength / totalStrength;
        float cohesionWeight = cohesionStrength / totalStrength;
        float alignmentWeight = alignmentStrength / totalStrength;
        float avoidanceWeight = avoidanceStrength / totalStrength;
        float leaderFollowWeight = leaderFollowStrength / totalStrength;
        float lineFormationWeight = lineFormationStrength / totalStrength;
        float boundToAreaWeight = boundToAreaStrength / totalStrength;
        float enemyInteractionWeight = enemyInteractionStrength / totalStrength;

        Color targetColor =
            (Color.red * separationWeight) +
            (Color.green * cohesionWeight) +
            (Color.blue * alignmentWeight) +
            (Color.yellow * avoidanceWeight) +
            (Color.magenta * leaderFollowWeight) +
            (Color.cyan * lineFormationWeight) +
            (Color.gray * boundToAreaWeight) +
            (Color.black * enemyInteractionWeight);

        if (TryGetComponent<Renderer>(out Renderer renderer))
        {
            currentColor = renderer.material.color;
        }

        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorLerp);
        renderer.material.color = currentColor;
    }

    private void SetBoidColorBasedOnFlockId()
    {
        Color[] flockColors = new Color[10]
        {
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.magenta,
            Color.cyan,
            Color.white,
            new Color(1f, 0.5f, 0f),  // orange
            new Color(0.5f, 0f, 0.5f), // purple
            new Color(0.3f, 0.6f, 0.2f)  // greenish
        };

        Color targetColor = flockColors[boidSettings.flockID];

        if (TryGetComponent<Renderer>(out Renderer renderer))
        {
            renderer.material.color = targetColor;
        }
    }
}
