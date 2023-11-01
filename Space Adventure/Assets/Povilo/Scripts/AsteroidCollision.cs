using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidCollision : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == "Bullet")
		{
			Destroy(collision.collider.gameObject);
			Destroy(this.gameObject);
		}
		else if (collision.collider.tag == "Player")
		{
			Destroy(this.gameObject);
			collision.collider.GetComponent<RocketShipController>().health -= 1;
		}
	}
}
