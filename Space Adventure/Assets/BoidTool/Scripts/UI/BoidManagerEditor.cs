using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoidManager))]
public class BoidManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BoidManager boidManager = (BoidManager)target;
        DrawDefaultInspector();
        EditorGUILayout.Space();

        BoidSettings boidSettings = boidManager.GetBoidSettings();
        BoidManagerSettings boidManagerSettings = boidManager.GetBoidManagerSettings();

        if (boidSettings != null && boidManagerSettings != null)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Boid Settings", EditorStyles.boldLabel);
            boidManagerSettings.manageBoids = EditorGUILayout.Toggle("Manage boids", boidManagerSettings.manageBoids);
            boidManagerSettings.updateInterval = EditorGUILayout.Slider("Boid Update Interval", boidManagerSettings.updateInterval, 0, 1f);

            boidManagerSettings.boidPrefab = (GameObject)EditorGUILayout.ObjectField(boidManagerSettings.boidPrefab, typeof(GameObject), true);

            if (boidManagerSettings.boidPrefab)
            {
                if (GUILayout.Button("Spawn boid"))
                {
                    boidManager.SpawnBoid(Vector3.zero, boidManagerSettings.boidPrefab, boidSettings.flockID);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Assign a boid prefab to spawn a boid", MessageType.Info);
            }

            EditorGUILayout.LabelField("Flock Settings", EditorStyles.boldLabel);
            boidSettings.flockID = EditorGUILayout.IntSlider("Flock ID", boidSettings.flockID, 0, 9);

            EditorGUILayout.LabelField("Vision Settings", EditorStyles.boldLabel);
            boidSettings.visionRadius = EditorGUILayout.IntSlider("Vision Radius", boidSettings.visionRadius, 1, 50);
            boidSettings.visionAngle = EditorGUILayout.IntSlider("Vision Angle", boidSettings.visionAngle, 1, 180);

            EditorGUILayout.LabelField("Movement Settings", EditorStyles.boldLabel);
            boidSettings.speed = EditorGUILayout.IntSlider("Speed", boidSettings.speed, 0, 20);
            boidSettings.speedScalingFactor = EditorGUILayout.Slider("Speed Scaling Factor", boidSettings.speedScalingFactor, 0, 10);

            EditorGUILayout.LabelField("Behavior Weights", EditorStyles.boldLabel);
            boidSettings.cohesionWeight = EditorGUILayout.Slider("Cohesion Weight", boidSettings.cohesionWeight, 0, 5);
            boidSettings.alignmentWeight = EditorGUILayout.Slider("Alignment Weight", boidSettings.alignmentWeight, 0, 5);
            boidSettings.separationWeight = EditorGUILayout.Slider("Separation Weight", boidSettings.separationWeight, 0, 5);

            boidSettings.leaderFollowWeight = EditorGUILayout.IntSlider("Leader Follow Weight", boidSettings.leaderFollowWeight, 0, 10);
            boidSettings.avoidanceWeight = EditorGUILayout.IntSlider("Avoidance Weight", boidSettings.avoidanceWeight, 0, 10);
            boidSettings.lineFormationWeight = EditorGUILayout.Slider("Line Formation Weight", boidSettings.lineFormationWeight, 0, 10);
            boidSettings.enemyInteractionWeight = EditorGUILayout.IntSlider("Enemy Interaction Weight", boidSettings.enemyInteractionWeight, 0, 10);
            boidSettings.boundToAreWeight = EditorGUILayout.IntSlider("Bound to Area Weight", boidSettings.boundToAreWeight, 0, 10);

            EditorGUILayout.LabelField("Behavioral Settings", EditorStyles.boldLabel);
            boidSettings.isAggressive = EditorGUILayout.Toggle("Aggressive Enemy Interaction?", boidSettings.isAggressive);
            boidSettings.interactsWithEnemies = EditorGUILayout.Toggle("Interacts With Enemies?", boidSettings.interactsWithEnemies);

            SerializedObject serializedBoidSettings = new SerializedObject(boidSettings);
            serializedBoidSettings.Update();
            EditorGUILayout.PropertyField(serializedBoidSettings.FindProperty("enemyInteractionActions"), true);
            EditorGUILayout.PropertyField(serializedBoidSettings.FindProperty("allyInteractionActions"), true);
            EditorGUILayout.PropertyField(serializedBoidSettings.FindProperty("rayCastInteractionActions"), true);
            serializedBoidSettings.ApplyModifiedProperties();

            boidSettings.leader = null;

            EditorGUILayout.LabelField("Stuck Detection", EditorStyles.boldLabel);
            boidSettings.stuckThreshold = EditorGUILayout.Slider("Stuck Threshold", boidSettings.stuckThreshold, 0.01f, 1f);
            boidSettings.maxStuckTime = EditorGUILayout.Slider("Max Stuck Time", boidSettings.maxStuckTime, 0.1f, 5f);

            EditorGUILayout.LabelField("Physics & Rendering", EditorStyles.boldLabel);
            boidSettings.numberOfRays = EditorGUILayout.IntSlider("Number of Rays", boidSettings.numberOfRays, 1, 16);
            boidSettings.rayCastCooldown = EditorGUILayout.Slider("Ray Cast Cooldown", boidSettings.rayCastCooldown, 0, 1f);
            boidSettings.lineSegments = EditorGUILayout.IntSlider("Line Segments", boidSettings.lineSegments, 6, 64);

            EditorGUILayout.LabelField("Visualization", EditorStyles.boldLabel);
            boidSettings.displayVisionRange = EditorGUILayout.Toggle("Display Vision Range", boidSettings.displayVisionRange);
            boidSettings.visualizeColors = EditorGUILayout.Toggle("Use Colors to Visualize Boid Behavior", boidSettings.visualizeColors);
            boidSettings.useTrail = EditorGUILayout.Toggle("Use trail", boidSettings.useTrail);
            boidSettings.trailTime = EditorGUILayout.Slider("Trail time", boidSettings.trailTime, 0, 3f);
            boidSettings.setBoidColorBasedOnFlockId = EditorGUILayout.Toggle("Set Color Based on Flock ID", boidSettings.setBoidColorBasedOnFlockId);

            EditorGUILayout.LabelField("Other Settings", EditorStyles.boldLabel);
            boidSettings.avoidanceThreshold = EditorGUILayout.Slider("Avoidance Threshold", boidSettings.avoidanceThreshold, 0, 1f);
            boidSettings.distanceFromLeader = EditorGUILayout.IntSlider("Distance from Leader", boidSettings.distanceFromLeader, 0, 10);

            boidSettings.useScreenWarp = EditorGUILayout.Toggle("Use Screen Warp", boidSettings.useScreenWarp);
            boidSettings.fixZPosition = EditorGUILayout.Toggle("Fix Z Position", boidSettings.fixZPosition);
            boidSettings.boundToArea = EditorGUILayout.Toggle("Bound to Area", boidSettings.boundToArea);
            boidSettings.minX = EditorGUILayout.Slider("MinX", boidSettings.minX, -100, 100);
            boidSettings.maxX = EditorGUILayout.Slider("MaxX", boidSettings.maxX, -100, 100);
            boidSettings.minY = EditorGUILayout.Slider("MinY", boidSettings.minY, -100, 100);
            boidSettings.maxY = EditorGUILayout.Slider("MaxY", boidSettings.maxY, -100, 100);

            if (EditorGUI.EndChangeCheck())
            {
                boidManager.UpdatetFlockSettings();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("BoidSettings is not assigned!", MessageType.Warning);
        }
    }
}
