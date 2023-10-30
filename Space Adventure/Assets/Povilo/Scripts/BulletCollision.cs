using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
	public int collisionCount = 0;

	private Camera mainCamera;
	private bool isVisible;

	// Start is called before the first frame update
	void Start()
	{
		mainCamera = Camera.main;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (collisionCount == 2)
		{
			Destroy(this.gameObject);
		}
		// Check if the bullet is visible in the camera view
		CheckVisibility();

		// If not visible, move the bullet to the opposite side
		if (!isVisible)
		{
			Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

			if (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1)
			{
				transform.position *= -1;
			}
			collisionCount++;
		}
		transform.Translate(Vector3.forward * Time.deltaTime);
	}

	// Check if the bullet is visible in the camera view
	void CheckVisibility()
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
		isVisible = GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider>().bounds);
	}
}
