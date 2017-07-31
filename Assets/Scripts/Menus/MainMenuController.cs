using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {

	public GameObject ControlsGameObject;
	public GameObject MainMenuGameObject;


    public void HandlePlayGameButtonClicked() {
        ChangeScene();
    }

    private void ChangeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("CraigDevScene");
    }

    public void HandleControlsButtonClicked() {
		MainMenuGameObject.SetActive ( false );
		ControlsGameObject.SetActive ( true );
	}

	public void HandleQuitButtonClicked() {
		Application.Quit ( );
	}

	public void HandleControlsBackButtonClicked() {
		MainMenuGameObject.SetActive ( true );
		ControlsGameObject.SetActive ( false );
	}
}
