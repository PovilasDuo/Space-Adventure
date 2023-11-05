using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartSiren : MonoBehaviour
{
	public AudioSource siren;
	private GameObject rocket;
	private bool playing;
	// Start is called before the first frame update
	void Start()
    {
		playing = false;
		rocket = GameObject.Find("Rocket");
	}

    // Update is called once per frame
    void Update()
    {
		if (rocket != null)
		{
			if (!playing)
			{
				if (rocket.GetComponent<RocketShipController>().health == 1)
				{
					siren.Play();
					playing = true;
				}
			}	
			if (rocket.GetComponent<RocketShipController>().health == 0)
			{
				siren.Stop();
			}
		}
		
	}
}
