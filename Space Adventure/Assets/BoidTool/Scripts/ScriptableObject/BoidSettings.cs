using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoidSettings", menuName = "Boid/Settings", order = 1)]
public class BoidSettings : ScriptableObject
{
    [Header("Vision Settings")]
    public int flockID = 0;

    [Header("Vision Settings")]
    [Range(1, 50)] public int visionRadius = 10;
    [Range(1, 180)] public int visionAngle = 120;

    [Header("Movement Settings")]
    [Range(0, 20)] public int speed = 10;
    [Range(0, 1)] public float smoothTime = 0.03f;
    [Range(0, 10)] public float speedScalingFactor = 5f;

    [Header("Behavior Weights")]
    [Range(0, 10)] public float cohesionWeight = 1;
    [Range(0, 10)] public float alignmentWeight = 1;
    [Range(0, 10)] public float separationWeight = 1.5f;
    [Range(0, 10)] public int leaderFollowWeight = 5;
    [Range(0, 10)] public int avoidanceWeight = 5;
    [Range(0, 10)] public float lineFormationWeight = 0;
    [Range(0, 10)] public int enemyInteractionWeight = 10;
    [Range(0, 10)] public int boundToAreWeight = 10;

    [Header("Behavioral Settings")]
    public bool isAggressive = false;
    public bool interactsWithEnemies = false;
    public List<BaseAction> enemyInteractionActions;
    public List<BaseAction> allyInteractionActions;
    public List<BaseAction> rayCastInteractionActions;
    public Transform leader = null;

    [Header("Stuck Detection")]
    [Range(0.01f, 1.0f)] public float stuckThreshold = 0.05f;
    [Range(0.1f, 5.0f)] public float maxStuckTime = 0.5f;

    [Header("Physics & Rendering")]
    [Range(1, 16)] public int numberOfRays = 8;
    [Range(0, 1.0f)] public float rayCastCooldown = 0.1f;
    [Range(1, 64)] public int lineSegments = 20;

    [Header("Visualization")]
    public bool displayVisionRange = false;
    public bool visualizeColors = false;
    public bool useTrail = true;
    [Range(0, 3)] public float trailTime = 1.5f;
    public bool setBoidColorBasedOnFlockId = true;

    [Header("Other Settings")]
    [Range(0, 1.0f)] public float avoidanceThreshold = 0.2f;
    [Range(0, 10)] public int distanceFromLeader = 2;
    public bool useScreenWarp = true;
    public bool fixZPosition = true;
    public bool boundToArea = false;
    public float minX = -20;
    public float maxX = 20;
    public float minY = -20;
    public float maxY = 20;
}
