using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartSiren : MonoBehaviour
{
	public AudioSource siren;
	private GameObject gameManager;
	private bool playing;
	// Start is called before the first frame update
	void Start()
    {
		playing = false;
		gameManager = GameObject.Find("Rocket");
	}

    // Update is called once per frame
    void Update()
    {
		if (gameManager != null)
		{
			if (!playing)
			{
				if (gameManager.GetComponent<UIControl>().lives == 1)
				{
					siren.Play();
					playing = true;
				}
			}	
			if (gameManager.GetComponent<UIControl>().lives == 0)
			{
				siren.Stop();
			}
		}
		
	}
}
