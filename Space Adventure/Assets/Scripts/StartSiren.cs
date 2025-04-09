using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class StartSiren : MonoBehaviour
{
	public AudioSource siren;
	private bool playing;
	private Proxy proxyManager = new Proxy("GameManager");
	private UIControl uIControl;

	void Start()
    {
		playing = false;
		uIControl = proxyManager.GetObject().GetComponent<UIControl>();
	}

    void Update()
    {
		if (proxyManager.GetObject() != null)
		{
			if (!playing)
			{
				if (uIControl.versus)
				{
					if (uIControl.lives == 1 || uIControl.lives2 == 1)
					{
						siren.Play();
						playing = true;
					}
				}
				else
				{
					if (uIControl.lives == 1)
					{
						siren.Play();
						playing = true;
					}
				}
			}
			if (uIControl.versus)
			{
				if ((uIControl.lives == 0 || uIControl.lives > 1) || (uIControl.lives2 == 0 || uIControl.lives2 > 1))
				{
					siren.Stop();
				}
			}
			else
			{
				if (uIControl.lives == 0 || uIControl.lives > 1)
                {
					siren.Stop();
				}
			}
		}
		
	}
}
