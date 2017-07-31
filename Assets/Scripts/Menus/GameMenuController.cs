using UnityEngine;
using System.Collections;

public class GameMenuController : MonoBehaviour {

	public void PlayAgainButtonClick() {
		UnityEngine.SceneManagement.SceneManager.LoadScene ( "CraigDevScene" );
	}

	public void MainMenuButtonClick () {
		UnityEngine.SceneManagement.SceneManager.LoadScene ( "MainMenu" );
	}
}

