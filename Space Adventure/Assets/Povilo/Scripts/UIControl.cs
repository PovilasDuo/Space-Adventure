using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
	public GameObject pausePanel;
	private bool paused;

	public TextMeshProUGUI highScoreText;
	public int highScore;

	public GameObject gameOverPanel;
	private bool gameOver;

	public TextMeshProUGUI livesText;
	public int lives;

	public TextMeshProUGUI highScoreMessage;
	private const string HighScoreKey = "HighScore";

	// Start is called before the first frame update
	void Start()
    {
		pausePanel.SetActive(false);
		paused = false;

		gameOverPanel.SetActive(false);
		gameOver = false;

		Proxy proxy = new Proxy("Rocket");
		lives = proxy.GetObject().GetComponent<RocketShipController>().health;
	}

    // Update is called once per frame
    void Update()
    {
		highScoreText.text = highScore.ToString();
		livesText.text = lives.ToString();
		Pause();
		GameOver();
	}

	/// <summary>
	/// Pauses the game and enables the pause panel
	/// </summary>
	private void Pause()
	{
		if (Input.GetKeyDown("escape") && paused)
		{
			Time.timeScale = 1f;
			pausePanel.SetActive(false);
			paused = false;
		}
		else if (Input.GetKeyDown("escape") && !paused)
		{
			Time.timeScale = 0f;
			pausePanel.SetActive(true);
			paused = true;
		}
	}

	/// <summary>
	/// Exits the game
	/// </summary>
	public void ExitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}

	/// <summary>
	/// Triggers a game over panel when player's health is depleted
	/// </summary>
	public void GameOver()
	{
		if (!gameOver)
		{
			Proxy proxy = new Proxy("Rocket");
			GameObject rocket = proxy.GetObject();
			if (rocket != null)
			{
				if (rocket.GetComponent<RocketShipController>().health <= 0)
				{
					gameOver = true;
					gameOverPanel.SetActive(true);
					gameOverPanel.GetComponentInChildren<TextMeshProUGUI>().text += highScore.ToString();

					highScoreText.gameObject.SetActive(false);
					livesText.gameObject.SetActive(false);
					rocket.SetActive(false);

					int currentHighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
					if (highScore >= currentHighScore)
					{
						PlayerPrefs.SetInt(HighScoreKey, highScore);
						PlayerPrefs.Save();
						highScoreMessage.gameObject.SetActive(true);
					}
				}
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void BackToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
