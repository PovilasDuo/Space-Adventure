using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AsteroidCollision : MonoBehaviour
{
	public GameObject explosionPS;
	public bool isPowerUp = false;

	/// <summary>
	/// Gets the required things for the game object and script
	/// </summary>
	private void Start()
	{
		StartCoroutine(AsteroidSceneWrap(this.gameObject));
	}
	/// <summary>
	/// Simple collision
	/// </summary>
	/// <param name="collision">The collision that occured</param>
	private void OnCollisionEnter(Collision collision)
	{
		Facade facade = new Facade(this.gameObject, explosionPS);
		facade.HandleCollision(collision);
	}

	/// <summary>
	/// Enables the SceneWrap script after a few seconds
	/// </summary>
	/// <param name="asteroid">The asteroid that the script will be enabled</param>
	/// <returns>Time</returns>
	private IEnumerator AsteroidSceneWrap(GameObject asteroid)
	{
		if (asteroid.GetComponent<SceneWrap>().enabled == false)
		{
			yield return new WaitForSeconds(3f);
			asteroid.GetComponent<SceneWrap>().enabled = true;
		}
	}
}
