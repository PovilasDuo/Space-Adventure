using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
	public GameObject explosionPS;
	public AudioSource explosionAudio;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag != "Asteroid")
		{
			Instantiate(explosionPS, this.gameObject.transform.position, Quaternion.identity);

			if (explosionAudio.enabled)
			{
                explosionAudio.Play();
            }

			if(collision.collider.tag == "Player")
			{
				Proxy proxyManager = new Proxy("GameManager");
				UIControl uIControl = proxyManager.GetObject().GetComponent<UIControl>();
				if (uIControl.versus)
				{
					collision.collider.GetComponent<RocketShipController>().health--;
					if (collision.collider.gameObject.layer == 9)
					{
						uIControl.lives2--;
					}
					if (collision.collider.gameObject.layer == 3)
					{
						uIControl.lives--;
					}
				}
				else
				{
					collision.collider.GetComponent<RocketShipController>().health--;
					uIControl.lives--;
				}
			}
			this.gameObject.GetComponent<Collider>().enabled = false;
			this.gameObject.GetComponent<MeshRenderer>().enabled = false;
			Destroy(this.gameObject, 1f);
		}
	}
}
