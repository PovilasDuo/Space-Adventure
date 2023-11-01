using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
	private int collisionCount = 0;

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

		CheckVisibility();
		if (!isVisible)
		{
			Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
			if (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1)
			{

				if (viewportPosition.x < 0 || viewportPosition.x > 1)
				{
					float xValue = transform.position.x * -1;
					transform.position = new Vector3(xValue, transform.position.y, transform.position.z);
				}
				else if (viewportPosition.y < 0 || viewportPosition.y > 1)
				{
					float yValue = transform.position.y * -1;
					transform.position = new Vector3(transform.position.x, yValue, transform.position.z);

				}
				collisionCount++;
			}
		}
	}

	// Check if the bullet is visible in the camera view
	void CheckVisibility()
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
		isVisible = GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider>().bounds);
	}
}
