using UnityEngine;

public class AsteroidCollision : MonoBehaviour
{
	/// <summary>
	/// Simple collision
	/// </summary>
	/// <param name="collision">The collision that occured</param>
	private void OnCollisionEnter(Collision collision)
	{
		//For bullets
		if (collision.collider.tag == "Bullet")
		{
			//Different cases based on the asteroid size
			if (this.transform.localScale == new Vector3(3f, 3f, 3f))
			{
				GameObject asteroid1 = Instantiate(this.gameObject);
				GameObject asteroid2 = Instantiate(this.gameObject);
				GameObject asteroid3 = Instantiate(this.gameObject);
				asteroid1.transform.localScale = new Vector3(2, 2, 2);
				asteroid2.transform.localScale = new Vector3(2, 2, 2);
				asteroid3.transform.localScale = new Vector3(2, 2, 2);

				asteroid1.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0);
				asteroid2.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0);
				asteroid3.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0);

				Destroy(this.gameObject);
			}
			else if (this.transform.localScale == new Vector3(2f, 2f, 2f))
			{
				GameObject asteroid1 = Instantiate(this.gameObject);
				GameObject asteroid2 = Instantiate(this.gameObject);
				asteroid1.transform.localScale = new Vector3(1, 1, 1);
				asteroid2.transform.localScale = new Vector3(1, 1, 1);

				asteroid1.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), 0);
				asteroid2.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), 0);

				Destroy(this.gameObject);
			}
			else
			{
				Destroy(collision.collider.gameObject);
				Destroy(this.gameObject);
			}
		}
		//Collision with the Player (Rocket)
		else if (collision.collider.tag == "Player")
		{
			Destroy(this.gameObject);
			collision.collider.GetComponent<RocketShipController>().health -= 1;
		}
	}
}
