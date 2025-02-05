using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Facade : Object
{
	//Properties
	private List<GameObject> asteroidList;
	private AudioSource[] audioList;
	private GameObject mainObject;
	private GameObject explosionPS;
	private GameObject powerUp;

	/// <summary>
	/// Constructor with parameters
	/// </summary>
	/// <param name="mainObject"></param>
	/// <param name="explosionPS"></param>
	public Facade(GameObject mainObject, GameObject explosionPS)
	{
		this.mainObject = mainObject;
		this.explosionPS = explosionPS;
	}

	/// <summary>
	/// Main facade usage - handles the collision with an object
	/// </summary>
	/// <param name="collision">The collision that occured</param>
	public void HandleCollision(Collision collision)
	{
		//Set up
		Proxy proxyAudio = new Proxy("AudioManager");
		Proxy proxyManager = new Proxy("GameManager");
		audioList = proxyAudio.GetObject().GetComponents<AudioSource>();
		asteroidList = proxyManager.GetObject().GetComponent<AsteroidSpawner>().asteroidList;
		powerUp = proxyManager.GetObject().GetComponent<AsteroidSpawner>().powerUp;

		//For bullets
		if (collision.collider.tag == "Bullet")
		{
			//Different cases based on the asteroid size
			if (mainObject.transform.localScale == new Vector3(3f, 3f, 3f))
			{
				SpawnAsteroid(3, 2f, 5f);
				AsteroidDestruction(audioList[1], collision.gameObject, 3);

			}
			else if (mainObject.transform.localScale == new Vector3(2f, 2f, 2f))
			{
				SpawnAsteroid(2, 1f, 10f);
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
			GameObject asteroid = Instantiate(asteroidList[Random.Range(0, asteroidList.Count - 1)], mainObject.transform.position, Quaternion.identity);
			asteroid.transform.localScale = new Vector3(asteroidSize, asteroidSize, asteroidSize);
			asteroid.GetComponent<Rigidbody>().linearVelocity = new Vector3(Random.Range(-asteroidVelocity, asteroidVelocity), Random.Range(-asteroidVelocity, asteroidVelocity), 0);
			asteroid.GetComponent<SceneWrap>().collisionCount = 0;
			asteroid.GetComponent<SceneWrap>().enabled = true;

			if (asteroid.GetComponent<AsteroidCollision>().isPowerUp)
			{
				asteroid.GetComponent<AsteroidCollision>().isPowerUp = false;
			}
			else
			{
				//Power up spawning
				int powerUpRNG = Random.Range(0, 100);
				if (powerUpRNG >= 75)
				{
					asteroid.GetComponent<Renderer>().material.color = Color.cyan;
					asteroid.GetComponent<AsteroidCollision>().isPowerUp = true;
				}
			}
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
		Instantiate(explosionPS, mainObject.transform.position, Quaternion.identity);
		mainObject.GetComponent<MeshRenderer>().enabled = false;
		mainObject.GetComponent<MeshCollider>().enabled = false;
		Destroy(bullet);
		Destroy(mainObject, audioSource.clip.length);
		if (mainObject.GetComponent<AsteroidCollision>().isPowerUp)
		{
			GameObject powerUpI = Instantiate(powerUp, mainObject.transform.position, Quaternion.identity);
			powerUpI.GetComponent<Rigidbody>().angularVelocity = new Vector3(1f, 1f) * 2f;
		}
	}
}
