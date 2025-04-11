using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
	public GameObject mainMenuPanel;
	public GameObject settingsPanel;
	public GameObject gameModePanel;

	public Slider gameVolumeSlider;

	public TMP_Dropdown resolutionDropdown;

	private Resolution[] resolutions;

	void Start()
	{
		gameVolumeSlider.value = AudioListener.volume;
		gameVolumeSlider.onValueChanged.AddListener(delegate { GameVolumeChange(); });

		resolutions = Screen.resolutions;

		resolutionDropdown.ClearOptions();

		List<string> resolutionOptions = new List<string>();
		foreach (var resolution in resolutions)
		{
			resolutionOptions.Add(resolution.width + "x" + resolution.height);
		}
		resolutionDropdown.AddOptions(resolutionOptions);
		resolutionDropdown.value = GetCurrentResolutionIndex();
	}

	public void ShowSettingsPanel()
	{
		settingsPanel.SetActive(true);
		mainMenuPanel.SetActive(false);
	}

	public void ShowGameModePanel()
	{
		gameModePanel.SetActive(true);
		mainMenuPanel.SetActive(false);
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

	public void SinglePlayer()
	{
		SceneManager.LoadScene("SinglePlayer");
	}

	public void MultiCoOp()
	{
		SceneManager.LoadScene("MultiCoOp");
	}

	public void MultiVersus()
	{
		SceneManager.LoadScene("MultiVersus");
	}

    public void Demo()
    {
        SceneManager.LoadScene("Demo");
    }

    public void Back()
	{
		mainMenuPanel.SetActive(true);
		if (settingsPanel.activeSelf || gameModePanel.activeSelf)
		{
			settingsPanel.SetActive(false);
			gameModePanel.SetActive(false);
		}
	}

	public void GameVolumeChange()
	{
		AudioListener.volume = gameVolumeSlider.value;
	}

	private int GetCurrentResolutionIndex()
	{
		Resolution currentResolution = Screen.currentResolution;
		for (int i = 0; i < resolutions.Length; i++)
		{
			if (resolutions[i].width == currentResolution.width &&
				resolutions[i].height == currentResolution.height)
			{
				return i;
			}
		}
		return 0;
	}

	public void OnResolutionChanged()
	{
		string selectedResolutionStr = resolutionDropdown.options[resolutionDropdown.value].text;
		string[] resolutionParts = selectedResolutionStr.Split('x');
		if (resolutionParts.Length == 2 && int.TryParse(resolutionParts[0], out int width) && int.TryParse(resolutionParts[1], out int height))
		{
			Screen.SetResolution(width, height, Screen.fullScreen);
		}
	}
}
