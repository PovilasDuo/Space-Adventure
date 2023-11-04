using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AsteroidCollision : MonoBehaviour
{
	private List<GameObject> asteroidList;
	private AudioSource[] audioList;
	public GameObject explosionPS;

	/// <summary>
	/// 
	/// </summary>
	private void Start()
	{
		audioList = GameObject.Find("AudioManager").GetComponents<AudioSource>();
		asteroidList = GameObject.FindWithTag("GameManager").GetComponent<AsteroidSpawner>().asteroidList;
	}
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
				SpawnAsteroid(3, 2f, 10f);
				AsteroidDestruction(audioList[1], collision.gameObject, 3);

			}
			else if (this.transform.localScale == new Vector3(2f, 2f, 2f))
			{
				SpawnAsteroid(2, 1f, 20f);
				AsteroidDestruction(audioList[1], collision.gameObject, 2);
			}
			else
			{
				AsteroidDestruction(audioList[1], collision.gameObject, 1);
			}
		}
		//Collision with the Player (Rocket)
		else if (collision.collider.tag == "Player")
		{
			collision.collider.GetComponent<RocketShipController>().health--;
			GameObject.FindGameObjectWithTag("GameManager").GetComponent<UIControl>().lives--;
			AsteroidDestruction(audioList[2], new GameObject(), 0);
		}
	}

	/// <summary>
	/// Spawns asteroids
	/// </summary>
	/// <param name="numberOfAsteroidsToSpawn">Number of asteroids needed to spawn</param>
	/// <param name="asteroidSize">Size of the asteroids that will be spawned (scaled cubicly)</param>
	/// <param name="asteroidVelocity">Velocity of the asteroids that will be spawned</param>

	private void SpawnAsteroid(int numberOfAsteroidsToSpawn, float asteroidSize, float asteroidVelocity)
	{
		for (int i = 0; i < numberOfAsteroidsToSpawn; i++)
		{
			GameObject asteroid = Instantiate(asteroidList[Random.Range(0, asteroidList.Count - 1)], this.transform.position, Quaternion.identity);
			asteroid.transform.localScale = new Vector3(asteroidSize, asteroidSize, asteroidSize);
			asteroid.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-asteroidVelocity, asteroidVelocity), Random.Range(-asteroidVelocity, asteroidVelocity), 0);
		}
	}

	/// <summary>
	/// Destroys the asteroid and the bullet
	/// </summary>
	/// <param name="audioSource">Audio source played upon destruction</param>
	/// <param name="bullet">Bullet GameObject that is destroyed alongside the asteroid</param>
	/// <param name="highScoreIncrease">The amount that the highscore is increased</param>
	private void AsteroidDestruction(AudioSource audioSource, GameObject bullet, int highScoreIncrease)
	{
		GameObject.FindGameObjectWithTag("GameManager").GetComponent<UIControl>().highScore += highScoreIncrease;
		audioSource.Play();
		Instantiate(explosionPS, this.transform.position, Quaternion.identity);
		GetComponent<MeshRenderer>().enabled = false;
		GetComponent<MeshCollider>().enabled = false;
		Destroy(bullet);
		Destroy(this.gameObject, audioSource.clip.length);
	}
}
