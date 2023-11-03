using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class UIControl : MonoBehaviour
{
	public GameObject pausePanel;
	private bool paused;

	public TextMeshProUGUI highScoreText;
	public int highScore;

	// Start is called before the first frame update
	void Start()
    {
		pausePanel.SetActive(false);
		paused = false;
	}

    // Update is called once per frame
    void Update()
    {
		highScoreText.text = highScore.ToString();
		Pause();
	}


	private void Pause()
	{
		if (Input.GetKeyDown("escape") && paused)
		{
			Time.timeScale = 1f;
			pausePanel.SetActive(false);
			paused = false;
			Debug.Log("ASD");
		}
		else if (Input.GetKeyDown("escape") && !paused)
		{
			Time.timeScale = 0f;
			pausePanel.SetActive(true);
			paused = true;
			Debug.Log("ASDASDASD");
		}
	}
}
