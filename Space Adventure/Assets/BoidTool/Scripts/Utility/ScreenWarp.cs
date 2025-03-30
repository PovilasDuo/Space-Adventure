using UnityEngine;

public class ScreenWarp : MonoBehaviour
{
    private Camera mainCamera;
    private bool isVisible;
    private TrailRenderer trailRenderer;

    void Start()
    {
        mainCamera = Camera.main;
        TryGetComponent<TrailRenderer>(out trailRenderer);
    }

    void FixedUpdate()
    {
        CheckVisibility();
        if (!isVisible)
        {
            Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

            if (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1)
            {
                if (viewportPosition.x < 0)
                {
                    transform.position = new Vector3(mainCamera.ViewportToWorldPoint(new Vector3(1, viewportPosition.y, viewportPosition.z)).x, transform.position.y, transform.position.z);
                    ClearTrail();
                }
                else if (viewportPosition.x > 1)
                {
                    transform.position = new Vector3(mainCamera.ViewportToWorldPoint(new Vector3(0, viewportPosition.y, viewportPosition.z)).x, transform.position.y, transform.position.z);
                    ClearTrail();
                }

                if (viewportPosition.y < 0)
                {
                    transform.position = new Vector3(transform.position.x, mainCamera.ViewportToWorldPoint(new Vector3(viewportPosition.x, 1, viewportPosition.z)).y, transform.position.z);
                    ClearTrail();
                }
                else if (viewportPosition.y > 1)
                {
                    transform.position = new Vector3(transform.position.x, mainCamera.ViewportToWorldPoint(new Vector3(viewportPosition.x, 0, viewportPosition.z)).y, transform.position.z);
                    ClearTrail();
                }
            }
        }
    }

    void CheckVisibility()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        isVisible = GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider>().bounds);
    }

    void ClearTrail()
    {
        if (trailRenderer != null && trailRenderer.enabled)
        {
            trailRenderer.enabled = false;
            trailRenderer.Clear();
            trailRenderer.enabled = true;
        }
    }
}
