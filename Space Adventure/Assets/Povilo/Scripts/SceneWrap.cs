using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SceneWrap : MonoBehaviour
{
	private Camera mainCamera;
	private bool isVisible;
	public int collisionCount = 0;

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
					float xValue = transform.position.x * -0.9f;
					transform.position = new Vector3(xValue, transform.position.y, transform.position.z);
				}
				else if (viewportPosition.y < 0 || viewportPosition.y > 1)
				{
					float yValue = transform.position.y * -0.9f;
					transform.position = new Vector3(transform.position.x, yValue, transform.position.z);
				
				}
				if (this.tag != "Player")
				{
					collisionCount++;
				}
			}
		}
	}

	/// <summary>
	/// Checks if the object is visible within the cameras projected planes
	/// </summary>
	void CheckVisibility()
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
		isVisible = GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider>().bounds);
	}
}
