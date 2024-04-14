using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
	public GameObject pausePanel;
	private bool paused = false;

	public TextMeshProUGUI highScoreText;
	public int highScore;

	public GameObject gameOverPanel;
	private bool gameOver;

	public TextMeshProUGUI livesText;
	public int lives;

	public TextMeshProUGUI livesText2;
	public int lives2;

	public TextMeshProUGUI highScoreMessage;
	private const string HighScoreKey = "HighScore";
	public bool versus = false;

	public TextMeshProUGUI GameOverText;

	// Start is called before the first frame update
	void Start()
    {
		Time.timeScale = 1f;
		pausePanel.SetActive(false);
		paused = false;

		gameOverPanel.SetActive(false);
		gameOver = false;

		Proxy proxy = new Proxy("Rocket");
		lives = proxy.GetObject().GetComponent<RocketShipController>().health;
		lives2 = 1;
		//For versus
		if (SceneManager.GetActiveScene().name == "MultiVersus")
		{
			Proxy proxy2 = new Proxy("Rocket2");
			GameObject rocket2 = proxy2.GetObject();
			lives2 = rocket2.GetComponent<RocketShipController>().health;
			versus = true;
		}
	}

    // Update is called once per frame
    void Update()
    {
		if (versus)
		{
			highScoreText.gameObject.SetActive(false);
			livesText.text = lives.ToString();
			livesText2.text = lives2.ToString();
		}
		else
		{
			//livesText2.gameObject.SetActive(false);
			highScoreText.text = highScore.ToString();
			livesText.text = lives.ToString();
		}
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
			Debug.Log("whatthefuck");
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
			Proxy proxy2 = new Proxy("Rocket2");
			GameObject rocket2 = proxy2.GetObject();
			if (lives <= 0 || (lives2 <= 0 && rocket2 != null))
			{
				gameOver = true;
				gameOverPanel.SetActive(true);
				if (!versus)
				{
					gameOverPanel.GetComponentInChildren<TextMeshProUGUI>().text += highScore.ToString();
					int currentHighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
					if (highScore >= currentHighScore)
					{
						PlayerPrefs.SetInt(HighScoreKey, highScore);
						PlayerPrefs.Save();
						highScoreMessage.gameObject.SetActive(true);
					}
					highScoreText.gameObject.SetActive(false);
					livesText.gameObject.SetActive(false);
					//livesText2.gameObject.SetActive(false);
					if (rocket2 != null)
					{
						rocket2.SetActive(false);
					}
					rocket.SetActive(false);
				}
				else
				{
					if (lives >= 0 && lives2 <= 0)
					{
						rocket2.SetActive(false);
						GameOverText.text = "GAME OVER\nPLAYER 1 WINS!";
					}
					else
					{
						rocket.SetActive(false);
						GameOverText.text = "GAME OVER\nPLAYER 2 WINS!";
					}
					highScoreText.gameObject.SetActive(false);
					livesText.gameObject.SetActive(false);
					livesText2.gameObject.SetActive(false);
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
