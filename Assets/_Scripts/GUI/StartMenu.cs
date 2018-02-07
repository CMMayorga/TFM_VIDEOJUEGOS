using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

	public void LoadGame()
    {
        SceneManager.LoadScene("TheGame", LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
