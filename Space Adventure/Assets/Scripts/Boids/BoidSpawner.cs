using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    public GameObject boidPrefab;
    public Camera mainCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnBoidAtClick();
        }
    }

    void SpawnBoidAtClick()
    {
        if (boidPrefab == null || mainCamera == null)
        {
            Debug.LogWarning("Boid Prefab or Main Camera is not assigned!");
            return;
        }

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Instantiate(boidPrefab, worldPosition, Quaternion.identity);
    }
}
