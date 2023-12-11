using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpCollision : MonoBehaviour
{
	private void OnCollisionEnter(Collision collider)
	{
		string infoMessage = "";
		if (collider.gameObject.tag == "Player")
		{
			GameObject Player = collider.gameObject;
			int powerUpPicker = Random.Range(1, 6);
			switch (powerUpPicker)
			{
				case 1:
					Proxy proxyManager = new Proxy("GameManager");
					UIControl uIControl = proxyManager.GetObject().GetComponent<UIControl>();
					if (uIControl.versus)
					{
						Player.GetComponent<RocketShipController>().health++;
						if (collider.collider.gameObject.layer == 9)
						{
							uIControl.lives2++;
						}
						if (collider.collider.gameObject.layer == 3)
						{
							uIControl.lives++;
						}
					}
					else
					{
						Player.GetComponent<RocketShipController>().health++;
						GameObject.FindGameObjectWithTag("GameManager").GetComponent<UIControl>().lives++;
					}
					infoMessage = "Health Increased!";
					break;
				case 2:
					Player.GetComponent<RocketShipController>().speed += 5;
					Player.GetComponent<RocketShipController>().rotationSpeed += 100;
					infoMessage = "Speed Increased!";
					break;
				case 3:
					if (Player.GetComponent<RocketShipController>().speed - 5f <= 0)
					{
						Player.GetComponent<RocketShipController>().speed = 1f;
						Player.GetComponent<RocketShipController>().rotationSpeed = 20;
						infoMessage = "Speed Decreased!";
					}
					else
					{
						Player.GetComponent<RocketShipController>().speed -= 5f;
						Player.GetComponent<RocketShipController>().rotationSpeed -= 100;
						infoMessage = "Speed Decreased!";
					}
					break;
				case 4:
					if (Player.GetComponent<RocketShipController>().fireRate -2f <= 0)
					{
						Player.GetComponent<RocketShipController>().fireRate = 1f;
						infoMessage = "Fire Rate Decreased!";
					}
					else
					{
						Player.GetComponent<RocketShipController>().fireRate -= 2f;
						infoMessage = "Fire Rate Decreased!";
					}
					break;
				case 5:
					Player.GetComponent<RocketShipController>().fireRate += 2f;
					infoMessage = "Fire Rate Increased!";
					break;
			}
			GameObject textObject = new GameObject("ChildText");
			GameObject canvas = GameObject.Find("CanvasPowerUp");
			textObject.transform.SetParent(canvas.transform);
			TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
			textComponent.font = Resources.Load<TMP_FontAsset>("Fonts/RoboticCyborgItalic-2O48l SDF");
			textComponent.fontSize = 8;
			textComponent.color = Color.white;
			textComponent.alignment = TextAlignmentOptions.Center;
			textComponent.alignment = TextAlignmentOptions.Midline;
			textObject.transform.localScale = new Vector3(0.1f, 0.1f);
			textObject.transform.position = this.gameObject.transform.position;
			this.gameObject.GetComponent<MeshRenderer>().enabled = false;
			this.gameObject.GetComponent<BoxCollider>().enabled = false;
			Proxy proxyAudio = new Proxy("AudioManager");
			AudioSource audio = proxyAudio.GetObject().GetComponents<AudioSource>()[5];
			audio.Play();
			StartCoroutine(DisplayInfoMessage(textComponent, infoMessage));
		}	
	}
	private IEnumerator DisplayInfoMessage(TextMeshProUGUI infoText, string message)
	{
		infoText.text = message;
		infoText.CrossFadeAlpha(1f, 0f, false);
		yield return new WaitForSeconds(1f);
		infoText.CrossFadeAlpha(0f, 1f, false);
		yield return new WaitForSeconds(1f);
		Destroy(this.gameObject);
	}
}
